import {AfterViewInit, Directive, ElementRef, forwardRef, HostListener, Input, OnDestroy} from '@angular/core';
import {LocalizationService} from '../services/internationalization/localization.service';
import {DateTime} from 'luxon';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';
import {DateObjectUnits} from 'luxon/src/datetime';
import {DateParserService} from '../services/date-parser.service';

const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DatePickerDirective),
  multi: true,
};

export type ViewMode = 'days' | 'months' | 'years';

export interface DatePickerOptions {
  format: string,
  todayHighlight: boolean,
  calendarWeeks: boolean,
  weekStart: number,
  autoclose: boolean,
  language: string,
  todayBtn: boolean | 'linked',
  clearBtn: boolean,
  showOnFocus: boolean,
  assumeNearbyYear: boolean,
  forceParse: boolean,
  keyboardNavigation: boolean,
  minViewMode: ViewMode
  startDate: Date | undefined,
  endDate: Date | undefined,
}

@Directive({
  selector: '[tsDatePicker]',
  providers: [CUSTOM_VALUE_ACCESSOR],
})
export class DatePickerDirective implements AfterViewInit, OnDestroy, ControlValueAccessor {
  @Input() relativeAnchor: 'start' | 'end' = 'start';

  @Input() set startDate(value: DateTime | undefined) {
    this.datePickerOptions.startDate = value?.toJSDate();
    this.datePicker?.datepicker('setStartDate', value?.toJSDate());
  }

  @Input() set endDate(value: DateTime | undefined) {
    this.datePickerOptions.endDate = value?.toJSDate();
    this.datePicker?.datepicker('setEndDate', value?.toJSDate());
  }

  @Input() set format(value: string | undefined) {
    this.dateFormat = value ?? this.localizationService.dateTime.dateFormat;
    this.hasCustomDateFormat = !!value;
  }

  @Input() set minViewMode(value: ViewMode) {
    this.datePickerOptions = {...this.datePickerOptions, minViewMode: value};
  }

  @Input() set clearBtn(value: boolean) {
    this.datePickerOptions = {...this.datePickerOptions, clearBtn: value};
  }

  private dateFormat: string;
  private hasCustomDateFormat = false;
  private value?: DateTime = DateTime.min();
  // private originTimePart: DateObjectUnits = {hour: 0, minute: 0, second: 0, millisecond: 0};
  private disabled = false;
  private datepickerShown = false;
  private datePicker: any;
  private datePickerOptions: DatePickerOptions;

  constructor(
    private elementRef: ElementRef,
    private localizationService: LocalizationService,
    private dateParserService: DateParserService,
  ) {
    this.dateFormat = localizationService.dateTime.dateFormat;

    // noinspection SpellCheckingInspection
    this.datePickerOptions = {
      format: this.toDatePickerFormat(this.dateFormat),
      todayHighlight: true,
      calendarWeeks: true,
      weekStart: 1,
      autoclose: true,
      language: this.localizationService.language,
      todayBtn: 'linked',
      clearBtn: false,
      showOnFocus: false,
      assumeNearbyYear: true,
      forceParse: false,
      keyboardNavigation: false,
      minViewMode: 'days',
      startDate: undefined,
      endDate: undefined
    };
  }

  @HostListener('click')
  public onClick(): void {
    this.datePicker?.datepicker('show');
  }

  @HostListener('blur', ['$event.target.value'])
  public onBlur(rawInput: string | undefined) {
    if (this.datepickerShown || this.hasCustomDateFormat)
      return;

    let parsedDate = this.dateParserService.parseDate(rawInput, this.relativeAnchor);
    if (parsedDate?.isValid) {
      parsedDate = this.adjustNewValueToStartEndRange(parsedDate);
      this.datePicker?.datepicker('setDate', parsedDate.toJSDate());
      this.elementRef.nativeElement.value = parsedDate.toFormat(this.dateFormat);
      this.emitValue(parsedDate);
    } else {
      this.elementRef.nativeElement.value = '';
      this.emitValue(undefined);
    }
  }

  public ngAfterViewInit(): void {
    this.datePicker = $(this.elementRef.nativeElement) as any;
    this.datePicker
      .datepicker(this.datePickerOptions)
      .on('show', () => this.datepickerShown = true)
      .on('hide', () => {
        this.datePicker.focus();
        this.datepickerShown = false;
      })
      .on('changeDate', (event: any) => {
        if (event.date === undefined) {
          this.elementRef.nativeElement.value = '';
          this.emitValue(undefined);
          return;
        }

        let parsedDate = DateTime.fromJSDate(event.date);

        switch (this.datePickerOptions.minViewMode) {
          case 'days':
            parsedDate = this.relativeAnchor === 'start' ? parsedDate.startOf('day') : parsedDate.endOf('day');
            break;
          case 'months':
            parsedDate = this.relativeAnchor === 'start' ? parsedDate.startOf('month') : parsedDate.endOf('month');
            break;
          case 'years':
            parsedDate = this.relativeAnchor === 'start' ? parsedDate.startOf('year') : parsedDate.endOf('year');
            break;
        }

        this.elementRef.nativeElement.value = parsedDate.toFormat(this.dateFormat);
        this.emitValue(parsedDate);
      });
    this.updateDatepickerValue();
  }

  public writeValue(newValue?: DateTime): void {
    if (newValue?.isValid)
      newValue = this.adjustNewValueToStartEndRange(newValue);
    this.value = newValue;
    // this.originTimePart = newValue?.isValid
    //   ? {hour: newValue.hour, minute: newValue.minute, second: newValue.second, millisecond: newValue.millisecond}
    //   : {hour: 0, minute: 0, second: 0, millisecond: 0};
    this.updateDatepickerValue();
  }

  public emitValue(newValue?: DateTime): void {
    // if (newValue?.isValid)
    //   newValue = newValue?.set(this.originTimePart);

    if ((!this.value && !newValue) || (newValue && this.value?.equals(newValue)))
      return;

    this.value = newValue;
    this.onChange(this.value);
    this.onTouched();
  }

  public registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  public setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  public ngOnDestroy(): void {
    this.datePicker?.datepicker('destroy');
  }

  private toDatePickerFormat(format: string): string {
    // see https://moment.github.io/luxon/docs/manual/formatting.html
    // see https://bootstrap-datepicker.readthedocs.io/en/stable/options.html#format
    return format
      .replace(/EEEE/g, 'DD')
      .replace(/cccc/g, 'DD')
      .replace(/MMMM/g, 'MM')
      .replace(/EEE/g, 'D')
      .replace(/ccc/g, 'D')
      .replace(/MMM/g, 'M')
      .replace(/MM/g, 'mm')
      .replace(/M/g, 'm');
  }

  private adjustNewValueToStartEndRange(value: DateTime): DateTime {
    const dateTImeInvalid = DateTime.invalid('no value given');
    const startDate = this.datePickerOptions.startDate ? DateTime.fromJSDate(this.datePickerOptions.startDate) : dateTImeInvalid;

    if (value && startDate.isValid && value < startDate)
      value = value.set({year: startDate.year, month: startDate.month, day: startDate.day});

    const endDate = this.datePickerOptions.endDate ? DateTime.fromJSDate(this.datePickerOptions.endDate) : dateTImeInvalid;
    if (value && endDate.isValid && value > endDate)
      value = value.set({year: endDate.year, month: endDate.month, day: endDate.day});

    return value;
  }

  private updateDatepickerValue() {
    this.datePicker?.datepicker('update', this.value?.toJSDate());
    this.elementRef.nativeElement.value = this.value?.toFormat(this.dateFormat) ?? '';
  }

  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};
}

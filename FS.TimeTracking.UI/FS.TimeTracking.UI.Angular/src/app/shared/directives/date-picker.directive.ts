import {AfterViewInit, Directive, ElementRef, forwardRef, HostListener, Input, OnDestroy} from '@angular/core';
import {LocalizationService} from '../services/internationalization/localization.service';
import {DateTime} from 'luxon';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';
import {UtilityService} from '../services/utility.service';
import {DateObjectUnits} from 'luxon/src/datetime';

const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DatePickerDirective),
  multi: true,
};

@Directive({
  selector: '[tsDatePicker]',
  providers: [CUSTOM_VALUE_ACCESSOR],
})
export class DatePickerDirective implements AfterViewInit, OnDestroy, ControlValueAccessor {
  private readonly format: string;
  private value?: DateTime = DateTime.min();
  private originTimePart: DateObjectUnits = {hour: 0, minute: 0, second: 0, millisecond: 0};
  private disabled = false;
  private datepickerShown = false;
  private datePicker: any;
  private datePickerOptions: any = {};

  @Input() set startDate(value: DateTime | undefined) {
    this.datePickerOptions.startDate = value?.toJSDate();
    this.datePicker?.datepicker('setStartDate', value?.toJSDate());
  }

  @Input() set endDate(value: DateTime | undefined) {
    this.datePickerOptions.endDate = value?.toJSDate();
    this.datePicker?.datepicker('setEndDate', value?.toJSDate());
  }

  constructor(
    private elementRef: ElementRef,
    private localizationService: LocalizationService,
    private utilityService: UtilityService,
  ) {
    this.format = localizationService.dateTime.dateFormat;

    // noinspection SpellCheckingInspection
    this.datePickerOptions = {
      format: this.toDatePickerFormat(this.format),
      todayHighlight: true,
      calendarWeeks: true,
      weekStart: 1,
      autoclose: true,
      language: this.localizationService.language,
      todayBtn: 'linked',
      showOnFocus: false,
      assumeNearbyYear: true,
      forceParse: false,
      keyboardNavigation: false
    };
  }

  @HostListener('click')
  public onClick(): void {
    this.datePicker?.datepicker('show');
  }

  @HostListener('blur', ['$event.target.value'])
  public onBlur(rawInput: string | undefined) {
    if (this.datepickerShown)
      return;

    let parsedDate = this.utilityService.parseDate(rawInput);
    if (parsedDate?.isValid) {
      parsedDate = this.adjustToStartEndRange(parsedDate);
      this.datePicker?.datepicker('setDate', parsedDate.toJSDate());
      this.elementRef.nativeElement.value = parsedDate.toFormat(this.format);
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
        const parsedDate = DateTime.fromJSDate(event.date);
        this.elementRef.nativeElement.value = parsedDate.toFormat(this.format);
        this.emitValue(parsedDate);
      });
  }

  public writeValue(newValue?: DateTime): void {
    if (newValue?.isValid)
      newValue = this.adjustToStartEndRange(newValue);
    this.value = newValue;
    this.originTimePart = newValue?.isValid
      ? {hour: newValue.hour, minute: newValue.minute, second: newValue.second, millisecond: newValue.millisecond}
      : {hour: 0, minute: 0, second: 0, millisecond: 0};
    this.datePicker?.datepicker('update', this.value?.toJSDate());
    this.elementRef.nativeElement.value = this.value?.toFormat(this.format) ?? '';
  }

  public emitValue(newValue?: DateTime): void {
    if (newValue?.isValid)
      newValue = newValue?.set(this.originTimePart);

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

  private adjustToStartEndRange(value: DateTime): DateTime {
    const startDate = DateTime.fromJSDate(this.datePickerOptions.startDate);
    if (value && startDate.isValid && value < startDate)
      value = value.set({year: startDate.year, month: startDate.month, day: startDate.day});

    const endDate = DateTime.fromJSDate(this.datePickerOptions.endDate);
    if (value && endDate.isValid && value > endDate)
      value = value.set({year: endDate.year, month: endDate.month, day: endDate.day});

    return value;
  }

  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};
}

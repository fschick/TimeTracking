import {AfterViewInit, Component, forwardRef, OnDestroy} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';
import {DateTime} from 'luxon';
import {LocalizationService} from '../../services/internationalization/localization.service';
import {GuidService} from '../../services/state-management/guid.service';

const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => BootstrapDatepickerComponent),
  multi: true,
};

@Component({
  selector: 'ts-bootstrap-datepicker',
  templateUrl: './bootstrap-datepicker.component.html',
  styleUrls: ['./bootstrap-datepicker.component.scss'],
  providers: [CUSTOM_VALUE_ACCESSOR],
})
export class BootstrapDatepickerComponent implements AfterViewInit, OnDestroy, ControlValueAccessor {
  public readonly id: string;

  private readonly format: string;
  private value?: DateTime = DateTime.min();
  private disabled = false;
  private datePicker: any;
  private datePickerOptions = {};

  constructor(
    private localizationService: LocalizationService,
    guidService: GuidService,
  ) {
    this.id = guidService.newGuid();
    this.format = localizationService.dateTime.dateFormat;

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
    };
  }

  public ngAfterViewInit(): void {
    this.datePicker = $(`#${this.id}`) as any;
    this.datePicker
      .datepicker(this.datePickerOptions)
      .on('hide', () => this.datePicker.focus());
  }

  public showDatePicker(): void {
    this.datePicker?.datepicker('show');
  }

  public onBlur() {
    const date = this.datePicker?.datepicker('getDate');
    const dateTime = DateTime.fromJSDate(date);
    this.emitValue(dateTime.isValid ? dateTime : undefined);
  }

  public emitValue(value?: DateTime): void {
    this.value = value;
    this.onChange(this.value);
    this.onTouched();
  }

  public writeValue(obj: DateTime): void {
    this.value = obj;
    this.datePicker?.datepicker('update', this.value?.toJSDate());
  }

  public get formattedValue(): string {
    return this.value?.toFormat(this.format) ?? '';
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

  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};

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
}

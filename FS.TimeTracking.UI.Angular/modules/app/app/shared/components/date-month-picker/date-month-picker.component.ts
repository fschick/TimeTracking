import {Component, forwardRef, Input} from '@angular/core';
import {DateTime} from 'luxon';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';

const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DateMonthPickerComponent),
  multi: true,
};

@Component({
  selector: 'ts-date-month-picker',
  templateUrl: './date-month-picker.component.html',
  styleUrls: ['./date-month-picker.component.scss'],
  providers: [CUSTOM_VALUE_ACCESSOR],
})
export class DateMonthPickerComponent implements ControlValueAccessor {

  @Input() public placeholderMonth: string | undefined;
  @Input() public placeholderDate: string | undefined;
  /* Should partial inputs adjusted to start or end of period (2000-12 => 2000-12-01 or 2000-12-31) */
  @Input() public relativeAnchor: 'start' | 'end' = 'start';
  @Input() public format: string | undefined;
  @Input() public startDate: DateTime | undefined;
  @Input() public endDate: DateTime | undefined;
  @Input() public clearBtn: boolean = false;

  public value?: DateTime = DateTime.min();

  public get isEmpty() {return this.value == null};

  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};
  private disabled = false;

  constructor() {
  }

  public writeValue(newValue?: DateTime): void {
    this.value = newValue;
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

  public valueChanged($event: DateTime | undefined) {
    this.value = $event;
    this.onChange(this.value);
    this.onTouched();
  }
}

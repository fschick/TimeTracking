import {Directive, ElementRef, forwardRef, HostListener} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';
import {DateTime} from 'luxon';
import {DateObjectUnits} from 'luxon/src/datetime';

const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => TimeDirective),
  multi: true,
};

@Directive({
  selector: '[tsTime]',
  providers: [CUSTOM_VALUE_ACCESSOR],
})
export class TimeDirective implements ControlValueAccessor {
  private disabled = false;
  private value?: DateTime = DateTime.min();
  private originTimePart: DateObjectUnits;

  constructor(
    private elementRef: ElementRef,
  ) {
    const now = DateTime.now();
    this.originTimePart = {year: now.year, month: now.month, day: now.day};
  }

  @HostListener('blur', ['$event.target.value'])
  public onBlur(rawInput: string | undefined) {
    const timeFormat = /^(?<hour>\d\d):(?<minute>\d\d)$/;
    const timeSpan = rawInput?.match(timeFormat);
    if (!this.value || !timeSpan || !timeSpan.groups)
      return;

    const newValue = this.value.set({hour: parseInt(timeSpan.groups['hour'], 10), minute: parseInt(timeSpan.groups['minute'], 10)});
    this.emitValue(newValue);
  }

  writeValue(newValue?: DateTime): void {
    this.value = newValue;
    if (newValue?.isValid) {
      const hour = newValue?.hour.toString().padStart(2, '0');
      const minute = newValue?.minute.toString().padStart(2, '0');
      this.elementRef.nativeElement.value = `${hour}:${minute}`;
    } else {
      this.elementRef.nativeElement.value = '';
    }
  }

  public emitValue(newValue?: DateTime): void {
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

  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};
}

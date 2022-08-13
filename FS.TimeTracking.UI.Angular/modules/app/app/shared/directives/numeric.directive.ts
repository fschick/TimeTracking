import {Directive, ElementRef, forwardRef, HostListener, Input, Optional} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';
import {LocalizationService} from '../services/internationalization/localization.service';
import {UtilityService} from '../services/utility.service';
import {FormatService} from '../services/format.service';

const CUSTOM_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => NumericDirective),
  multi: true,
};

export interface NumericOptions {
  fractionDigits?: number;
}

@Directive({
  selector: '[tsNumeric]',
  providers: [CUSTOM_VALUE_ACCESSOR],
})
export class NumericDirective implements ControlValueAccessor {
  @Input() @Optional() tsNumeric?: NumericOptions;

  private inputElement: HTMLInputElement;
  private readonly digitGroupingSymbol: string;
  private readonly nonModifyingTail: RegExp;
  private previousRawInput?: string = undefined;
  private disabled = false;

  constructor(
    private elementRef: ElementRef,
    private utilityService: UtilityService,
    private formatService: FormatService,
    localizationService: LocalizationService,
  ) {
    this.inputElement = this.elementRef.nativeElement;
    this.digitGroupingSymbol = localizationService.numbers.digitGroupingSymbol;
    const decimalSymbol = utilityService.escapeRegex(localizationService.numbers.decimalSymbol);
    this.nonModifyingTail = new RegExp(`(?:(${decimalSymbol}0*)$|${decimalSymbol}\\d*?(0+)$)`);
  }

  @HostListener('input', ['$event'])
  public onInput($event: any) {
    let rawInput = $event.target.value as (string | undefined);

    const valueChanged = this.previousRawInput !== rawInput;
    if (!valueChanged)
      return;

    if (rawInput === undefined || this.previousRawInput === undefined) {
      const newValue2 = this.utilityService.parseNumber(rawInput);
      const newRawInput2 = this.formatService.formatNumber(newValue2);
      this.setRawInput(newRawInput2);
      this.emitValue(newValue2);
      return;
    }

    let cursorPosition = this.inputElement.selectionStart ?? -1;
    const cursorIsBehindInput = cursorPosition === rawInput.length;

    const digitGroupingSymbolRemoved = this.utilityService.isNumericallyEqual(this.previousRawInput, rawInput);
    if (digitGroupingSymbolRemoved) {
      const removedDigitPosition = $event.inputType === 'deleteContentForward' ? cursorPosition : cursorPosition - 1;
      rawInput = this.utilityService.replaceAt(rawInput, removedDigitPosition, this.digitGroupingSymbol);
    }

    const newValue = this.utilityService.parseNumber(rawInput);
    let newRawInput = this.formatService.formatNumber(newValue);

    const nonModifyingTail = rawInput.match(this.nonModifyingTail);
    if (nonModifyingTail != null)
      newRawInput += nonModifyingTail[1] ?? nonModifyingTail[2];

    this.setRawInput(newRawInput);
    this.emitValue(newValue);

    const previousCursorPos = rawInput.substr(0, cursorPosition).replace(this.utilityService.digitGroupingChars, '').length;
    // https://stackoverflow.com/a/28439031/1271211
    const nthDigitPosition = newRawInput.match(new RegExp(`(?:[^${this.utilityService.digitCharPattern}]*[${this.utilityService.digitCharPattern}]){${previousCursorPos}}`));
    if (nthDigitPosition != null)
      cursorPosition = nthDigitPosition[0].length;

    if (!cursorIsBehindInput)
      this.inputElement.setSelectionRange(cursorPosition, cursorPosition);
  }

  @HostListener('blur', ['$event.target.value'])
  public onBlur(rawInput: string | undefined) {
    const fractionDigits = this.tsNumeric?.fractionDigits;
    const newValue = this.utilityService.parseNumber(rawInput, fractionDigits);

    const options = fractionDigits ? {minimumFractionDigits: fractionDigits, maximumFractionDigits: fractionDigits} : undefined;
    const newRawInput = this.formatService.formatNumber(newValue, options);

    if (newRawInput !== rawInput) {
      this.setRawInput(newRawInput);
      this.emitValue(newValue);
    }
  }

  public writeValue(value: number | undefined): void {
    const fractionDigits = this.tsNumeric?.fractionDigits;
    const options = fractionDigits ? {minimumFractionDigits: fractionDigits, maximumFractionDigits: fractionDigits} : undefined;
    const newRawInput = this.formatService.formatNumber(value, options);
    this.setRawInput(newRawInput);
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

  private setRawInput(rawInput: string) {
    this.previousRawInput = rawInput;
    this.inputElement.value = rawInput;
  }

  private emitValue(value?: number): void {
    this.onChange(value);
    this.onTouched();
  }

  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};
}

import {Component, Input, OnInit, Self} from '@angular/core';
import {ControlValueAccessor, NgControl} from '@angular/forms';

declare type FormHooks = 'change' | 'blur' | 'submit';

@Component({
  selector: 'ts-clearable-input',
  templateUrl: './clearable-input.component.html',
  styleUrls: ['./clearable-input.component.scss'],
})
export class ClearableInputComponent implements OnInit, ControlValueAccessor {
  @Input() placeholder?: string;

  public value?: any;
  public updateOn?: FormHooks;

  public get isEmpty() {return this.value == null};

  private disabled = false;
  private onChange: (obj: any) => void = (_: any) => {};
  private onTouched: () => void = () => {};

  constructor(
    @Self() public ngControl: NgControl
  ) {
    ngControl.valueAccessor = this;
  }

  ngOnInit(): void {
    this.updateOn = this.ngControl.control?.updateOn;
  }

  public valueChanged($event: any) {
    this.value = $event;
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

  public writeValue(newValue?: any): void {
    this.value = newValue;
  }
}

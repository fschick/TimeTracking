import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {BehaviorSubject, interval} from 'rxjs';
import {debounce, debounceTime} from 'rxjs/operators';

@Component({
  selector: 'ts-confirm-button',
  templateUrl: './confirm-button.component.html',
  styleUrls: ['./confirm-button.component.scss']
})
export class ConfirmButtonComponent implements OnInit {
  @Input() color: 'primary' | 'secondary' | 'success' | 'info' | 'warning' | 'danger' | 'light' | 'dark' = 'secondary';
  @Input() disabled = false;
  @Input() debounceTime = 1000;
  @Output() confirmed = new EventEmitter<MouseEvent>();

  public confirm$ = new BehaviorSubject<boolean>(false);
  public confirmEnabled$ = this.confirm$.pipe(debounceTime(this.debounceTime));

  constructor() { }

  public ngOnInit(): void {
  }

  public switchConfirm(enabled: boolean): void {
    this.confirm$.next(enabled);
  }

  public onConfirmed($event: MouseEvent) {
    this.switchConfirm(false);
    this.confirmed.emit($event);
  }
}

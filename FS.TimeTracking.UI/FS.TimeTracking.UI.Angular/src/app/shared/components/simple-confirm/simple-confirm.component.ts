import {Component, ElementRef, EventEmitter, Input, OnDestroy, Output, ViewChild} from '@angular/core';
import {Modal} from 'bootstrap';

@Component({
  selector: 'ts-simple-confirm',
  templateUrl: './simple-confirm.component.html',
  styleUrls: ['./simple-confirm.component.scss']
})
export class SimpleConfirmComponent implements OnDestroy {
  @Input() public color: 'primary' | 'secondary' | 'success' | 'info' | 'warning' | 'danger' | 'light' | 'dark' = 'secondary';
  @Input() public disabled = false;
  @Input() public title = '';
  @Input() public message = '';
  @Input() public actionIcon = '';
  @Input() public actionText = '';

  @Output() public confirmed = new EventEmitter<MouseEvent>();
  @ViewChild('confirmDialog') private confirmDialog?: ElementRef;
  @ViewChild('confirmSubmit') private confirmSubmit?: ElementRef;

  private modal!: Modal;

  public onConfirmed($event: MouseEvent) {
    this.hideConfirmDialog();
    this.confirmed.emit($event);
  }

  public createConfirmDialog() {
    this.modal = new bootstrap.Modal(this.confirmDialog?.nativeElement);
    this.confirmDialog?.nativeElement.addEventListener('shown.bs.modal', () => this.confirmSubmit?.nativeElement.focus());
    this.modal.show();
  }

  public hideConfirmDialog() {
    this.modal?.hide();
  }

  public ngOnDestroy(): void {
    this.hideConfirmDialog();
  }
}

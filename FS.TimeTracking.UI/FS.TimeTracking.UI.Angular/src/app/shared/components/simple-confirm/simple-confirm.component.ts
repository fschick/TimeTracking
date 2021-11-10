import {Component, EventEmitter, Input, Output, ElementRef, EmbeddedViewRef, OnDestroy, TemplateRef, ViewChild, ViewContainerRef} from '@angular/core';
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
  @Input() public actionText = '';

  @Output() public confirmed = new EventEmitter<MouseEvent>();

  @ViewChild('confirmDialog') private confirmDialog?: TemplateRef<any>;

  private modalDialog!: Modal;
  private modalDialogRef?: EmbeddedViewRef<any>;

  constructor(
    private host: ViewContainerRef,
  ) {
  }

  public onConfirmed($event: MouseEvent) {
    this.hideConfirmDialog();
    this.confirmed.emit($event);
  }

  public createConfirmDialog() {
    this.modalDialogRef = this.host.createEmbeddedView(this.confirmDialog!);
    const modalDialogElement = this.modalDialogRef.rootNodes[0];
    this.modalDialog = new bootstrap.Modal(modalDialogElement);
    modalDialogElement.addEventListener('shown.bs.modal', () => modalDialogElement.querySelector('#confirmSubmit').focus());
    this.modalDialog.show();
  }

  public hideConfirmDialog() {
    this.modalDialog?.hide();
    this.modalDialogRef?.destroy();
  }

  public ngOnDestroy(): void {
    this.hideConfirmDialog();
  }
}

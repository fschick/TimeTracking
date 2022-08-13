import {Component, EventEmitter, Input, Output, ElementRef, EmbeddedViewRef, OnDestroy, TemplateRef, ViewChild, ViewContainerRef} from '@angular/core';
import {Modal} from 'bootstrap';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {single} from 'rxjs/operators';

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

  @Output() public opened = new EventEmitter();
  @Output() public closed = new EventEmitter();
  @Output() public confirmed = new EventEmitter<MouseEvent>();

  private modal?: NgbModalRef

  constructor(
    private host: ViewContainerRef,
    private modalService: NgbModal
  ) {
  }

  public onOpened() {
    this.opened.emit();
  }

  public onClosed() {
    this.closed.emit();
  }

  public onConfirmed($event: MouseEvent) {
    this.hideConfirmDialog();
    this.confirmed.emit($event);
  }

  public createConfirmDialog(confirmDialog: TemplateRef<any>) {
    this.modal = this.modalService.open(confirmDialog, {size: 'lg', scrollable: true, centered: true});
    this.onOpened();
  }

  public hideConfirmDialog() {
    this.modal?.close();
    this.onClosed();
  }

  public ngOnDestroy(): void {
    this.hideConfirmDialog();
  }
}

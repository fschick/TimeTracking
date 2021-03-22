import {Component, EventEmitter, Input, OnDestroy, Output, TemplateRef, ViewChild} from '@angular/core';
import {DialogRef, DialogService} from '@ngneat/dialog';
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
  @Input() public actionIcon = '';
  @Input() public actionText = '';
  @Output() public confirmed = new EventEmitter<MouseEvent>();
  @ViewChild('confirmDialog') private confirmDialogRef?: TemplateRef<any>;

  private confirmDialog?: DialogRef<unknown, any, TemplateRef<any>>;

  constructor(private dialogService: DialogService) { }

  public onConfirmed($event: MouseEvent) {
    this.destroyConfirmDialog();
    this.confirmed.emit($event);
  }

  public createConfirmDialog() {
    if (this.confirmDialogRef) {
      this.confirmDialog = this.dialogService.open(this.confirmDialogRef, {draggable: true, size: 'inherit', windowClass: 'center dialog-lg'});
      this.confirmDialog.afterClosed$.pipe(single()).subscribe(_ => this.confirmDialog = undefined);
    }
  }

  public destroyConfirmDialog() {
    if (this.confirmDialog) {
      this.confirmDialog.close();
      this.confirmDialog = undefined;
    }
  }

  public ngOnDestroy(): void {
    this.destroyConfirmDialog();
  }
}

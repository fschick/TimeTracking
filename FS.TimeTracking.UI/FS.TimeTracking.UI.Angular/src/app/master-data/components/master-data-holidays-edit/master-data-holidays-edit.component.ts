import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {HolidayDto, HolidayService} from '../../../shared/services/api';
import {ActivatedRoute, Router} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {Modal} from 'bootstrap';
import {GuidService} from '../../../shared/services/state-management/guid.service';

@Component({
  selector: 'ts-master-data-holidays-edit',
  templateUrl: './master-data-holidays-edit.component.html',
  styleUrls: ['./master-data-holidays-edit.component.scss']
})
export class MasterDataHolidaysEditComponent implements AfterViewInit {
  @ViewChild('holidayEdit') private holidayEdit?: ElementRef;
  @ViewChild('title') private title?: ElementRef;

  public holidayForm: ValidationFormGroup;
  public isNewRecord: boolean;
  private modal!: Modal;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private holidayService: HolidayService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
  ) {
    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    this.holidayForm = this.formValidationService.getFormGroup<HolidayDto>('HolidayDto', {id: GuidService.guidEmpty, type: 'Holiday'});

    if (!this.isNewRecord)
      this.holidayService
        .get({id: this.route.snapshot.params['id']})
        .pipe(single())
        .subscribe(holiday => this.holidayForm.patchValue(holiday));
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.holidayEdit?.nativeElement);
    this.holidayEdit?.nativeElement.addEventListener('hide.bs.modal', () => this.router.navigate(['..'], {relativeTo: this.route}));
    this.holidayEdit?.nativeElement.addEventListener('shown.bs.modal', () => this.title?.nativeElement.focus());
    this.modal.show();
  }

  public save(): void {
    if (!this.holidayForm.valid)
      return;

    const apiAction = this.isNewRecord
      ? this.holidayService.create({holidayDto: this.holidayForm.value})
      : this.holidayService.update({holidayDto: this.holidayForm.value});

    const holidayChangedAction = this.isNewRecord
      ? 'created'
      : 'updated';

    apiAction
      .pipe(single())
      .subscribe(holiday => {
        this.close();
        this.entityService.holidayChanged.next({entity: holiday, action: holidayChangedAction});
      });
  }

  public close(): void {
    this.modal?.hide();
  }
}

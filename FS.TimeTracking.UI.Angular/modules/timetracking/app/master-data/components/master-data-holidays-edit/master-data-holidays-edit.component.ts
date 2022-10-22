import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
import {HolidayDto, HolidayService} from '../../../../../api/timetracking';
import {ActivatedRoute, Router} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'ts-master-data-holidays-edit',
  templateUrl: './master-data-holidays-edit.component.html',
  styleUrls: ['./master-data-holidays-edit.component.scss']
})
export class MasterDataHolidaysEditComponent implements AfterViewInit {
  public holidayForm: ValidationFormGroup;
  public isNewRecord: boolean;

  @ViewChild('holidayEdit') private holidayEdit?: TemplateRef<any>;

  private modal?: NgbModalRef

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private holidayService: HolidayService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private modalService: NgbModal
  ) {
    this.holidayForm = this.formValidationService.getFormGroup<HolidayDto>('HolidayDto', {id: GuidService.guidEmpty, type: 'holiday'});

    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;
    if (this.isNewRecord)
      return;

    this.holidayService
      .get({id: this.route.snapshot.params['id']})
      .pipe(single())
      .subscribe(holiday => this.holidayForm.patchValue(holiday));
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.holidayEdit, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
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
        this.modal?.close();
        this.entityService.holidayChanged.next({entity: holiday, action: holidayChangedAction});
      });
  }
}

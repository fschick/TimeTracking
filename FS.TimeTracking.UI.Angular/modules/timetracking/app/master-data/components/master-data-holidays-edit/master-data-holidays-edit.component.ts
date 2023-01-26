import {AfterViewInit, Component, TemplateRef, ViewChild} from '@angular/core';
import {GuidStringTypeaheadDto, HolidayDto, HolidayService, TypeaheadService} from '../../../../../api/timetracking';
import {ActivatedRoute, Router} from '@angular/router';
import {single} from 'rxjs/operators';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {EntityService} from '../../../../../core/app/services/state-management/entity.service';
import {GuidService} from '../../../../../core/app/services/state-management/guid.service';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {forkJoin, of} from 'rxjs';
import {ConfigurationService} from '../../../../../core/app/services/configuration.service';
import {AuthenticationService} from '../../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-master-data-holidays-edit',
  templateUrl: './master-data-holidays-edit.component.html',
  styleUrls: ['./master-data-holidays-edit.component.scss']
})
export class MasterDataHolidaysEditComponent implements AfterViewInit {
  public holidayForm: ValidationFormGroup;
  public isNewRecord: boolean;
  public authorizationEnabled: boolean;
  public allUsers: GuidStringTypeaheadDto[] = [];

  @ViewChild('holidayEdit') private holidayEdit?: TemplateRef<any>;

  private modal?: NgbModalRef

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private holidayService: HolidayService,
    private typeaheadService: TypeaheadService,
    private configurationService: ConfigurationService,
    private entityService: EntityService,
    private formValidationService: FormValidationService,
    private authenticationService: AuthenticationService,
    private modalService: NgbModal,
  ) {
    this.authorizationEnabled = this.configurationService.clientConfiguration.features.authorization;
    this.isNewRecord = this.route.snapshot.params['id'] === GuidService.guidEmpty;

    this.holidayForm = this.formValidationService
      .getFormGroup<HolidayDto>(
        'HolidayDto',
        {
          id: GuidService.guidEmpty,
          type: 'holiday',
          userId: this.authorizationEnabled ? this.authenticationService.currentUser?.id : GuidService.guidEmpty,
        }
      );

    const getHolidays = !this.isNewRecord ? holidayService.get({id: this.route.snapshot.params['id']}) : of(undefined);
    const getUsers = this.authorizationEnabled ? typeaheadService.getUsers({}) : of([]);

    forkJoin([getHolidays, getUsers])
      .subscribe(([holiday, users]) => {
        this.allUsers = users;

        if (holiday)
          this.holidayForm.patchValue(holiday);
      })
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

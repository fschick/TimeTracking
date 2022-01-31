import {AfterViewInit, Component, ElementRef, TemplateRef, ViewChild} from '@angular/core';
import {ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Modal} from 'bootstrap';
import {ActivatedRoute, Router} from '@angular/router';
import {HolidayService} from '../../../shared/services/api';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {FormBuilder, FormControl, Validators} from '@angular/forms';
import {HttpClient, HttpParams} from '@angular/common/http';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'ts-master-data-holidays-import',
  templateUrl: './master-data-holidays-import.component.html',
  styleUrls: ['./master-data-holidays-import.component.scss']
})
export class MasterDataHolidaysImportComponent implements AfterViewInit {
  public holidayForm: ValidationFormGroup;

  @ViewChild('holidayImport') private holidayImport?: TemplateRef<any>;

  private modal?: NgbModalRef
  private icsImportFile?: File;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private holidayService: HolidayService,
    private httpClient: HttpClient,
    private entityService: EntityService,
    private modalService: NgbModal
  ) {

    this.holidayForm = new ValidationFormGroup('HolidayImportDto', {
      file: new FormControl('', Validators.required),
      type: new FormControl('Holiday', Validators.required),
    });
  }

  public ngAfterViewInit(): void {
    this.modal = this.modalService.open(this.holidayImport, {size: 'lg', scrollable: true});
    this.modal.hidden.pipe(single()).subscribe(() => this.router.navigate(['..'], {relativeTo: this.route}));
  }

  handleFileInput(event$: Event | null) {
    this.icsImportFile = (event$?.target as HTMLInputElement).files?.[0];
  }

  public save(): void {
    if (!this.holidayForm.valid || !this.icsImportFile)
      return;

    const formData: FormData = new FormData();
    formData.append('file', this.icsImportFile, this.icsImportFile.name);

    let queryParams = new HttpParams({encoder: this.holidayService.encoder})
      .append('type', this.holidayForm.value.type);

    this.httpClient
      .post(
        `${this.holidayService.configuration.basePath}/api/v1/Holiday/Import`,
        formData,
        {
          params: queryParams,
          responseType: 'json',
          withCredentials: this.holidayService.configuration.withCredentials,
          headers: this.holidayService.defaultHeaders,
          observe: undefined,
          reportProgress: undefined
        }
      )
      .pipe(single())
      .subscribe(() => {
        this.modal?.close();
        this.entityService.holidayChanged.next({action: 'reloadAll'});
      });
  }
}

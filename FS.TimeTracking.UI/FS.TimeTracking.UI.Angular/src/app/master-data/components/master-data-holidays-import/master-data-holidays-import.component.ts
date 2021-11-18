import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {ValidationFormGroup} from '../../../shared/services/form-validation/form-validation.service';
import {Modal} from 'bootstrap';
import {ActivatedRoute, Router} from '@angular/router';
import {HolidayService} from '../../../shared/services/api';
import {EntityService} from '../../../shared/services/state-management/entity.service';
import {single} from 'rxjs/operators';
import {FormBuilder, FormControl, Validators} from '@angular/forms';
import {HttpClient, HttpParams} from '@angular/common/http';

@Component({
  selector: 'ts-master-data-holidays-import',
  templateUrl: './master-data-holidays-import.component.html',
  styleUrls: ['./master-data-holidays-import.component.scss']
})
export class MasterDataHolidaysImportComponent implements AfterViewInit {
  @ViewChild('holidayImport') private holidayImport?: ElementRef;
  @ViewChild('title') private title?: ElementRef;

  public holidayForm: ValidationFormGroup;
  private modal!: Modal;
  private icsImportFile?: File;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private holidayService: HolidayService,
    private httpClient: HttpClient,
    private entityService: EntityService,
  ) {

    this.holidayForm = new ValidationFormGroup('HolidayImportDto', {
      file: new FormControl('', Validators.required),
      type: new FormControl('Holiday', Validators.required),
    });
  }

  public ngAfterViewInit(): void {
    this.modal = new bootstrap.Modal(this.holidayImport?.nativeElement);
    this.holidayImport?.nativeElement.addEventListener('hide.bs.modal', () => this.router.navigate(['..'], {relativeTo: this.route}));
    this.holidayImport?.nativeElement.addEventListener('shown.bs.modal', () => this.title?.nativeElement.focus());
    this.modal.show();
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
        this.close();
        this.entityService.holidayChanged.next({action: 'reloadAll'});
      });
  }

  public close(): void {
    this.modal?.hide();
  }
}

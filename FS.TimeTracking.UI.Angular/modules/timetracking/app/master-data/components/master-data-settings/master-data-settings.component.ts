import {Component} from '@angular/core';
import {map, single} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {SettingDto, SettingService} from '../../../../../api/timetracking';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {FormControl, Validators} from '@angular/forms';
import {UtilityService} from '../../../../../core/app/services/utility.service';
import {DomSanitizer, SafeUrl} from '@angular/platform-browser';
import {Observable} from 'rxjs';

interface SettingDtoWithDate extends SettingDto {
  workHoursPerWorkdayDate: DateTime
}

@Component({
  selector: 'ts-master-data-settings',
  templateUrl: './master-data-settings.component.html',
  styleUrls: ['./master-data-settings.component.scss']
})
export class MasterDataSettingsComponent {

  public settingsForm: ValidationFormGroup;
  public logoImageSrc$: Observable<SafeUrl | undefined>;

  constructor(
    private settingService: SettingService,
    private formValidationService: FormValidationService,
    private utilityService: UtilityService,
    private sanitizer: DomSanitizer,
  ) {
    this.settingsForm = this.createSettingsForm();

    this.logoImageSrc$ = this.settingsForm.valueChanges.pipe(map(value => {
      return this.getImageUrl(value.company.logo);
    }));

    this.settingService.getSettings()
      .pipe(single())
      .subscribe(settingsDto => {
        this.settingsForm.patchValue(this.toSettingDtoWithDate(settingsDto));
      });
  }

  public save(): void {
    if (!this.settingsForm.valid)
      return;

    const settingDto = this.toSettingDto(this.settingsForm.value);
    this.settingService.updateSettings({settingDto}).pipe(single()).subscribe();
  }

  public setLogoFileInput(event$: Event | null) {
    const file = (event$?.target as HTMLInputElement).files?.[0];
    if (file == null)
      return;

    this.utilityService.getBase64EncodedFileData(file)
      .pipe(single())
      .subscribe(base64Image =>
        this.settingsForm.patchValue({company: {logo: base64Image}})
      );
  }

  public clearLogoFileInput() {
    this.settingsForm.patchValue({company: {logo: undefined}})
  }

  private createSettingsForm(): ValidationFormGroup {
    return this.formValidationService.getFormGroup<SettingDtoWithDate>(
      'SettingDto',
      {},
      {
        workHoursPerWorkdayDate: new FormControl(undefined, [Validators.required]),
      }
    );
  }

  private getImageUrl(base64Image: string | null | undefined): SafeUrl | undefined {
    return base64Image
      ? this.sanitizer.bypassSecurityTrustUrl(`data:image/png;base64,${base64Image}`)
      : undefined
  }

  private toSettingDtoWithDate(settings: SettingDto): SettingDtoWithDate {
    return {
      ...settings,
      workHoursPerWorkdayDate: this.toDateTime(settings.workHoursPerWorkday),
    };
  }

  private toSettingDto(settings: SettingDtoWithDate): SettingDto {
    return {
      ...settings,
      workHoursPerWorkday: this.toDuration(settings.workHoursPerWorkdayDate),
    };
  }

  private toDateTime(duration: Duration): DateTime {
    return DateTime.fromObject({
      hour: duration.hours,
      minute: duration.minutes
    })
  }

  private toDuration(dateTime: DateTime): Duration {
    return Duration.fromObject({
      hours: dateTime.hour,
      minutes: dateTime.minute
    });
  }
}

import {Component} from '@angular/core';
import {single} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {SettingDto, SettingService} from '../../../../../api/timetracking';
import {FormValidationService, ValidationFormGroup} from '../../../../../core/app/services/form-validation/form-validation.service';
import {FormControl, Validators} from '@angular/forms';

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

  constructor(
    private settingService: SettingService,
    private formValidationService: FormValidationService,
  ) {
    this.settingsForm = this.createSettingsForm();

    this.settingService.getSettings().pipe(single())
      .subscribe(settingsDto => this.settingsForm.patchValue(this.toSettingDtoWithDate(settingsDto)));
  }

  public save(): void {
    if (!this.settingsForm.valid)
      return;

    const settingDto = this.toSettingDto(this.settingsForm.value);
    this.settingService.updateSettings({settingDto}).pipe(single()).subscribe();
  }

  private createSettingsForm(): ValidationFormGroup {
    return this.formValidationService.getFormGroup<SettingDtoWithDate>(
      'SettingDto',
      {},
      {
        workHoursPerWorkdayDate: new FormControl(undefined, [Validators.required])
      }
    );
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

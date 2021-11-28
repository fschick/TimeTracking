import {Component} from '@angular/core';
import {single} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {SettingDto, SettingDtoWorkdays, SettingService} from '../../../shared/services/api';

interface Settings {
  workdays: SettingDtoWorkdays;
  workHoursPerWorkday: DateTime;
  currency: string;
}

@Component({
  selector: 'ts-master-data-settings',
  templateUrl: './master-data-settings.component.html',
  styleUrls: ['./master-data-settings.component.scss']
})
export class MasterDataSettingsComponent {

  public settingsForm: FormGroup;

  constructor(
    private settingService: SettingService,
    private formBuilder: FormBuilder,
  ) {
    this.settingsForm = this.createSettingsForm();

    this.settingService.get().pipe(single())
      .subscribe(settingsDto => {
        const settings = this.convertTimeSpans(settingsDto);
        this.settingsForm.patchValue(settings)
      });
  }

  public save(): void {
    if (!this.settingsForm.valid)
      return;

    const settingDto: SettingDto = {
      workdays: this.settingsForm.value.workdays,
      workHoursPerWorkday: this.toDuration(this.settingsForm.value.workHoursPerWorkday),
      currency: this.settingsForm.value.currency,
    }

    this.settingService.update({settingDto}).pipe(single()).subscribe();
  }

  private createSettingsForm(): FormGroup {
    return this.formBuilder.group({
      workdays: this.formBuilder.group({
        Monday: [],
        Tuesday: [],
        Wednesday: [],
        Thursday: [],
        Friday: [],
        Saturday: [],
        Sunday: [],
      }),
      workHoursPerWorkday: [],
      currency: ['', Validators.required]
    });
  }

  private convertTimeSpans(settings: SettingDto): Settings {
    return {
      workdays: settings.workdays,
      workHoursPerWorkday: this.toDateTime(settings.workHoursPerWorkday),
      currency: settings.currency,
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

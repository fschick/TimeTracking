import {Component} from '@angular/core';
import {SettingDto, SettingDtoWorkingHours, SettingService} from '../../../shared/services/api';
import {Observable} from 'rxjs';
import {map, single} from 'rxjs/operators';
import {DateTime, Duration} from 'luxon';
import {FormBuilder, FormGroup} from '@angular/forms';

interface WorkingHours {
  monday: DateTime;
  tuesday: DateTime;
  wednesday: DateTime;
  thursday: DateTime;
  friday: DateTime;
  saturday: DateTime;
  sunday: DateTime;
}

@Component({
  selector: 'ts-master-data-settings',
  templateUrl: './master-data-settings.component.html',
  styleUrls: ['./master-data-settings.component.scss']
})
export class MasterDataSettingsComponent {

  public workingHoursForm: FormGroup;

  constructor(
    private settingService: SettingService,
    private formBuilder: FormBuilder
  ) {
    this.workingHoursForm = this.createWorkingHoursForm();

    this.settingService
      .get()
      .pipe(
        single(),
        map(settings => this.toWorkingHours(settings.workingHours))
      )
      .subscribe(workingHours => this.workingHoursForm.patchValue(workingHours));
  }

  public save(): void {
    if (!this.workingHoursForm.valid)
      return;

    const settingDto = {
      workingHours: {
        Monday: this.toDuration(this.workingHoursForm.value.monday),
        Tuesday: this.toDuration(this.workingHoursForm.value.tuesday),
        Wednesday: this.toDuration(this.workingHoursForm.value.wednesday),
        Thursday: this.toDuration(this.workingHoursForm.value.thursday),
        Friday: this.toDuration(this.workingHoursForm.value.friday),
        Saturday: this.toDuration(this.workingHoursForm.value.saturday),
        Sunday: this.toDuration(this.workingHoursForm.value.sunday),
      }
    }

    this.settingService.update({settingDto}).pipe(single()).subscribe();
  }

  private createWorkingHoursForm(): FormGroup {
    return this.formBuilder.group({
      monday: [],
      tuesday: [],
      wednesday: [],
      thursday: [],
      friday: [],
      saturday: [],
      sunday: [],
    });
  }

  private toWorkingHours(workingHours: SettingDtoWorkingHours): WorkingHours {
    return {
      monday: this.toDateTime(workingHours.Monday!),
      tuesday: this.toDateTime(workingHours.Tuesday!),
      wednesday: this.toDateTime(workingHours.Wednesday!),
      thursday: this.toDateTime(workingHours.Thursday!),
      friday: this.toDateTime(workingHours.Friday!),
      saturday: this.toDateTime(workingHours.Saturday!),
      sunday: this.toDateTime(workingHours.Sunday!),
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

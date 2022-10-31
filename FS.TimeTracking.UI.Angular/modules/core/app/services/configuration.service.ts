import {Injectable} from '@angular/core';
import {ClientConfigurationDto, InformationService} from '../../../api/timetracking';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {
  public clientConfiguration!: ClientConfigurationDto
}

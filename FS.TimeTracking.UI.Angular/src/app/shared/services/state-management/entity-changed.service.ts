import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';
import {CustomerDto} from '../api';

export interface EntityChanged<TDto> {
  action: 'created' | 'updated' | 'deleted';
  entity: TDto;
}

@Injectable({
  providedIn: 'root'
})
export class EntityChangedService {
  public customerChanged: Subject<EntityChanged<CustomerDto>> = new Subject<EntityChanged<CustomerDto>>();

  public updateCollection<TDto>(entities: TDto[], key: keyof TDto, changedEvent: EntityChanged<TDto>): TDto[] {
    switch (changedEvent?.action) {
      case 'created':
        entities.push(changedEvent.entity);
        break;
      case 'updated':
        const idxUpdated = entities.findIndex(x => x[key] === changedEvent.entity[key]);
        entities[idxUpdated] = changedEvent?.entity;
        break;
      case 'deleted':
        const idxDeleted = entities.findIndex(x => x[key] === changedEvent.entity[key]);
        entities.slice(idxDeleted, 1);
        break;
    }

    return entities;
  }
}

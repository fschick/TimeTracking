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
export class EntityService {
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

  // public showUpdateNote(dtoTransUnitId: string) {
  //   return <T>(source: Observable<HttpResponse<T>>) =>
  //     source.pipe(
  //       catchError(err => {
  //         const fieldName = '--fieldName--';
  //         const requiredLength = '--requiredLength--';
  //         const dtoName = $localizeId`${dtoTransUnitId}:TRANSUNITID:`;
  //         this.toastr.error($localize`:@@Common.DtoSaveError:[I18N] Error while saving ${dtoName}:DTONAME:`);
  //         return throwError(err);
  //       }),
  //       map(response => {
  //         const dtoName = $localizeId`${dtoTransUnitId}:TRANSUNITID:`;
  //         this.toastr.success($localize`:@@Common.DtoSaveSuccess:[I18N] ${dtoName}:DTONAME: saved`);
  //         return response.body ?? {} as T;
  //       }),
  //     );
  // }
}

import {Injectable} from '@angular/core';
import {Observable, of, Subject} from 'rxjs';
import {ActivityListDto, CustomerDto, ProjectListDto} from '../api';
import {map, single, switchMap} from 'rxjs/operators';

export interface EntityChanged<TDto> {
  entity: TDto;
  action: 'created' | 'updated' | 'deleted';
}

export type CrudDto = {
  id: string;
};

export type CrudService<TDto> = {
  list: (id: string) => Observable<TDto[]>;
};

@Injectable({
  providedIn: 'root'
})
export class EntityService {
  public guidEmpty = '00000000-0000-0000-0000-000000000000';

  public customerChanged: Subject<EntityChanged<CustomerDto>> = new Subject<EntityChanged<CustomerDto>>();
  public projectChanged: Subject<EntityChanged<ProjectListDto>> = new Subject<EntityChanged<ProjectListDto>>();
  public activityChanged: Subject<EntityChanged<ActivityListDto>> = new Subject<EntityChanged<ActivityListDto>>();

  public updateCollection<TDto>(entities: TDto[], key: keyof TDto, changedEvent: EntityChanged<TDto>): TDto[] {
    switch (changedEvent?.action) {
      case 'created':
        entities.push(changedEvent.entity);
        break;
      case 'updated':
        const idxUpdated = entities.findIndex(x => x[key] === changedEvent.entity[key]);
        if (idxUpdated >= 0)
          entities[idxUpdated] = changedEvent?.entity;
        break;
      case 'deleted':
        const idxDeleted = entities.findIndex(x => x[key] === changedEvent.entity[key]);
        if (idxDeleted >= 0)
          entities.splice(idxDeleted, 1);
        break;
    }

    return entities;
  }

  public replaceEntityWithOverviewDto<TDto extends CrudDto>(crudService: CrudService<TDto>) {
    return (source: Observable<EntityChanged<TDto>>) =>
      source.pipe(switchMap((changedEvent: EntityChanged<TDto>) => {
          if (changedEvent.action === 'deleted') {
            changedEvent.entity = {id: changedEvent.entity.id} as TDto;
            return of(changedEvent);
          }

          return crudService
            .list(changedEvent.entity.id)
            .pipe(single(), map(project => {
              changedEvent.entity = project[0];
              return changedEvent;
            }));
        }
      ));
  }

  // // See https://stackoverflow.com/a/2117523/1271211
  // public guidNewGuid(): string {
  //   // @ts-ignore
  //   return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
  //     // eslint-disable-next-line no-bitwise
  //     (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
  //   );
  // }

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

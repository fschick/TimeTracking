import {Injectable} from '@angular/core';
import {merge, Observable, of, Subject} from 'rxjs';
import {ActivityListDto, CustomerDto, HolidayDto, HolidayListDto, OrderListDto, ProjectListDto, TimeSheetListDto} from '../api';
import {filter, map, single, switchMap, tap} from 'rxjs/operators';

export interface EntityChanged<TDto> {
  entity?: TDto;
  action: 'created' | 'updated' | 'deleted' | 'reloadAll';
}

export type CrudDto = {
  id: string;
};

export type CrudService<TDto> = {
  list: (requestParameters: { id?: string }) => Observable<TDto[]>;
};

@Injectable({
  providedIn: 'root'
})
export class EntityService {
  public timesheetChanged: Subject<EntityChanged<TimeSheetListDto>> = new Subject<EntityChanged<TimeSheetListDto>>();
  public orderChanged: Subject<EntityChanged<OrderListDto>> = new Subject<EntityChanged<OrderListDto>>();
  public activityChanged: Subject<EntityChanged<ActivityListDto>> = new Subject<EntityChanged<ActivityListDto>>();
  public projectChanged: Subject<EntityChanged<ProjectListDto>> = new Subject<EntityChanged<ProjectListDto>>();
  public customerChanged: Subject<EntityChanged<CustomerDto>> = new Subject<EntityChanged<CustomerDto>>();
  public holidayChanged: Subject<EntityChanged<HolidayListDto>> = new Subject<EntityChanged<HolidayListDto>>();

  public withUpdatesFrom<TDto extends CrudDto>(entityChanged: Observable<EntityChanged<TDto>>, crudService: CrudService<TDto>) {
    return (sourceList: Observable<TDto[]>) => {
      let cachedSourceList: TDto[] = [];

      const cachedSourceList$ = sourceList
        .pipe(tap(list => cachedSourceList = list));

      const updatedSourceList$ = entityChanged
        .pipe(
          filter(x => x.action !== 'reloadAll'),
          this.replaceEntityWithListDto(crudService),
          map(changedEvent => {
            const updatedDtos = this.updateCollection(cachedSourceList, 'id', changedEvent);
            cachedSourceList = [...updatedDtos];
            return cachedSourceList;
          }));

      const reloadedSourceList$ = entityChanged.pipe(
        filter(x => x.action === 'reloadAll'),
        switchMap(() => crudService.list({}).pipe(single()))
      );

      return merge(cachedSourceList$, updatedSourceList$, reloadedSourceList$);
    };
  }

  private replaceEntityWithListDto<TDto extends CrudDto>(crudService: CrudService<TDto>) {
    return (source: Observable<EntityChanged<TDto>>) =>
      source.pipe(
        switchMap((changedEvent: EntityChanged<TDto>) => {
          if (changedEvent.entity === undefined)
            throw Error("No entity given");

          if (changedEvent.action === 'deleted') {
            changedEvent.entity = {id: changedEvent.entity.id} as TDto;
            return of(changedEvent);
          }

          return crudService
            .list({id: changedEvent.entity.id})
            .pipe(
              single(),
              map(entity => {
                changedEvent.entity = entity[0];
                return changedEvent;
              })
            );
        })
      );
  }

  private updateCollection<TDto>(entities: TDto[], key: keyof TDto, changedEvent: EntityChanged<TDto>): TDto[] {
    if (!changedEvent.entity)
      throw Error("No entity given");

    const changedEntity = changedEvent.entity;
    switch (changedEvent?.action) {
      case 'created':
        entities.unshift(changedEntity);
        break;
      case 'updated':
        const idxUpdated = entities.findIndex(x => x[key] === changedEntity[key]);
        if (idxUpdated >= 0)
          entities[idxUpdated] = changedEvent?.entity;
        break;
      case 'deleted':
        const idxDeleted = entities.findIndex(x => x[key] === changedEntity[key]);
        if (idxDeleted >= 0)
          entities.splice(idxDeleted, 1);
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
  //         this.toastr.error($localize`:@@Common.DtoSaveError:[i18n] Error while saving ${dtoName}:DTONAME:`);
  //         return throwError(err);
  //       }),
  //       map(response => {
  //         const dtoName = $localizeId`${dtoTransUnitId}:TRANSUNITID:`;
  //         this.toastr.success($localize`:@@Common.DtoSaveSuccess:[i18n] ${dtoName}:DTONAME: saved`);
  //         return response.body ?? {} as T;
  //       }),
  //     );
  // }
}

import {Injectable} from '@angular/core';
import {merge, mergeMap, Observable, of, Subject} from 'rxjs';
import {ActivityGridDto, CustomerGridDto, HolidayGridDto, OrderGridDto, ProjectGridDto, TimeSheetGridDto} from '../../../../api/timetracking';
import {filter, map, single, switchMap, tap} from 'rxjs/operators';
import {FilteredRequestParams, FilterName} from '../../components/filter/filter.component';

export interface EntityChanged<TDto> {
  entity?: TDto;
  action: 'created' | 'updated' | 'deleted' | 'reloadAll';
}

export type CrudDto = {
  id: string;
};

export type CrudService<TDto> = {
  getGridItem: (requestParameters: { id: string }) => Observable<TDto>;
  getGridFiltered: (requestParameters: {}) => Observable<TDto[]>;
};

@Injectable({
  providedIn: 'root'
})
export class EntityService {
  public filterChanged = new Subject<FilteredRequestParams>();
  public filterValuesChanged = new Subject<Record<FilterName, any>>();
  public timesheetChanged = new Subject<EntityChanged<TimeSheetGridDto>>();
  public orderChanged = new Subject<EntityChanged<OrderGridDto>>();
  public activityChanged = new Subject<EntityChanged<ActivityGridDto>>();
  public projectChanged = new Subject<EntityChanged<ProjectGridDto>>();
  public customerChanged = new Subject<EntityChanged<CustomerGridDto>>();
  public holidayChanged = new Subject<EntityChanged<HolidayGridDto>>();

  public withUpdatesFrom<TDto extends CrudDto>(entityChanged: Observable<EntityChanged<TDto>>, crudService: CrudService<TDto>) {
    return (gridData: Observable<TDto[]>) => {
      let cachedGridData: TDto[] = [];

      const cachedGridData$ = gridData
        .pipe(tap(data => cachedGridData = data));

      const updatedGridData$ = entityChanged
        .pipe(
          filter(x => x.action !== 'reloadAll'),
          this.replaceEntityWithGridDto(crudService),
          map(changedEvent => {
            const updatedDtos = this.updateCollection(cachedGridData, 'id', changedEvent);
            cachedGridData = [...updatedDtos];
            return cachedGridData;
          }));

      const reloadedGridData$ = entityChanged.pipe(
        filter(x => x.action === 'reloadAll'),
        switchMap(() => crudService.getGridFiltered({}).pipe(single()))
      );

      return merge(cachedGridData$, updatedGridData$, reloadedGridData$);
    };
  }

  private replaceEntityWithGridDto<TDto extends CrudDto>(crudService: CrudService<TDto>) {
    return (source: Observable<EntityChanged<TDto>>) =>
      source.pipe(
        mergeMap((changedEvent: EntityChanged<TDto>) => {
          if (changedEvent.entity === undefined)
            throw Error("No entity given");

          if (changedEvent.action === 'deleted') {
            changedEvent.entity = {id: changedEvent.entity.id} as TDto;
            return of(changedEvent);
          }

          return crudService
            .getGridItem({id: changedEvent.entity.id})
            .pipe(
              single(),
              map(entity => {
                changedEvent.entity = entity;
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

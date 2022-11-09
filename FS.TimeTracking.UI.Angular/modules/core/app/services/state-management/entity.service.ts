import {Injectable} from '@angular/core';
import {combineLatest, EMPTY, fromEvent, interval, merge, mergeMap, Observable, of, Subject} from 'rxjs';
import {ActivityGridDto, CustomerGridDto, HolidayGridDto, OrderGridDto, ProjectGridDto, TimeSheetGridDto, UserGridDto} from '../../../../api/timetracking';
import {filter, map, single, startWith, switchMap, takeUntil, takeWhile, tap, throttleTime} from 'rxjs/operators';
import {FilteredRequestParams, FilterName} from '../../components/filter/filter.component';
import {environment} from '../../../../timetracking/environments/environment';

export interface EntityChanged<TDto> {
  entity?: TDto;
  action: 'created' | 'updated' | 'deleted';
}

export type CrudDto = {
  id: string;
};

export type CrudService<TDto> = {
  getGridItem: (requestParameters: { id: string }) => Observable<TDto>;
  getGridFiltered: (requestParameters: {}) => Observable<TDto[]>;
};

@Injectable()
export class EntityService {
  public readonly reloadRequested: Observable<FilteredRequestParams>;
  public readonly filterChanged = new Subject<FilteredRequestParams>();
  public readonly filterValuesChanged = new Subject<Record<FilterName, any>>();
  public readonly timesheetChanged = new Subject<EntityChanged<TimeSheetGridDto>>();
  public readonly orderChanged = new Subject<EntityChanged<OrderGridDto>>();
  public readonly activityChanged = new Subject<EntityChanged<ActivityGridDto>>();
  public readonly projectChanged = new Subject<EntityChanged<ProjectGridDto>>();
  public readonly customerChanged = new Subject<EntityChanged<CustomerGridDto>>();
  public readonly holidayChanged = new Subject<EntityChanged<HolidayGridDto>>();
  public readonly holidaysImported = new Subject<void>();
  public readonly userChanged = new Subject<EntityChanged<UserGridDto>>();

  constructor() {
    this.reloadRequested = this.createReloadRequested();
  }

  public withUpdatesFrom<TDto extends CrudDto>(entityChanged: Observable<EntityChanged<TDto>>, crudService: CrudService<TDto>) {
    return (gridData: Observable<TDto[]>) => {
      let cachedGridData: TDto[] = [];

      const cachedGridData$ = gridData
        .pipe(tap(data => cachedGridData = data));

      const updatedGridData$ = entityChanged
        .pipe(
          this.replaceEntityWithGridDto(crudService),
          map(changedEvent => {
            const updatedDtos = this.updateCollection(cachedGridData, 'id', changedEvent);
            cachedGridData = [...updatedDtos];
            return cachedGridData;
          }));

      return merge(updatedGridData$, cachedGridData$);
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

  private createReloadRequested() {
    const filterChanged$ = this.filterChanged;

    const windowDisplayed$ = fromEvent(window.document, 'visibilitychange').pipe(filter(() => document.visibilityState === 'visible'));
    const windowFocused$ = fromEvent(window, 'focus');

    const windowActivated$ = merge(windowDisplayed$, windowFocused$).pipe(throttleTime(100), startWith(0));
    const windowDeactivated$ = fromEvent(window, 'blur');

    const fiveMinutes = 5 * 60 * 1000;
    const pingWhileVisibleButInactive$ = interval(fiveMinutes).pipe(takeWhile(() => !window.document.hidden), takeUntil(windowFocused$));
    const pollWhileVisibleButInactive$ = merge(windowDeactivated$, windowDisplayed$).pipe(switchMap(() => pingWhileVisibleButInactive$), startWith(0));

    const reloadTrigger = environment.production
      ? combineLatest([filterChanged$, windowActivated$, pollWhileVisibleButInactive$])
      : combineLatest([filterChanged$, of(0), of(0)]);

    return reloadTrigger.pipe(map(([filter]) => filter));
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

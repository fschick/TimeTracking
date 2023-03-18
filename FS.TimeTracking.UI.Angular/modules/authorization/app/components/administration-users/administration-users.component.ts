import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Column, Configuration, DataCellTemplate, SimpleTableComponent} from '../../../../core/app/components/simple-table/simple-table.component';
import {UserGridDto, UserService} from '../../../../api/timetracking';
import {GuidService} from '../../../../core/app/services/state-management/guid.service';
import {Filter, FilteredRequestParams, FilterName} from '../../../../core/app/components/filter/filter.component';
import {Observable, Subscription} from 'rxjs';
import {EntityService} from '../../../../core/app/services/state-management/entity.service';
import {ActivatedRoute, Router} from '@angular/router';
import {LocalizationService} from '../../../../core/app/services/internationalization/localization.service';
import {single, switchMap} from 'rxjs/operators';
import {AuthenticationService} from '../../../../core/app/services/authentication.service';

@Component({
  selector: 'ts-administration-users',
  templateUrl: './administration-users.component.html',
  styleUrls: ['./administration-users.component.scss']
})
export class AdministrationUsersComponent implements OnInit, OnDestroy {
  @ViewChild(SimpleTableComponent) private customerTable?: SimpleTableComponent<UserGridDto>;
  @ViewChild('dataCellTemplate', {static: true}) private dataCellTemplate?: DataCellTemplate<UserGridDto>;
  @ViewChild('actionCellTemplate', {static: true}) private actionCellTemplate?: DataCellTemplate<UserGridDto>;

  public guidService = GuidService;
  public rows?: UserGridDto[];
  public columns!: Column<UserGridDto>[];
  public configuration?: Partial<Configuration<UserGridDto>>;
  public filters: (Filter | FilterName)[];
  public createNewDisabled: boolean;

  private readonly subscriptions = new Subscription();

  constructor(
    private entityService: EntityService,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    private localizationService: LocalizationService,
    private authenticationService: AuthenticationService,
  ) {
    this.createNewDisabled = !authenticationService.currentUser.hasRole.administrationUsersManage || !authenticationService.currentUser.hasRole.foreignDataManage;

    this.filters = [
      {name: 'userId', showHidden: true, isPrimary: true},
      {name: 'userFirstName', isPrimary: true},
      {name: 'userLastName', isPrimary: true},
      {name: 'userEmail', isPrimary: true},
    ];
  }

  public ngOnInit(): void {
    const filterChanged = this.entityService.reloadRequested
      .pipe(
        switchMap(filter => this.loadData(filter)),
        this.entityService.withUpdatesFrom(this.entityService.userChanged, this.userService),
      )
      .subscribe(rows => this.rows = rows);
    this.subscriptions.add(filterChanged);

    this.configuration = {
      cssWrapper: 'table-responsive',
      cssTable: 'table',
      glyphSortAsc: '',
      glyphSortDesc: '',
      locale: this.localizationService.language,
    };

    const cssHeadCell = 'border-0 text-nowrap';
    const cssHeadCellMd = 'd-none d-md-table-cell';
    const cssDataCellMd = cssHeadCellMd;
    this.columns = [
      {
        title: $localize`:@@DTO.UserDto.Username:[i18n] Username`,
        prop: 'username',
        cssHeadCell: cssHeadCell,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.UserDto.FirstName:[i18n] First name`,
        prop: 'firstName',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.UserDto.LastName:[i18n] Last name`,
        prop: 'lastName',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: $localize`:@@DTO.UserDto.Email:[i18n] Email`,
        prop: 'email',
        cssHeadCell: `${cssHeadCell} ${cssHeadCellMd}`,
        cssDataCell: cssDataCellMd,
        dataCellTemplate: this.dataCellTemplate
      }, {
        title: '',
        customId: 'delete',
        dataCellTemplate: this.actionCellTemplate,
        cssHeadCell: cssHeadCell,
        cssDataCell: 'text-nowrap action-cell',
        width: '1px',
        sortable: false
      },
    ];
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public isCurrentUser(userDto: UserGridDto) {
    return userDto.id === this.authenticationService.currentUser.id;
  }

  public deleteItem(id: string): void {
    this.userService
      .delete({id})
      .pipe(single())
      .subscribe(() => {
        this.entityService.userChanged.next({entity: {id} as UserGridDto, action: 'deleted'});
      });
  }

  private loadData(filter: FilteredRequestParams): Observable<UserGridDto[]> {
    return this.userService.getGridFiltered(filter)
      .pipe(single());
  }

}

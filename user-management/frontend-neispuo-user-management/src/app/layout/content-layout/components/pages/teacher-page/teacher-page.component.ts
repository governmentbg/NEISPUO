import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth.query';
import { ImpersonationQuery } from '@core/impersonation/impersonation.query';
import { ImpersonationService } from '@core/impersonation/impersonation.service';
import { ApiService } from '@core/services/api.service';
import { TranslateService } from '@ngx-translate/core';
import { SyncAzureUserDTO } from '@shared/business-object-model/requests/sync-azure-user.dto';
import { UserManagementResponse } from '@shared/business-object-model/responses/user-management-response';
import { HasAzureIDEnum } from '@shared/enums/has-azure-id.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { UserService } from '@shared/services/user.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { TeacherTableColumnsConfig } from 'src/app/configs/datatables-config';
import { TeacherResponseDTO } from '@shared/business-object-model/responses/teacher-response.dto';
import { CONSTANTS } from 'src/app/shared/constants';
import { SubSink } from 'subsink';
import { combineLatest } from 'rxjs';
import { distinctUntilChanged, filter, map, switchMap } from 'rxjs/operators';
import { PreventButtonClickService } from '@shared/services/prevent-button-click.service';
import { TeacherUsersService } from '@shared/services/teacher-users.service';
import { PersonService } from '@shared/services/person.service';
import { RoleEnum } from '@shared/enums/roles.enum';
import { EnvironmentService } from '@core/services/environment.service';

@Component({
    selector: 'app-teacher-page',
    templateUrl: './teacher-page.component.html',
    styleUrls: ['./teacher-page.component.scss'],
})
export class TeacherPageComponent
    extends PaginatedTableList<TeacherResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('teachersTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    lastTableLazyLoadEvent!: LazyLoadEvent;

    columnConfig: ColumnServiceConfig<TeacherResponseDTO> = TeacherTableColumnsConfig;

    hasAccess$ = combineLatest([
        this.authQuery.isMon$,
        this.authQuery.isInstitution$,
        this.authQuery.isRuo$,
        this.authQuery.isCIOO$,
    ]).pipe(
        map(([isMon, isInstitution, isRuo, isCIOO]) => isMon || isInstitution || isRuo || isCIOO),
        distinctUntilChanged(),
    );

    public readonly canImpersonateTeacherStudent$ = this.impersonationQuery.canImpersonateTeacherStudent$;

    public readonly isImpersonator$ = this.authQuery.isImpersonator$;

    public readonly isMon$ = this.authQuery.isMon$;

    private readonly environment = this.envService.environment;

    public readonly isImpersonationEnabled = this.environment.IS_IMPERSONATION_ENABLED;

    public readonly isSchoolBookAccessEnabled = this.environment.IS_SCHOOL_BOOK_ACCESS_ENABLED;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<TeacherResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private readonly authQuery: AuthQuery,
        private userService: UserService,
        private personService: PersonService,
        private teacherService: TeacherUsersService,
        private preventButtonClickService: PreventButtonClickService,
        private impersonationService: ImpersonationService,
        private impersonationQuery: ImpersonationQuery,
        private messageService: MessageService,
        private envService: EnvironmentService,
    ) {
        super(router, route, dotNotatedPipe, columnService, translateService);
        this.columnService.setConfig(this.columnConfig);
    }

    ngOnInit(): void {
        this.columnService.reloadVisibleColumns();
    }

    ngAfterViewInit() {
        this.loadByUrl();
    }

    ngOnDestroy() {
        this.subSink.unsubscribe();
    }

    public load(event: LazyLoadEvent) {
        this.loading = true;
        this.lastTableLazyLoadEvent = event;
        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/teacher-users', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<TeacherResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    updateUser(userID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHERS}/${CONSTANTS.ROUTER_PATH_TEACHERS_UPDATE}/${userID}`,
        );
    }

    updateRoles(user: TeacherResponseDTO) {
        sessionStorage.setItem(CONSTANTS.SESSION_STORAGE_ROLE_UPDATE_INSTITUTION_ID, JSON.stringify(user));
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHERS}/${CONSTANTS.ROUTER_PATH_ROLES_UPDATE}`,
        );
    }

    updateSchoolBookAccess(user: TeacherResponseDTO) {
        sessionStorage.setItem(CONSTANTS.SESSION_STORAGE_SCHOOL_BOOK_ACCESS, JSON.stringify(user));
        this.userService.getSysUserIDByPersonID(user.personID!).subscribe((data) => {
            this.router.navigateByUrl(
                `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHERS}/${user.institutionID}/${user.personID}/${CONSTANTS.ROUTER_PATH_SCHOOL_BOOKS_ACCESS_UPDATE}`,
            );
        });
    }

    async syncAzureTeacher(user: TeacherResponseDTO) {
        this.loading = true;
        if (this.preventButtonClickService.checkButton(user?.personID?.toString()!)) {
            this.personService.printFiveMinutesWarrningMessage();
            this.loading = false;
            return;
        }

        this.preventButtonClickService.lockButton(user?.personID?.toString()!);
        this.subSink.sink = this.teacherService
            .callSyncAzureTeacher(user)
            .subscribe(
                (success: UserManagementResponse<SyncAzureUserDTO>) => {
                    this.teacherService.printSuccessMessage();
                    this.tableRef.filter(user.personID, 'personID', 'equals');
                    this.tableRef.filter(HasAzureIDEnum.CHOOSE, 'hasAzureID', 'equals');
                },
                (error) => {
                    this.teacherService.printErrorMessage(error);
                },
            )
            .add(() => (this.loading = false));
    }

    enableSyncButton(dto: TeacherResponseDTO) {
        return this.teacherService.enableSyncButton(dto);
    }

    impersonate(personID: number) {
        this.userService
            .getSysUserIDByPersonID(personID)
            .pipe(
                map((data) => data.payload.username ?? ''),
                switchMap((username) => this.impersonationService.impersonate(username, RoleEnum.TEACHER)),
            )
            .subscribe({
                next: () => {
                    window.location.reload();
                },
                error: (err) => {
                    this.messageService.add({
                        summary: 'Грешка при имперсонация',
                        detail: 'Не беше намерен потребител с този идентификатор.',
                        severity: 'error',
                    });
                },
            });
    }

    refreshTableData() {
        this.load(this.lastTableLazyLoadEvent);
    }

    isUpdateRolesButtonDisabled(teacherDTO: TeacherResponseDTO) {
        if (teacherDTO.publicEduNumber) return false;
        return true;
    }

    isSchoolBookAccessButtonDisabled(teacherDTO: TeacherResponseDTO) {
        if (teacherDTO.publicEduNumber) return false;
        return true;
    }

    isImpersonationButtonDisabled(teacherDTO: TeacherResponseDTO) {
        if (teacherDTO.publicEduNumber) return false;
        return true;
    }

    routeToSyncHistory(personID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_TEACHERS}/${personID}/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
        );
    }
}

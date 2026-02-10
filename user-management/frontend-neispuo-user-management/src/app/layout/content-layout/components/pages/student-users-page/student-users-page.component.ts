import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth.query';
import { ImpersonationQuery } from '@core/impersonation/impersonation.query';
import { ImpersonationService } from '@core/impersonation/impersonation.service';
import { ApiService } from '@core/services/api.service';
import { TranslateService } from '@ngx-translate/core';
import { SyncAzureUserDTO } from '@shared/business-object-model/requests/sync-azure-user.dto';
import { StudentResponseDTO } from '@shared/business-object-model/responses/student-response.dto';
import { UserManagementResponse } from '@shared/business-object-model/responses/user-management-response';
import { HasAzureIDEnum } from '@shared/enums/has-azure-id.enum';
import { RoleEnum } from '@shared/enums/roles.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { ConfirmUpdateCodeDialogService } from '@shared/services/confirm-update-code-dialog.service';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { PersonService } from '@shared/services/person.service';
import { PreventButtonClickService } from '@shared/services/prevent-button-click.service';
import { ScrollService } from '@shared/services/scroll.service';
import { StudentUsersExportService } from '@shared/services/student-users-export.service';
import { StudentUsersService } from '@shared/services/student-users.service';
import { UserService } from '@shared/services/user.service';
import { cloneDeep } from 'lodash';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { combineLatest } from 'rxjs';
import { distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { StudentTableColumnsConfig } from 'src/app/configs/datatables-config';
import { CONSTANTS } from 'src/app/shared/constants';
import { SubSink } from 'subsink';
import { EnvironmentService } from '@core/services/environment.service';
import { GetManyDefaultResponse } from '../../../../../shared/models/get-many-default-response';

@Component({
    selector: 'app-student-users-page',
    templateUrl: './student-users-page.component.html',
    styleUrls: ['./student-users-page.component.scss'],
})
export class StudentUsersPageComponent
    extends PaginatedTableList<StudentResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('studentUsersTable') tableRef: Table = {} as Table;

    loading = true;

    exporting = false;

    subSink = new SubSink();

    lastTableLazyLoadEvent!: LazyLoadEvent;

    hasAccess$ = combineLatest([
        this.authQuery.isMon$,
        this.authQuery.isRuo$,
        this.authQuery.isInstitution$,
        this.authQuery.isCIOO$,
    ]).pipe(
        map(([isMon, isRUO, isInstitution, isCIOO]) => isMon || isRUO || isInstitution || isCIOO),
        distinctUntilChanged(),
    );

    columnConfig: ColumnServiceConfig<StudentResponseDTO> = StudentTableColumnsConfig;

    public readonly canImpersonateTeacherStudent$ = this.impersonationQuery.canImpersonateTeacherStudent$;

    public readonly isImpersonator$ = this.authQuery.isImpersonator$;

    public readonly isMon$ = this.authQuery.isMon$;

    private readonly environment = this.envService.environment;

    public readonly isImpersonationEnabled = this.environment.IS_IMPERSONATION_ENABLED;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<StudentResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private readonly authQuery: AuthQuery,
        private confirmUpdateCodeDialogService: ConfirmUpdateCodeDialogService,
        private studentUsersExportService: StudentUsersExportService,
        private studentService: StudentUsersService,
        private preventButtonClickService: PreventButtonClickService,
        private personService: PersonService,
        private userService: UserService,
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
            .get('/v1/student-users', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<StudentResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    updateSchoolBookCode(personID: number) {
        this.confirmUpdateCodeDialogService.updateSchoolBookCode(personID, this);
    }

    async syncAzureStudent(user: StudentResponseDTO) {
        this.loading = true;
        if (this.preventButtonClickService.checkButton(user?.personID?.toString()!)) {
            this.personService.printFiveMinutesWarrningMessage();
            this.loading = false;
            return;
        }
        this.preventButtonClickService.lockButton(user?.personID?.toString()!);
        this.subSink.sink = this.studentService
            .callSyncAzureStudent(user)
            .subscribe(
                (success: UserManagementResponse<SyncAzureUserDTO>) => {
                    this.studentService.printSuccessMessage();
                    this.tableRef.filter(user.personID, 'personID', 'equals');
                    this.tableRef.filter(HasAzureIDEnum.CHOOSE, 'hasAzureID', 'equals');
                },
                (error) => {
                    this.studentService.printErrorMessage(error);
                },
            )
            .add(() => (this.loading = false));
    }

    async convertAzureStudentToTeacher(user: StudentResponseDTO) {
        this.loading = true;
        this.subSink.sink = this.studentService
            .callCovertAzureStudentToTeacher(user)
            .subscribe(
                (success: UserManagementResponse<SyncAzureUserDTO>) => {
                    this.studentService.printSuccessMessage();
                },
                (error) => {
                    this.studentService.printErrorMessage(error);
                },
            )
            .add(() => (this.loading = false));
    }

    enableSyncButton(dto: StudentResponseDTO) {
        return this.studentService.enableSyncButton(dto);
    }

    enableCovertButton(dto: StudentResponseDTO) {
        return true;
    }

    updateUser(userID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_STUDENTS}/${CONSTANTS.ROUTER_PATH_STUDENTS_UPDATE}/${userID}`,
        );
    }

    exportExcel() {
        const event: LazyLoadEvent = cloneDeep(this.tableRef.createLazyLoadMetadata());
        const queryParams = this.createQueryParams({ ...event, first: 0 }, 9999999); // load un-paginated data, but with selected filters
        const getDataMethod = this.getToDisplayValue.bind(this);

        this.exporting = true;
        this.studentUsersExportService
            .createExcel(queryParams, getDataMethod)
            .subscribe()
            .add(() => {
                this.exporting = false;
            });
    }

    impersonate(personID: number) {
        this.userService
            .getSysUserIDByPersonID(personID)
            .pipe(
                map((data) => data.payload.username ?? ''),
                switchMap((username) => this.impersonationService.impersonate(username, RoleEnum.STUDENT)),
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

    routeToSyncHistory(personID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_STUDENTS}/${personID}/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
        );
    }

    routeToLinkedUsersPage(personID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_STUDENTS}/${personID}/${CONSTANTS.ROUTER_PATH_LINKED_USERS}`,
        );
    }
}

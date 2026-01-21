import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { TeacherClassesResponseDTO } from '@shared/business-object-model/responses/teacher-classes-response.dto';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ConfirmUpdateCodeDialogService } from '@shared/services/confirm-update-code-dialog.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { TeacherClassesTableColumnsConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { cloneDeep } from 'lodash';
import { LeadTeacherStudentsExportExcelService } from '@shared/services/lead-teacher-students-export-excel';
import { TranslateService } from '@ngx-translate/core';
import { SyncAzureUserDTO } from '@shared/business-object-model/requests/sync-azure-user.dto';
import { StudentResponseDTO } from '@shared/business-object-model/responses/student-response.dto';
import { UserManagementResponse } from '@shared/business-object-model/responses/user-management-response';
import { PersonService } from '@shared/services/person.service';
import { PreventButtonClickService } from '@shared/services/prevent-button-click.service';
import { StudentUsersService } from '@shared/services/student-users.service';

@Component({
    selector: 'app-teacher-class-page',
    templateUrl: './teacher-class-page.component.html',
    styleUrls: ['./teacher-class-page.component.scss'],
})
export class TeacherClassPageComponent
    extends PaginatedTableList<TeacherClassesResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('teacherClassesTable') tableRef: Table = {} as Table;

    @Input() userID!: number;

    @Input() institutionID!: number;

    loading = true;

    exporting = false;

    subSink = new SubSink();

    lastTableLazyLoadEvent!: LazyLoadEvent;

    columnConfig: ColumnServiceConfig<TeacherClassesResponseDTO> = TeacherClassesTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<TeacherClassesResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private confirmUpdateCodeDialogService: ConfirmUpdateCodeDialogService,
        private leadTeacherStudentsExportExcelService: LeadTeacherStudentsExportExcelService,
        private studentService: StudentUsersService,
        private preventButtonClickService: PreventButtonClickService,
        private personService: PersonService,
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
        this.scrollService.scrollTo('body');
        this.lastTableLazyLoadEvent = event;
        console.log(event);
        this.subSink.sink = this.apiService
            .get('/v1/lead-teacher-students', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<TeacherClassesResponseDTO>) => {
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

    exportExcel() {
        const event: LazyLoadEvent = cloneDeep(this.tableRef.createLazyLoadMetadata());
        const queryParams = this.createQueryParams({ ...event, first: 0 }, 9999999); // load un-paginated data, but with selected filters
        const getDataMethod = this.getToDisplayValue.bind(this);

        this.exporting = true;
        this.leadTeacherStudentsExportExcelService
            .createExcel(queryParams, getDataMethod)
            .subscribe()
            .add(() => {
                this.exporting = false;
            });
    }

    async syncAzureStudent(personID: number) {
        this.loading = true;
        if (this.preventButtonClickService.checkButton(personID?.toString()!)) {
            this.personService.printFiveMinutesWarrningMessage();
            this.loading = false;
            return;
        }
        this.preventButtonClickService.lockButton(personID?.toString()!);
        this.subSink.sink = this.studentService
            .callSyncAzureStudent({ personID })
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

    refreshTableData() {
        this.load(this.lastTableLazyLoadEvent);
    }
}

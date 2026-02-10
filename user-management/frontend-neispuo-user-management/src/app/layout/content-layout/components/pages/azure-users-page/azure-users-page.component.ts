import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { CONSTANTS } from '@shared/constants';
import { EventStatus } from '@shared/enums/event-status.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { AzureUsersService } from '@shared/services/azure-users.service';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { take } from 'rxjs/operators';
import { AzureUsersTableColumnsConfig } from 'src/app/configs/datatables-config';
import { AzureUsersResponseDTO } from '@shared/business-object-model/responses/azure-users-response.dto';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-azure-users-page',
    templateUrl: './azure-users-page.component.html',
    styleUrls: ['./azure-users-page.component.scss'],
})
export class AzureUsersPageComponent
    extends PaginatedTableList<AzureUsersResponseDTO>
    implements OnInit, AfterViewInit, OnDestroy
{
    @ViewChild('azureUsersTable') tableRef: Table = {} as Table;

    lastTableLazyLoadEvent!: LazyLoadEvent;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<AzureUsersResponseDTO> = AzureUsersTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<AzureUsersResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private messageService: MessageService,
        private azureUsersService: AzureUsersService,
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
        this.lastTableLazyLoadEvent = event;
        this.loading = true;
        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/azure-users', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<AzureUsersResponseDTO>) => {
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
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_AZURE_USERS}/${CONSTANTS.ROUTER_PATH_AZURE_USERS_UPDATE}/${userID}`,
        );
    }

    restartWorkflow(id: number) {
        this.azureUsersService
            .restartWorkflow(id)
            .pipe(take(1))
            .subscribe(
                (response) => {
                    this.ngOnInit();
                    this.messageService.add({
                        severity: 'success',
                        summary: this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_RESTART_WORKFLOW),
                    });
                    this.tableRef.filter('', 'status', 'contains');
                    this.tableRef.filter(id, 'rowID', 'contains');
                },
                (error) => {
                    this.messageService.add({
                        severity: 'error',
                        summary: this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_ERROR),
                        detail: error.message,
                    });
                    console.log('error fetching paginated data', error);
                },
            );
    }

    refreshTableData() {
        this.load(this.lastTableLazyLoadEvent);
    }

    isRestartWorkflowButtonEnabled(status: EventStatus, retryAttempts: number) {
        return this.azureUsersService.isRestartWorkflowButtonEnabled(status, retryAttempts);
    }
}

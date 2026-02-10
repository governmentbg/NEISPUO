import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { CONSTANTS } from '@shared/constants';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { AzureEnrollmentsTableColumnsConfig } from 'src/app/configs/datatables-config';
import { AzureEnrollmentsResponseDTO } from '@shared/business-object-model/responses/azure-enrollments-response.dto';
import { SubSink } from 'subsink';
import { AzureEnrollMentsService } from '@shared/services/azure-enrollments.service';
import { take } from 'rxjs/operators';
import { EventStatus } from '@shared/enums/event-status.enum';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-azure-enrollments-page',
    templateUrl: './azure-enrollments-page.component.html',
    styleUrls: ['./azure-enrollments-page.component.scss'],
})
export class AzureEnrollmentsPageComponent
    extends PaginatedTableList<AzureEnrollmentsResponseDTO>
    implements OnInit, AfterViewInit, OnDestroy
{
    @ViewChild('azureEnrollmentsTable') tableRef: Table = {} as Table;

    lastTableLazyLoadEvent!: LazyLoadEvent;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<AzureEnrollmentsResponseDTO> = AzureEnrollmentsTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<AzureEnrollmentsResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private messageService: MessageService,
        private azureEnrollmentsService: AzureEnrollMentsService,
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
            .get('/v1/azure-enrollments', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<AzureEnrollmentsResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    updateEnrollment(classID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_AZURE_ENROLLMENT}/${CONSTANTS.ROUTER_PATH_AZURE_ENROLLMENT_UPDATE}/${classID}`,
        );
    }

    restartWorkflow(id: number) {
        this.azureEnrollmentsService
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
        return this.azureEnrollmentsService.isRestartWorkflowButtonEnabled(status, retryAttempts);
    }
}

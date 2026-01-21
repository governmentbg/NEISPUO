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
import { AzureOrganizationsTableColumnsConfig } from 'src/app/configs/datatables-config';
import { AzureOrganizationResponseDTO } from '@shared/business-object-model/responses/azure-organizations-response.dto';
import { SubSink } from 'subsink';
import { AzureOrganizationsService } from '@shared/services/azure-organizations.service';
import { take } from 'rxjs/operators';
import { EventStatus } from '@shared/enums/event-status.enum';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-azure-organizations-page',
    templateUrl: './azure-organizations-page.component.html',
    styleUrls: ['./azure-organizations-page.component.scss'],
})
export class AzureOrganizationsPageComponent
    extends PaginatedTableList<AzureOrganizationResponseDTO>
    implements OnInit, AfterViewInit, OnDestroy
{
    @ViewChild('organizationsTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    lastTableLazyLoadEvent!: LazyLoadEvent;

    columnConfig: ColumnServiceConfig<AzureOrganizationResponseDTO> = AzureOrganizationsTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<AzureOrganizationResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private messageService: MessageService,
        private azureOrganizationsService: AzureOrganizationsService,
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
            .get('/v1/azure-organizations', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<AzureOrganizationResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    updateOrganization(organizationID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_AZURE_ORGANIZATIONS}/${CONSTANTS.ROUTER_PATH_AZURE_ORGANIZATIONST_UPDATE}/${organizationID}`,
        );
    }

    restartWorkflow(id: number) {
        this.azureOrganizationsService
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
        return this.azureOrganizationsService.isRestartWorkflowButtonEnabled(status, retryAttempts);
    }
}

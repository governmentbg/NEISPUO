import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { CONSTANTS } from '@shared/constants';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { AzureClassesService } from '@shared/services/azure-classes.service';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { AzureClassesTableColumnsConfig } from 'src/app/configs/datatables-config';
import { AzureClassesResponseDTO } from '@shared/business-object-model/responses/azure-classes-response.dto';
import { SubSink } from 'subsink';
import { take } from 'rxjs/operators';
import { EventStatus } from '@shared/enums/event-status.enum';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-azure-classes-page',
    templateUrl: './azure-classes-page.component.html',
    styleUrls: ['./azure-classes-page.component.scss'],
})
export class AzureClassesPageComponent
    extends PaginatedTableList<AzureClassesResponseDTO>
    implements OnInit, AfterViewInit, OnDestroy
{
    @ViewChild('azureClassesTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<AzureClassesResponseDTO> = AzureClassesTableColumnsConfig;

    lastTableLazyLoadEvent!: LazyLoadEvent;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<AzureClassesResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private messageService: MessageService,
        private azureClassesService: AzureClassesService,
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
            .get('/v1/azure-classes', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<AzureClassesResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    updateClass(classID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_AZURE_CLASSES}/${CONSTANTS.ROUTER_PATH_AZURE_CLASSES_UPDATE}/${classID}`,
        );
    }

    restartWorkflow(id: number) {
        this.azureClassesService
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
        return this.azureClassesService.isRestartWorkflowButtonEnabled(status, retryAttempts);
    }
}

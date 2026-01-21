import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { LoginAuditResponseDTO } from '@shared/business-object-model/responses/login-audit.response.dto';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { LoginAuditExportService } from '@shared/services/login-audit-export.service';
import { ScrollService } from '@shared/services/scroll.service';
import { cloneDeep } from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { LoginAuditTableConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-login-audit-page',
    templateUrl: './login-audit-page.component.html',
    styleUrls: ['./login-audit-page.component.scss'],
})
export class LoginAuditPageComponent
    extends PaginatedTableList<LoginAuditResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('loginAuditTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<LoginAuditResponseDTO> = LoginAuditTableConfig;

    exporting = false;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<LoginAuditResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private loginAuditExportService: LoginAuditExportService,
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

        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/login-audit', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<LoginAuditResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    exportExcel() {
        const event: LazyLoadEvent = cloneDeep(this.tableRef.createLazyLoadMetadata());
        const queryParams = this.createQueryParams({ ...event, first: 0 }, 9999999); // load un-paginated data, but with selected filters
        const getDataMethod = this.getToDisplayValue.bind(this);

        this.exporting = true;
        this.loginAuditExportService
            .createExcel(queryParams, getDataMethod)
            .subscribe()
            .add(() => {
                this.exporting = false;
            });
    }
}

import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { RoleAuditExportService } from '@shared/services/role-audit-export.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { RoleAuditTableConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { cloneDeep } from 'lodash';
import { CondOperator } from '@nestjsx/crud-request';
import { AuditActionEnum } from '@shared/enums/audit-action.enum';
import { TranslateService } from '@ngx-translate/core';
import { RoleAudit } from '../update-roles-page/role-audit/role-audit';

@Component({
    selector: 'app-role-audit-page',
    templateUrl: './role-audit-page.component.html',
    styleUrls: ['./role-audit-page.component.scss'],
})
export class RoleAuditPageComponent extends PaginatedTableList<RoleAudit> implements OnInit, OnDestroy, AfterViewInit {
    @ViewChild('roleAuditTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<RoleAudit> = RoleAuditTableConfig;

    exporting = false;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<RoleAudit>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private roleAuditExportService: RoleAuditExportService,
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
        event.filters = {
            ...event.filters,
            AuditModuleId: {
                value: 403, // 403 = USER_MANAGMENT module
                matchMode: CondOperator.EQUALS,
            },
            ObjectName: {
                value: 'SysUserSysRole',
                matchMode: CondOperator.EQUALS,
            },
        };

        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/role-assignment', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<RoleAudit>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    getModifiedEntry(roleAudit: RoleAudit) {
        return {
            ...roleAudit,
            Action: roleAudit.Action === AuditActionEnum.INSERT ? 'Добавена роля' : 'Отнета роля',
        };
    }

    exportExcel() {
        const event: LazyLoadEvent = cloneDeep(this.tableRef.createLazyLoadMetadata());
        const queryParams = this.createQueryParams(
            {
                ...event,
                filters: {
                    ...event.filters,
                    ObjectName: {
                        value: 'SysUserSysRole',
                        matchMode: CondOperator.EQUALS,
                    },
                    AuditModuleId: {
                        value: 403, // 403 = USER_MANAGMENT module
                        matchMode: CondOperator.EQUALS,
                    },
                },
                first: 0,
            },
            9999999,
        ); // load un-paginated data, but with selected filters
        const getDataMethod = this.getToDisplayValue.bind(this);

        this.exporting = true;
        this.roleAuditExportService
            .createExcel(queryParams, getDataMethod)
            .subscribe()
            .add(() => {
                this.exporting = false;
            });
    }
}

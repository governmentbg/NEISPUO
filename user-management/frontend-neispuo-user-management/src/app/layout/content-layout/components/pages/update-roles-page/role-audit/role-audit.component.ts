import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth.query';
import { ApiService } from '@core/services/api.service';
import { CondOperator } from '@nestjsx/crud-request';
import { TranslateService } from '@ngx-translate/core';
import { AuditActionEnum } from '@shared/enums/audit-action.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { RoleAuditExportService } from '@shared/services/role-audit-export.service';
import { ScrollService } from '@shared/services/scroll.service';
import { UserService } from '@shared/services/user.service';
import { cloneDeep } from 'lodash';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { RoleAuditTableConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { RoleAudit } from './role-audit';

@Component({
    selector: 'app-role-audit',
    templateUrl: './role-audit.component.html',
    styleUrls: ['./role-audit.component.scss'],
})
export class RoleAuditComponent extends PaginatedTableList<RoleAudit> implements OnInit, OnDestroy, AfterViewInit {
    @ViewChild('rolesAuditTable') tableRef: Table = {} as Table;

    @Input() personID!: number;

    @Input() institutionID!: number;

    sysUserID!: number;

    loading = true;

    subSink = new SubSink();

    isMon$ = this.authQuery.isMon$;

    exporting = false;

    columnConfig: ColumnServiceConfig<RoleAudit> = RoleAuditTableConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<RoleAudit>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private roleAuditExportService: RoleAuditExportService,
        private authQuery: AuthQuery,
        private userService: UserService,
    ) {
        super(router, route, dotNotatedPipe, columnService, translateService);
        this.columnService.setConfig(this.columnConfig);
    }

    ngOnInit(): void {
        this.columnService.reloadVisibleColumns();
    }

    ngAfterViewInit() {
        this.userService.getSysUserIDByPersonID(this.personID!).subscribe((data) => {
            this.sysUserID = data.payload.sysUserID!;
            this.loadByUrl();
        });
    }

    ngOnDestroy() {
        this.subSink.unsubscribe();
    }

    public load(event: LazyLoadEvent) {
        this.loading = true;
        event.filters = {
            ...event.filters,
            ObjectId: {
                value: this.sysUserID,
                matchMode: CondOperator.EQUALS,
            },
            ObjectName: {
                value: 'SysUserSysRole',
                matchMode: CondOperator.EQUALS,
            },
            AuditModuleId: {
                value: 403, // 403 = USER_MANAGMENT module
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
                    ObjectId: {
                        value: this.sysUserID,
                        matchMode: CondOperator.EQUALS,
                    },
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

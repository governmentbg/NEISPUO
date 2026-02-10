import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { RuoTableColumnsConfig } from 'src/app/configs/datatables-config';
import { RUOResponseDTO } from '@shared/business-object-model/responses/ruo-response.dto';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';
import { AuthQuery } from '@core/authentication/auth.query';
import { ImpersonationQuery } from '@core/impersonation/impersonation.query';
import { ImpersonationService } from '@core/impersonation/impersonation.service';
import { RoleEnum } from '@shared/enums/roles.enum';
import { EnvironmentService } from '@core/services/environment.service';

@Component({
    selector: 'app-ruo-page',
    templateUrl: './ruo-page.component.html',
    styleUrls: ['./ruo-page.component.scss'],
})
export class RuoPageComponent extends PaginatedTableList<RUOResponseDTO> implements OnInit, OnDestroy, AfterViewInit {
    @ViewChild('ruoTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<RUOResponseDTO> = RuoTableColumnsConfig;

    public readonly canImpersonateRUO$ = this.impersonationQuery.canImpersonateRUO$;

    public readonly isImpersonator$ = this.authQuery.isImpersonator$;

    private readonly environment = this.envService.environment;

    public readonly isImpersonationEnabled = this.environment.IS_IMPERSONATION_ENABLED;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<RUOResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private authQuery: AuthQuery,
        private impersonationQuery: ImpersonationQuery,
        private impersonationService: ImpersonationService,
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
        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/ruo', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<RUOResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    impersonate(username: string, role: number) {
        this.impersonationService.impersonate(username, role).subscribe({
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
}

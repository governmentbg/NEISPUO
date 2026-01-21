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
import { InstitutionTableColumnsConfig } from 'src/app/configs/datatables-config';
import { InstitutionResponseDTO } from '@shared/business-object-model/responses/institution-response.dto';
import { CONSTANTS } from 'src/app/shared/constants';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';
import { ImpersonationQuery } from '@core/impersonation/impersonation.query';
import { ImpersonationService } from '@core/impersonation/impersonation.service';
import { AuthQuery } from '@core/authentication/auth.query';
import { RoleEnum } from '@shared/enums/roles.enum';
import { EnvironmentService } from '@core/services/environment.service';

@Component({
    selector: 'app-institution-page',
    templateUrl: './institution-page.component.html',
    styleUrls: ['./institution-page.component.scss'],
})
export class InstitutionPageComponent
    extends PaginatedTableList<InstitutionResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('institutionsTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<InstitutionResponseDTO> = InstitutionTableColumnsConfig;

    public readonly canImpersonateInstitution$ = this.impersonationQuery.canImpersonateInstitution$;

    public readonly isImpersonator$ = this.authQuery.isImpersonator$;

    public readonly isMon$ = this.authQuery.isMon$;

    private readonly environment = this.envService.environment;

    public readonly isImpersonationEnabled = this.environment.IS_IMPERSONATION_ENABLED;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<InstitutionResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private readonly authQuery: AuthQuery,
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
        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/institutions', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<InstitutionResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    updateInstitution(institutionID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_INSTITUTIONS}/${CONSTANTS.ROUTER_PATH_INSTITUTIONS_UPDATE}/${institutionID}`,
        );
    }

    impersonate(username: string) {
        this.impersonationService.impersonate(username, RoleEnum.INSTITUTION).subscribe({
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

    routeToSyncHistory(institutionID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_INSTITUTIONS}/${institutionID}/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
        );
    }
}

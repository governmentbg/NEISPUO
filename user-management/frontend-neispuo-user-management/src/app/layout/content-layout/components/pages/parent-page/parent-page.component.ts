import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { TranslateService } from '@ngx-translate/core';
import { ParentResponseDTO } from '@shared/business-object-model/responses/parent-response.dto';
import { UserManagementResponse } from '@shared/business-object-model/responses/user-management-response';
import { HasAzureIDEnum } from '@shared/enums/has-azure-id.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ParentService } from '@shared/services/parent.service';
import { PersonService } from '@shared/services/person.service';
import { PreventButtonClickService } from '@shared/services/prevent-button-click.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { ParentTableColumnsConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { CONSTANTS } from 'src/app/shared/constants';
import { AuthQuery } from '@core/authentication/auth.query';

@Component({
    selector: 'app-parent-page',
    templateUrl: './parent-page.component.html',
    styleUrls: ['./parent-page.component.scss'],
})
export class ParentPageComponent
    extends PaginatedTableList<ParentResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('parentTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    lastTableLazyLoadEvent!: LazyLoadEvent;

    columnConfig: ColumnServiceConfig<ParentResponseDTO> = ParentTableColumnsConfig;

    public readonly isMon$ = this.authQuery.isMon$;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<ParentResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private parentService: ParentService,
        private personService: PersonService,
        private preventButtonClickService: PreventButtonClickService,
        private authQuery: AuthQuery,
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
            .get('/v1/parent', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<ParentResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    async syncParent(user: ParentResponseDTO) {
        this.loading = true;
        if (this.preventButtonClickService.checkButton(user?.personID?.toString()!)) {
            this.personService.printFiveMinutesWarrningMessage();
            this.loading = false;
            return;
        }
        this.preventButtonClickService.lockButton(user?.personID?.toString()!);
        this.subSink.sink = this.parentService
            .callSyncParent(user)
            .subscribe(
                (success: UserManagementResponse<ParentResponseDTO>) => {
                    this.parentService.printSuccessMessage();
                    // this.tableRef.filter(user.personID, 'personID', 'equals');
                },
                (error) => {
                    this.parentService.printErrorMessage(error);
                },
            )
            .add(() => (this.loading = false));
    }

    refreshTableData() {
        this.load(this.lastTableLazyLoadEvent);
    }

    isSyncButtonEnabled(dto: ParentResponseDTO) {
        return this.parentService.isSyncButtonEnabled(dto);
    }

    routeToSyncHistory(personID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_PARENTS}/${personID}/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
        );
    }

    routeToLinkedUsersPage(personID: number) {
        this.router.navigateByUrl(
            `/${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_PARENTS}/${personID}/${CONSTANTS.ROUTER_PATH_LINKED_USERS}`,
        );
    }
}

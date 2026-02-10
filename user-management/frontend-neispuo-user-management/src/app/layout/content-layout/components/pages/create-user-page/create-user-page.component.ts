import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { TranslateService } from '@ngx-translate/core';
import { AzureUsersResponseDTO } from '@shared/business-object-model/responses/azure-users-response.dto';
import { ErrorsResponseDTO } from '@shared/business-object-model/responses/errors-response.dto';
import { NonSyncedPersonResponseDTO } from '@shared/business-object-model/responses/non-synced-person-response.dto';
import { StudentResponseDTO } from '@shared/business-object-model/responses/student-response.dto';
import { UserManagementResponse } from '@shared/business-object-model/responses/user-management-response';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { PersonService } from '@shared/services/person.service';
import { PreventButtonClickService } from '@shared/services/prevent-button-click.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { CreateUserTableColumnsConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';

@Component({
    selector: 'app-create-user-page',
    templateUrl: './create-user-page.component.html',
    styleUrls: ['./create-user-page.component.scss'],
})
export class CreateUserPageComponent
    extends PaginatedTableList<NonSyncedPersonResponseDTO>
    implements AfterViewInit, OnInit, OnDestroy
{
    @ViewChild('createUserTable') tableRef: Table = {} as Table;

    lastTableLazyLoadEvent!: LazyLoadEvent;

    loading = false;

    personalID: string = '';

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<NonSyncedPersonResponseDTO> = CreateUserTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<ErrorsResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private personService: PersonService,

        private preventButtonClickService: PreventButtonClickService,
    ) {
        super(router, route, dotNotatedPipe, columnService, translateService);
        this.columnService.setConfig(this.columnConfig);
    }

    ngAfterViewInit(): void {
        this.loadByUrl();
    }

    ngOnInit(): void {
        this.columnService.reloadVisibleColumns();
    }

    searchForUser() {
        this.load(this.lastTableLazyLoadEvent);
    }

    ngOnDestroy() {
        this.subSink.unsubscribe();
    }

    public load(event: LazyLoadEvent) {
        this.lastTableLazyLoadEvent = event;
        this.loading = true;
        this.scrollService.scrollTo('body');
        this.subSink.sink = this.apiService
            .get('/v1/non-synced-person-crud', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<NonSyncedPersonResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }

    refreshTableData() {
        this.load(this.lastTableLazyLoadEvent);
    }

    async createNonSyncedPerson(personID: number) {
        this.loading = true;
        if (this.preventButtonClickService.checkButton(personID?.toString())) {
            this.personService.printFiveMinutesWarrningMessage();
            this.loading = false;
            return;
        }
        this.preventButtonClickService.lockButton(personID?.toString());
        this.subSink.sink = this.personService
            .callNonSyncedPersonCreateEndpoint({ personID })
            .subscribe(
                (success: UserManagementResponse<AzureUsersResponseDTO>) => {
                    this.personService.printSuccessMessage();
                },
                (error) => {
                    this.personService.printErrorMessage(error);
                },
            )
            .add(() => (this.loading = false));
    }
}

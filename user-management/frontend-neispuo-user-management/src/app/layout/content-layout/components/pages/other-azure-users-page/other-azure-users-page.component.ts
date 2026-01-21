import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { OtherAzureUsersResponseDTO } from '@shared/business-object-model/responses/other-azure-users-response.dto';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { OtherAzureUsersTableColumnsConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-other-azure-users-page',
    templateUrl: './other-azure-users-page.component.html',
    styleUrls: ['./other-azure-users-page.component.scss'],
})
export class OtherAzureUsersPageComponent
    extends PaginatedTableList<OtherAzureUsersResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('otherAzureUsersTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<OtherAzureUsersResponseDTO> = OtherAzureUsersTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<OtherAzureUsersResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
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
            .get('/v1/other-azure-users', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<OtherAzureUsersResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }
}

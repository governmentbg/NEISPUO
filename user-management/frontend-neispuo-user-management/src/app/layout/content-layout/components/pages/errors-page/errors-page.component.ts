import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { CONSTANTS } from '@shared/constants';
import { EventStatus } from '@shared/enums/event-status.enum';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { take } from 'rxjs/operators';
import { ErrorsTableColumnsConfig } from 'src/app/configs/datatables-config';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';
import { ErrorsResponseDTO } from '@shared/business-object-model/responses/errors-response.dto';
import { ErrorsService } from '@shared/services/errors.service';

@Component({
    selector: 'app-errors-page',
    templateUrl: './errors-page.component.html',
    styleUrls: ['./errors-page.component.scss'],
})
export class ErrorsPageComponent
    extends PaginatedTableList<ErrorsResponseDTO>
    implements OnInit, AfterViewInit, OnDestroy
{
    @ViewChild('errorsTable') tableRef: Table = {} as Table;

    lastTableLazyLoadEvent!: LazyLoadEvent;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<ErrorsResponseDTO> = ErrorsTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<ErrorsResponseDTO>,
        public scrollService: ScrollService,
        private apiService: ApiService,
        private messageService: MessageService,
        private errorsService: ErrorsService,
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
            .get('/v1/log', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<ErrorsResponseDTO>) => {
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
}

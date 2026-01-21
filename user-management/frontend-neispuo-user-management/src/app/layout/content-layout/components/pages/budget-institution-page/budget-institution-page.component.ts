import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '@core/services/api.service';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { PaginatedTableList } from '@shared/models/paginated-table-list';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/scroll.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { BudgetingInstitutionsTableColumnsConfig } from 'src/app/configs/datatables-config';
import { BudgetingInstitutionResponseDTO } from '@shared/business-object-model/responses/budgeting-institution-response.dto';
import { SubSink } from 'subsink';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-budget-institution-page',
    templateUrl: './budget-institution-page.component.html',
    styleUrls: ['./budget-institution-page.component.scss'],
})
export class BudgetInstitutionPageComponent
    extends PaginatedTableList<BudgetingInstitutionResponseDTO>
    implements OnInit, OnDestroy, AfterViewInit
{
    @ViewChild('budgetInstitutionTable') tableRef: Table = {} as Table;

    loading = true;

    subSink = new SubSink();

    columnConfig: ColumnServiceConfig<BudgetingInstitutionResponseDTO> = BudgetingInstitutionsTableColumnsConfig;

    constructor(
        router: Router,
        route: ActivatedRoute,
        dotNotatedPipe: DotNotatedPipe,
        translateService: TranslateService,
        columnService: DynamicColumnService<BudgetingInstitutionResponseDTO>,
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
            .get('/v1/budgeting-institutions', this.createQueryParams(event))
            .subscribe(
                (success: GetManyDefaultResponse<BudgetingInstitutionResponseDTO>) => {
                    this.paginatedEntries = success;
                },
                (error) => {
                    console.log(error);
                },
            )
            .add(() => (this.loading = false));
    }
}

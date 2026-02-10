import { ActivatedRoute, Router } from '@angular/router';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
import { TranslateService } from '@ngx-translate/core';
import { CONSTANTS } from '@shared/constants';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import * as objectPath from 'object-path';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { GetManyDefaultResponse } from './get-many-default-response';

interface CustomDotNotatedPropertyConfig<T> {
    keyName: keyof T;
    path: string;
    nestedArrayKey?: string;
    translate?: boolean;
}

export abstract class PaginatedTableList<T> {
    tableRef: any;

    CONSTANTS = CONSTANTS;

    // p calendar asks for a tempVariable to store the user chosen date
    tempDate!: string;

    customDotNotatedProperties: CustomDotNotatedPropertyConfig<T>[] = [];

    filterDelay: number = CONSTANTS.PRIMENG_CONFIG_FILTER_DELAY_MILISECS;

    paginatedEntries: GetManyDefaultResponse<T> = {
        count: 25,
        data: [],
        page: 1,
        pageCount: 0,
        total: 0,
    };

    pageSize = 25;

    loading = true;

    constructor(
        public router: Router,
        public route: ActivatedRoute,
        public dotNotatedPipe: DotNotatedPipe,
        public columnService: DynamicColumnService<T>,
        public translateService: TranslateService,
    ) {}

    public abstract load(event: LazyLoadEvent): any;

    public onColumnChoiceChange(event: { value: string[] }) {
        const selectedFields = event?.value || [];
        this.columnService.updateVisibleColumns(selectedFields);
        const lazyEvent: LazyLoadEvent = (this.tableRef as Table).createLazyLoadMetadata();
        // Delete value of the additional filter when it hides
        // delete lazyEvent.filters[event['itemValue']];
        Object.assign(this.tableRef, lazyEvent);
        this.load(lazyEvent);
    }

    public loadByUrl() {
        this.route.queryParams.subscribe((params) => {
            const event: LazyLoadEvent = params.tableMeta
                ? JSON.parse(params.tableMeta)
                : this.tableRef.createLazyLoadMetadata();
            Object.assign(this.tableRef, event);
            this.load(event);
        });
    }

    public navigate(event: any) {
        this.router.navigate([], {
            queryParams: { tableMeta: JSON.stringify(event) },
        });
    }

    public createQueryParams(event: LazyLoadEvent, pageSize = this.pageSize) {
        const eventCopy = { ...event, first: event.first ? event.first : 0 };
        const page = Math.ceil(eventCopy.first / pageSize) + 1;
        const searches = this.createSearchFilters(eventCopy);

        let qb = RequestQueryBuilder.create()
            .search({
                $and: searches,
            })
            .setPage(page)
            .setLimit(pageSize);

        if (eventCopy.sortField && eventCopy.sortOrder) {
            qb = qb.sortBy({
                field: eventCopy.sortField,
                order: eventCopy.sortOrder < 0 ? 'DESC' : 'ASC',
            });
        } else if (
            this.tableRef.el.nativeElement.id === 'organizationsTable' ||
            this.tableRef.el.nativeElement.id === 'azureUsersTable' ||
            this.tableRef.el.nativeElement.id === 'azureClassesTable' ||
            this.tableRef.el.nativeElement.id === 'azureEnrollmentsTable' ||
            this.tableRef.el.nativeElement.id === 'loginAuditTable'
        ) {
            qb = qb.sortBy({ field: 'createdOn', order: 'DESC' });
        } else if (this.tableRef.el.nativeElement.id === 'roleAuditTable') {
            qb = qb.sortBy({ field: 'DateUtc', order: 'DESC' });
        } else if (
            this.tableRef.el.nativeElement.id === 'monTable' ||
            this.tableRef.el.nativeElement.id === 'ruoTable' ||
            this.tableRef.el.nativeElement.id === 'municipalityTable' ||
            this.tableRef.el.nativeElement.id === 'budgetInstitutionTable' ||
            this.tableRef.el.nativeElement.id === 'otherAzureUsersTable'
        ) {
            qb = qb.sortBy([{ field: 'sysUserID', order: 'ASC' }]);
        } else if (
            this.tableRef.el.nativeElement.id === 'teachersTable' ||
            this.tableRef.el.nativeElement.id === 'studentUsersTable'
        ) {
            qb = qb.sortBy([
                { field: 'publicEduNumber', order: 'ASC' },
                { field: 'hasAzureID', order: 'DESC' },
            ]);
        }
        return qb.queryObject;
    }

    public createSearchFilters(eventCopy: LazyLoadEvent): any[] {
        const searches = [] as any[];
        if (!eventCopy.filters) {
            return searches;
        }
        for (const [key, filter] of Object.entries(eventCopy.filters)) {
            if (!key) {
                continue; // catches not yet implemented fields
            }
            searches.push({
                [key]: { [filter.matchMode as any]: filter.value },
            });
        }

        return searches;
    }

    public getToDisplayValue(value: T, dotNotatedProperty: string) {
        const extractedDotNotated = this.dotNotatedPipe.transform([value, dotNotatedProperty]);

        for (const c of this.customDotNotatedProperties) {
            if (dotNotatedProperty === c.keyName) {
                const obj = objectPath.get(value as any, c.path);
                console.log(obj);
                return c.translate ? this.translateService.instant(obj) : obj;
            }
        }
        return extractedDotNotated;
    }

    onDateSelect(event: any, field: string, operator: string) {
        console.log(event);
        // below line will ocnvert the date from the calendar so that its not a utc date. we do this to avoid the backend from transforming it again.
        const chosenDate = new Date(Date.UTC(event.getFullYear(), event.getMonth(), event.getDate()));
        const nextDay = new Date(Date.UTC(event.getFullYear(), event.getMonth(), event.getDate() + 1));
        this.tableRef.filter([chosenDate, nextDay], field, operator);
    }

    onDateClear(field: string, operator: string) {
        this.tableRef.filter('', field, operator);
    }
}

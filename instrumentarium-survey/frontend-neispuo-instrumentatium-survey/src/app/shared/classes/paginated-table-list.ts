import { ActivatedRoute, Router } from '@angular/router';
import { GetManyDefaultResponse } from '@nestjsx/crud';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
import { DynamicColumnService } from '../services/dynamic-column.service';
import * as objectPath from 'object-path';
import { DotNotatedPipe } from '@shared/modules/pipes/dot-notated.pipe';
import { TranslatePipe } from '@shared/modules/pipes/translate.pipe';
import { LazyLoadEvent } from 'primeng/api';

interface CustomDotNotatedPropertyConfig<T> {
  keyName: keyof T;
  path: string;
  nestedArrayKey?: string;
  translate?: boolean;
}

export abstract class PaginatedTableList<T> {
  tableRef;

  customDotNotatedProperties: CustomDotNotatedPropertyConfig<T>[] = [];

  paginatedEntries: GetManyDefaultResponse<T> = {
    count: 25,
    data: [],
    page: 1,
    pageCount: null,
    total: null
  };

  constructor(
    public router: Router,
    public route: ActivatedRoute,
    public dotNotatedPipe: DotNotatedPipe,
    public columnService: DynamicColumnService<T>,
    public translatePipe: TranslatePipe
  ) { }

  public abstract load(event: LazyLoadEvent);

  public onColumnChoiceChange(event: { value: string[] }) {
    const selectedFields = event?.value || [];
    this.columnService.updateVisibleColumns(selectedFields);
    const lazyEvent: LazyLoadEvent = this.tableRef.createLazyLoadMetadata();
    // Delete value of the additional filter when it hides
    delete lazyEvent.filters[event['itemValue']];
    Object.assign(this.tableRef, lazyEvent);
    this.load(lazyEvent);
  }

  public loadByUrl() {
    this.route.queryParams.subscribe(params => {
      const event: LazyLoadEvent = params.tableMeta
        ? JSON.parse(params.tableMeta)
        : this.tableRef.createLazyLoadMetadata();
      Object.assign(this.tableRef, event);
      this.load(event);
    });
  }

  pageSize = 10;
  loading = true;

  public navigate(event) {
    this.router.navigate([], { queryParams: { tableMeta: JSON.stringify(event) } });
  }

  public createQueryParams(event: LazyLoadEvent, pageSize = this.pageSize) {
    const eventCopy = { ...event };
    const page = eventCopy.first / pageSize + 1;
    const searches = this.createSearchFilters(eventCopy);

    let qb = RequestQueryBuilder.create()
      .search({
        $and: searches
      })
      .setPage(page)
      .setLimit(pageSize);

    if (eventCopy.sortField) {
      qb = qb.sortBy({
        field: eventCopy.sortField,
        order: eventCopy.sortOrder < 0 ? 'DESC' : 'ASC'
      });
    }

    return qb.queryObject;
  }

  public createSearchFilters(eventCopy: LazyLoadEvent): any[] {
    const searches = [];
    for (const [key, filter] of Object.entries(eventCopy.filters)) {
      if (!key) {
        continue; // catches not yet implemented fields
      }
      searches.push({
        [key]: { [filter.matchMode]: filter.value }
      });
    }

    return searches;
  }
  public getToDisplayValue(value: T, dotNotatedProperty: string) {
    const extractedDotNotated = this.dotNotatedPipe.transform([value, dotNotatedProperty]);

    for (let c of this.customDotNotatedProperties) {
      if (dotNotatedProperty === c.keyName) {
        let obj = objectPath.get(value as any, c.path);

        return c.translate
          ? this.translatePipe.transform(obj)
          : obj;
      }
    }
    return extractedDotNotated;
  }
}

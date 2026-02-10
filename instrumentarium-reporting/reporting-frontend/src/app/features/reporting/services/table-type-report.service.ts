import { Injectable } from '@angular/core';
import { CubejsClient } from '@cubejs-client/ngx';
import { LazyLoadEvent } from 'primeng/api';
import { BehaviorSubject, firstValueFrom, lastValueFrom } from 'rxjs';
import { ReportService } from './report.service';
import { CONSTANTS } from '@shared/constants';
import { CubeJsClientService } from '@shared/services/cubejs-client.service';
import { PivotTableTypeReportService } from './pivot-table-type-report.service';

@Injectable()
export class TableTypeReportService {
  private _data = new BehaviorSubject<any[]>([]);
  private cubejs: CubejsClient = null;

  public data$ = this._data.asObservable();

  public isLoading = false;
  public isLoadingNext = false;
  public rows = 100;
  public first = 0;
  public lastFetchedRowsCount = 0;

  set data(response) {
    this._data.next(response);
  }

  get data() {
    return this._data.getValue();
  }

  constructor(
    private reportService: ReportService,
    private cubeJsClientService: CubeJsClientService,
    public pivotTableTypeReportService: PivotTableTypeReportService
  ) {
    this.initCubeJsClientService();
  }

  private async initCubeJsClientService() {
    this.cubejs = await firstValueFrom(this.cubeJsClientService.cubeJs$);
  }
  async load(event: LazyLoadEvent) {
    this.isLoading = true;

    try {
      if (this.reportService.isCubeData) {
        this.reportService.lastLazyLoadEvent = { ...event, first: this.first };
      } else {
        this.reportService.lastLazyLoadEvent = {
          ...this.reportService.lastLazyLoadEvent,
          ...event,
          filters: Object.keys(event.filters).length
            ? JSON.parse(JSON.stringify(event.filters))
            : JSON.parse(JSON.stringify(this.reportService.lastLazyLoadEvent.filters)),
          sortField: event.sortField && !!event.sortOrder ? event.sortField : this.reportService.lastLazyLoadEvent.sortField,
          sortOrder: event.sortField && !!event.sortOrder ? event.sortOrder : this.reportService.lastLazyLoadEvent.sortOrder
        };
      }

      await this.reportService.createQueryObject(
        this.reportService.lastLazyLoadEvent,
        this.reportService.selectedDimensions,
        this.reportService.selectedMeasures
      );

      /**
       * Check if filter has been changed so we can reset the whole data object
       */
      const filterHasChanged = this.reportService.checkFilterHasChanged(this.reportService.queryObject);

      const resultSet = await lastValueFrom(this.cubejs.load(this.reportService.queryObject));
      this.reportService.resultSet = resultSet;

      const responseData = resultSet.rawData();

      /**
       * Supports the functionality of loading next results because Cube.js /load endpoint's response doesn't return
       * the total number of rows. Оffset and limit query parameters are used to check if there is a next page available
       * by means of limit and row count comparison.
       */
      if ((responseData.length <= this.rows && this.first === 0) || filterHasChanged || !this.isLoadingNext) {
        this.data = [...responseData];
      } else {
        this.data = [...this.data, ...responseData];
        this.isLoadingNext = false;
      }

      this.reportService.lastAppliedFilters = this.reportService.queryObject.filters;
      this.reportService.lastAppliedOrder = this.reportService.queryObject.order;
      this.lastFetchedRowsCount = responseData.length;

      return responseData;
    } catch (err) {
      this.isLoading = false;
      return false;
    } finally {
      this.isLoading = false;
    }
  }

  async loadNext() {
    this.isLoadingNext = true;
    this.first += this.rows;

    await this.load({
      ...this.reportService.lastLazyLoadEvent,
      first: this.first
    });
  }

  async updateSelectedColumns(event: any) {
    if (event.value?.aggType) {
      this.reportService.selectedMeasures = [event.value];
    } else {
      this.reportService.selectedDimensions = event.value;
    }
  }

  async loadTableDataForExcelExport() {
    this.isLoading = true;

    const queryObject: any = {
      dimensions: this.reportService.selectedDimensions.map((sd) => sd.name),
      measures: this.reportService.selectedMeasures.map((sm) => sm.name),
      filters: this.reportService.lastAppliedFilters,
      limit: CONSTANTS.CUBEJS_MAX_RESULTS,
      offset: 0,
      order: this.reportService.lastAppliedOrder
    };

    try {
      const resultSet = await lastValueFrom(this.cubejs.load(queryObject));
      const responseData = resultSet.rawData();
      return responseData;
    } catch (err) {
      this.isLoading = false;
      return false;
    } finally {
      this.isLoading = false;
    }
  }

  cleanUpData() {
    this.data = [];
  }
}

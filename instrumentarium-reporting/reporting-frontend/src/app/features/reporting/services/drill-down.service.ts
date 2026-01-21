import { Injectable } from '@angular/core';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { CubejsClient } from '@cubejs-client/ngx';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { CONSTANTS } from '@shared/constants';
import { CubeJsClientService } from '@shared/services/cubejs-client.service';
import { LazyLoadEvent } from 'primeng/api';
import { BehaviorSubject, combineLatest, firstValueFrom, lastValueFrom, take } from 'rxjs';
import { PieChartTypeReportService } from './pie-chart-type-report.service';
import { PivotTableTypeReportService } from './pivot-table-type-report.service';
import { ReportService } from './report.service';

@Injectable()
export class DrillDownService {
  private _drillDownData = new BehaviorSubject<any>([]);
  private _drillDownQuery = new BehaviorSubject<any>({});
  private _drillDownDimensions = new BehaviorSubject<any>([]);
  private _drillDownMeasures = new BehaviorSubject<any>([]);
  private _drillDownColumns = new BehaviorSubject<any>([]);
  private _initialDrillDownFilters = new BehaviorSubject<any[]>([]);
  private _lastAppliedDrillDownFilters = new BehaviorSubject<any[]>([]);
  private cubejs: CubejsClient = null;

  public isLoading = false;
  public isLoadingNext = false;
  public drillDownRows = 50;
  public drillDownFirst = 0;
  public lastFetchedDrillDownRowsCount = 0;
  private lazyLoadEvent: LazyLoadEvent;
  public isPieChartData = false;

  set drillDownData(response) {
    this._drillDownData.next(response);
  }

  get drillDownData() {
    return this._drillDownData.getValue();
  }

  set drillDownQuery(response) {
    this._drillDownQuery.next(response);
  }

  get drillDownQuery() {
    return this._drillDownQuery.getValue();
  }

  set drillDownDimensions(response) {
    this._drillDownDimensions.next(response);
  }

  get drillDownDimensions() {
    return this._drillDownDimensions.getValue();
  }

  set drillDownMeasures(response) {
    this._drillDownMeasures.next(response);
  }

  get drillDownMeasures() {
    return this._drillDownMeasures.getValue();
  }

  set drillDownColumns(response) {
    this._drillDownColumns.next(response);
  }

  get drillDownColumns() {
    return this._drillDownColumns.getValue();
  }

  set requiredDrillDownQueryFilters(response) {
    this._initialDrillDownFilters.next(response);
  }

  get requiredDrillDownQueryFilters() {
    return this._initialDrillDownFilters.getValue();
  }

  set lastAppliedDrillDownFilters(response) {
    this._lastAppliedDrillDownFilters.next(response);
  }

  get lastAppliedDrillDownFilters() {
    return this._lastAppliedDrillDownFilters.getValue();
  }

  constructor(
    private cubeJsClientService: CubeJsClientService,
    private reportService: ReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    private rbQuery: ReportBuilderQuery,
    private pieChartService: PieChartTypeReportService,
    private toastService: AppToastMessageService
  ) {
    this.initCubeJsClientService();
  }
  private async initCubeJsClientService() {
    this.cubejs = await firstValueFrom(this.cubeJsClientService.cubeJs$);
  }

  async loadDrillDownData(event: LazyLoadEvent, dataIndex: string, rowIndex: number) {
    if (
      !this.reportService.columnIsMeasure(dataIndex) ||
      (this.isPieChartData && rowIndex === CONSTANTS.PIE_CHART_LAST_SLICE_INDEX)
    ) {
      return false;
    }

    this.isLoading = true;

    try {
      this.lazyLoadEvent = event;

      const pivotConfig = this.isPieChartData
        ? this.pieChartService.pieChartPivotConfig
        : this.pivotTableTypeReportService.tablePivotConfig;

      // A pie chart type visualization shows the first N highest values.
      // Sorting the data is necessary to get the correct drill down data.
      const chartPivot = this.isPieChartData
        ? this.reportService.resultSet.chartPivot(pivotConfig).sort((a, b) => b[dataIndex] - a[dataIndex])
        : this.reportService.resultSet.chartPivot(pivotConfig);

      let xValues = chartPivot[rowIndex].xValues;

      const yValues = this.reportService.resultSet.seriesNames(pivotConfig).find((n) => n.key === dataIndex).yValues;

      const drillDownResult = this.reportService.resultSet.drillDown({ xValues, yValues }, pivotConfig);

      this.drillDownQuery = { ...drillDownResult, limit: this.drillDownRows, offset: this.drillDownFirst };
      this.requiredDrillDownQueryFilters = [...this.drillDownQuery.filters];

      const availableColumns$ = combineLatest([this.rbQuery.availableDimensions$, this.rbQuery.availableMeasures$]);
      availableColumns$.pipe(take(1)).subscribe({
        next: ([d, m]) => {
          this.drillDownDimensions = d.filter((d) => this.drillDownQuery.dimensions.includes(d.name));
          this.drillDownMeasures = m.filter((m) => this.drillDownQuery.measures.includes(m.name));
          this.drillDownColumns = [...this.drillDownDimensions];
        },
        error: (err) => {
          this.toastService.displayError();
        }
      });

      const tableFilters = this.reportService.getCubeFilter(event.filters);
      const order = event.sortField
        ? [[event.sortField, event.sortOrder === 1 ? 'asc' : 'desc']]
        : [[this.drillDownDimensions[0].name, 'asc']];

      this.drillDownQuery.filters = [...tableFilters, ...this.drillDownQuery.filters];
      this.drillDownQuery.order = order;

      this.lastAppliedDrillDownFilters = this.drillDownQuery.filters;

      /**
       * Check if filter has been changed so we can reset the whole data object
       */
      const filterHasChanged = this.checkFilterHasChanged(this.drillDownQuery);

      const resultSet = await lastValueFrom(this.cubejs.load(this.drillDownQuery));
      const responseData = resultSet.rawData();

      /**
       * Supports the functionality of loading next results because Cube.js /load endpoint's response doesn't return
       * the total number of drillDownRows. Оffset and limit query parameters are used to check if there is a next page available
       * by means of limit and row count comparison.
       */
      if (
        (responseData.length <= this.drillDownRows && this.drillDownFirst === 0) ||
        filterHasChanged ||
        !this.isLoadingNext
      ) {
        this.drillDownData = responseData;
      } else {
        this.drillDownData = [...this.drillDownData, ...responseData];
        this.isLoadingNext = false;
      }

      this.lastFetchedDrillDownRowsCount = responseData.length;

      return responseData;
    } catch (err) {
      this.isLoading = false;
      return false;
    } finally {
      this.isLoading = false;
    }
  }

  public checkFilterHasChanged(queryObject: any): boolean {
    let filterHasChanged = false;

    if (this.lastAppliedDrillDownFilters.length !== queryObject.filters.length) {
      filterHasChanged = true;
    } else {
      for (let lastFilterValue of this.lastAppliedDrillDownFilters) {
        if (lastFilterValue.operator !== 'measureFilter') {
          let filter = queryObject.filters.find((f: any) => {
            return f.member === lastFilterValue.member;
          });
          if (!filter) {
            filterHasChanged = true;
            break;
          }
          if (filter.values.join(',') !== lastFilterValue.values.join(',')) {
            filterHasChanged = true;
            break;
          }
        }
      }
    }
    return filterHasChanged;
  }

  async loadNext(dataIndex: string, rowIndex: number) {
    this.isLoadingNext = true;
    this.drillDownFirst += this.drillDownRows;

    await this.loadDrillDownData(this.lazyLoadEvent, dataIndex, rowIndex);
  }

  cleanUpData() {
    this.drillDownFirst = 0;
    this.drillDownData = [];
    this.drillDownQuery = {};
    this.drillDownDimensions = [];
    this.drillDownMeasures = [];
    this.drillDownColumns = [];
    this.requiredDrillDownQueryFilters = [];
    this.lastAppliedDrillDownFilters = [];
    this.isPieChartData = false;
  }
}

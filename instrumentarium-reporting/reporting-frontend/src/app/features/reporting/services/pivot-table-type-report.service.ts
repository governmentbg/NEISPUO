import { Injectable } from '@angular/core';
import { CubejsClient } from '@cubejs-client/ngx';
import { LazyLoadEvent } from 'primeng/api';
import { BehaviorSubject, firstValueFrom, lastValueFrom } from 'rxjs';
import { ReportService } from './report.service';
import { CONSTANTS } from '@shared/constants';
import { CubeJsClientService } from '@shared/services/cubejs-client.service';

@Injectable()
export class PivotTableTypeReportService {
  private _tablePivotConfig = new BehaviorSubject<any>({});
  public tablePivotConfig$ = this._tablePivotConfig.asObservable();
  private _tablePivotData = new BehaviorSubject<any>([]);
  private _pivotTableHeaderRows = new BehaviorSubject<any>([]);
  private _pivotTableDataIndexes = new BehaviorSubject<any>([]);
  private _lazyLoadOnInit = new BehaviorSubject<boolean>(true);

  public isLoading = false;
  public isLoadingNextPivot = false;
  public rows = 100;
  public first = 0;
  public lastFetchedRowsCount = 0;
  private cubejs: CubejsClient;

  set tablePivotConfig(response) {
    this._tablePivotConfig.next(response);
  }

  get tablePivotConfig() {
    return this._tablePivotConfig.getValue();
  }

  set tablePivotData(response) {
    this._tablePivotData.next(response);
  }

  get tablePivotData() {
    return this._tablePivotData.getValue();
  }

  set pivotTableHeaderRows(response) {
    this._pivotTableHeaderRows.next(response);
  }

  get pivotTableHeaderRows() {
    return this._pivotTableHeaderRows.getValue();
  }

  set pivotTableDataIndexes(response) {
    this._pivotTableDataIndexes.next(response);
  }

  get pivotTableDataIndexes() {
    return this._pivotTableDataIndexes.getValue();
  }

  set lazyLoadOnInit(response) {
    this._lazyLoadOnInit.next(response);
  }

  get lazyLoadOnInit() {
    return this._lazyLoadOnInit.getValue();
  }

  constructor(private reportService: ReportService, private cubeJsClientService: CubeJsClientService) {
    this.initCubeJsClientService();
  }

  private async initCubeJsClientService() {
    this.cubejs = await firstValueFrom(this.cubeJsClientService.cubeJs$);
  }

  async loadPivot(event: LazyLoadEvent, pivotConfig?: any) {
    this.isLoading = true;

    try {
      if (!this.reportService.lastLazyLoadEvent) {
        this.reportService.lastLazyLoadEvent = event;
      }

      await this.reportService.createQueryObject(
        {
          ...this.reportService.lastLazyLoadEvent,
          first: this.first,
          rows:
            event.rows ||
            (pivotConfig || this.reportService.lastLazyLoadEvent.pivotConfig ? CONSTANTS.CUBEJS_MAX_RESULTS : this.rows)
        },
        this.reportService.selectedDimensions,
        this.reportService.selectedMeasures
      );

      /**
       * Check if filter has been changed so we can reset the whole data object
       */
      const filterHasChanged = this.reportService.checkFilterHasChanged(this.reportService.queryObject);

      const resultSet = await lastValueFrom(this.cubejs.load(this.reportService.queryObject));
      this.reportService.resultSet = resultSet;

      const fallbackPivotConfig = {
        x: [...this.reportService.selectedDimensions.map((d) => d.name)],
        y: [...this.reportService.selectedMeasures.map((m) => m.name)]
      };

      this.tablePivotConfig = pivotConfig || this.reportService.lastLazyLoadEvent.pivotConfig || fallbackPivotConfig;

      let pivotRawData = resultSet.tablePivot(this.tablePivotConfig);
      let pivotTableCols = resultSet.tableColumns(this.tablePivotConfig);

      let headerRowsData = [];
      let dataIndexes = [];
      this.traverseColumnsToGetHeaderRows(0, pivotTableCols, headerRowsData);
      this.traverseColumnsToGetDataIndexes(pivotTableCols, dataIndexes);

      const tableHeaders = headerRowsData.splice(0, headerRowsData.length - 1);

      for (let i = 0; i < tableHeaders.length; i++) {
        if (i < tableHeaders.length - 1) {
          // If we have next element
          const diff = tableHeaders[i].length - tableHeaders[i + 1].length;

          if (diff > 0) {
            for (let j = 0; j < diff; j++) {
              tableHeaders[i + 1].unshift(null);
            }
          }
        }
      }

      /**
       * Supports the functionality of loading next results because Cube.js /load endpoint's response doesn't return
       * the total number of rows. Оffset and limit query parameters are used to check if there is a next page available
       * by means of limit and row count comparison.
       */
      if ((pivotRawData.length <= this.rows && this.first === 0) || filterHasChanged || !this.isLoadingNextPivot) {
        this.tablePivotData = pivotRawData;
      } else {
        this.tablePivotData = [...this.tablePivotData, ...pivotRawData];
        this.isLoadingNextPivot = false;
      }

      this.pivotTableHeaderRows = tableHeaders;
      this.pivotTableDataIndexes = dataIndexes;

      this.reportService.lastAppliedFilters = this.reportService.queryObject.filters;
      this.reportService.lastAppliedOrder = this.reportService.queryObject.order;
      this.lastFetchedRowsCount = pivotRawData.length;

      return pivotRawData;
    } catch (err) {
      this.isLoading = false;
      return false;
    } finally {
      this.isLoading = false;
      this.lazyLoadOnInit = true;
    }
  }

  traverseColumnsToGetHeaderRows(depth, columns, headerRowsData) {
    for (let col of columns) {
      if (headerRowsData[depth]) {
        headerRowsData[depth].push(col);
      } else {
        headerRowsData[depth] = [col];
      }

      if (col.children) {
        this.traverseColumnsToGetHeaderRows(depth + 1, col.children, headerRowsData);
      } else {
        if (headerRowsData[depth + 1]) {
          headerRowsData[depth + 1].push(null);
        } else {
          headerRowsData[depth + 1] = [null];
        }
      }
    }
  }

  traverseColumnsToGetDataIndexes(columns, dataIndexes) {
    for (let col of columns) {
      if (col.children) {
        this.traverseColumnsToGetDataIndexes(col.children, dataIndexes);
      } else {
        dataIndexes.push(col.dataIndex);
      }
    }
  }

  async updatePivotConfig(xValues: string[], yValues: string[]) {
    this.tablePivotConfig = {
      x: xValues.slice(),
      y: yValues.slice()
    };
  }

  async loadNextPivot() {
    this.isLoadingNextPivot = true;
    this.first += this.rows;
    await this.loadPivot({
      ...this.reportService.lastLazyLoadEvent,
      first: this.first
    });
  }

  cleanUpData() {
    this.tablePivotConfig = {};
    this.tablePivotData = [];
    this.pivotTableHeaderRows = [];
    this.pivotTableDataIndexes = [];
  }
}

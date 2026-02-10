import { Injectable } from '@angular/core';
import { CubejsClient } from '@cubejs-client/ngx';
import { LazyLoadEvent } from 'primeng/api';
import { BehaviorSubject, firstValueFrom, lastValueFrom } from 'rxjs';
import { ReportService } from './report.service';
import { CONSTANTS } from '@shared/constants';
import { CubeJsClientService } from '@shared/services/cubejs-client.service';
import { ChartDataset, ChartOptions } from 'chart.js';

@Injectable()
export class PieChartTypeReportService {
  private _pieChartData = new BehaviorSubject<ChartDataset[]>([]);
  private _pieChartLabels = new BehaviorSubject<any[]>([]);
  private _pieChartPivotConfig = new BehaviorSubject<any>(null);
  public pieChartPivotConfig$ = this._pieChartPivotConfig.asObservable();
  private _loadOnInit = new BehaviorSubject<boolean>(true);
  private _isLoading = new BehaviorSubject<boolean>(false);
  public isLoading$ = this._isLoading.asObservable();

  private cubejs: CubejsClient;
  public isLoadingWaiting = false;
  public rows = 100;
  public first = 0;
  public pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    layout: {
      padding: {
        left: 10,
        right: 10,
        top: 20,
        bottom: 20
      }
    },
    plugins: {
      legend: {
        labels: {
          pointStyle: 'circle'
        }
      }
    }
  };
  public pieChartLegend = true;

  set pieChartData(response) {
    this._pieChartData.next(response);
  }

  get pieChartData() {
    return this._pieChartData.getValue();
  }

  set pieChartLabels(response) {
    this._pieChartLabels.next(response);
  }

  get pieChartLabels() {
    return this._pieChartLabels.getValue();
  }

  set pieChartPivotConfig(response) {
    this._pieChartPivotConfig.next(response);
  }

  get pieChartPivotConfig() {
    return this._pieChartPivotConfig.getValue();
  }

  set loadOnInit(response) {
    this._loadOnInit.next(response);
  }

  get loadOnInit() {
    return this._loadOnInit.getValue();
  }

  set isLoading(response) {
    this._isLoading.next(response);
  }

  get isLoading() {
    return this._isLoading.getValue();
  }

  constructor(private reportService: ReportService, private cubeJsClientService: CubeJsClientService) {
    this.initCubeJsClientService();
  }

  private async initCubeJsClientService() {
    this.cubejs = await firstValueFrom(this.cubeJsClientService.cubeJs$);
  }

  async loadPie(event: LazyLoadEvent, pivotConfig?: any) {
    this.isLoading = true;

    try {
      if (!this.reportService.lastLazyLoadEvent) {
        this.reportService.lastLazyLoadEvent = {
          first: this.first,
          rows: CONSTANTS.CUBEJS_MAX_RESULTS
        };
      }

      await this.reportService.createQueryObject(
        {
          ...this.reportService.lastLazyLoadEvent,
          first: this.first,
          rows: CONSTANTS.CUBEJS_MAX_RESULTS
        },
        this.reportService.selectedDimensions,
        this.reportService.selectedMeasures
      );

      const resultSet = await lastValueFrom(this.cubejs.load(this.reportService.queryObject));
      this.reportService.resultSet = resultSet;

      const fallbackPivotConfig = {
        x: [...this.reportService.selectedDimensions.map((d) => d.name)],
        y: [...this.reportService.selectedMeasures.map((m) => m.name)]
      };

      this.pieChartPivotConfig = pivotConfig || this.reportService.lastLazyLoadEvent.pivotConfig || fallbackPivotConfig;

      await this.updatePieChartData(resultSet, this.pieChartPivotConfig);
      return this.pieChartData;
    } catch (err) {
      this.isLoading = false;
      return false;
    } finally {
      this.isLoading = false;
      this.loadOnInit = true;
    }
  }

  async updatePieChartData(resultSet, pivotConfig) {
    let visibleData;
    this.pieChartData = resultSet.series(pivotConfig).map((item) => {
      const sortedData = item.series.sort((a, b) => b.value - a.value);
      visibleData = sortedData.slice(0, CONSTANTS.PIE_CHART_MAX_DISPLAYED_RESULTS); // Show first N results with highest values
      const otherData = sortedData.slice(CONSTANTS.PIE_CHART_MAX_DISPLAYED_RESULTS);
      const otherPieSliceValue = otherData.reduce((acc, slice) => {
        return acc + slice.value;
      }, 0);
      const otherSlice = {
        value: otherPieSliceValue,
        x: 'Друго'
      };
      if (otherPieSliceValue) {
        visibleData.push(otherSlice);
      }
      return {
        label: item.key,
        data: visibleData.map(({ value }) => value)
      };
    });

    this.pieChartLabels = visibleData.map((row) => row.x);
  }

  async updatePivotConfig(xValues: string[], yValues: string[]) {
    this.pieChartPivotConfig = {
      x: xValues.slice(),
      y: yValues.slice()
    };
  }

  cleanUpData() {
    this.pieChartData = [];
    this.pieChartLabels = [];
    this.pieChartPivotConfig = {};
    this.loadOnInit = true;
  }
}

import { Injectable } from '@angular/core';
import { BinaryOperator } from '@cubejs-client/core';
import { CubejsClient } from '@cubejs-client/ngx';
import { CONSTANTS } from '@shared/constants';
import { CubeJsClientService } from '@shared/services/cubejs-client.service';
import { PrimengConfigService } from '@shared/services/primeng-config.service';
import * as moment from 'moment';
import { FilterMatchMode, LazyLoadEvent } from 'primeng/api';
import { BehaviorSubject, firstValueFrom, map } from 'rxjs';

@Injectable()
export class ReportService {
  private _selectedDimensions = new BehaviorSubject<any>([]);
  private _selectedMeasures = new BehaviorSubject<any>([]);
  private _selectedColumns = new BehaviorSubject<any>([]);
  private _lastAppliedFilters = new BehaviorSubject<any>([]);
  private _lastLazyLoadEvent = new BehaviorSubject<any>(null);
  private _lastAppliedOrder = new BehaviorSubject<any[]>([]);
  private _queryObject = new BehaviorSubject<any>({});
  private _resultSet = new BehaviorSubject<any>(null);
  private _lastAppliedColumnFilters = new BehaviorSubject<any>({});
  private cubejs: CubejsClient = null;

  /**
   * Whether the query comes from Cube.js or from the user API
   */
  private _isCubeData = new BehaviorSubject<boolean>(null);

  public rows = 100;
  public first = 0;
  public isLoading = false;
  public lastFetchedRowsCount = 0;
  public headerRows = [];
  public dataIndexes = [];

  set selectedDimensions(response) {
    this._selectedDimensions.next(response);
  }

  get selectedDimensions() {
    return this._selectedDimensions.getValue();
  }

  set selectedMeasures(response) {
    this._selectedMeasures.next(response);
  }

  get selectedMeasures() {
    return this._selectedMeasures.getValue();
  }

  set selectedColumns(response) {
    this._selectedColumns.next(response);
  }

  get selectedColumns() {
    return this._selectedColumns.getValue();
  }

  set lastAppliedFilters(response) {
    this._lastAppliedFilters.next(response);
  }

  get lastAppliedFilters() {
    return this._lastAppliedFilters.getValue();
  }

  set lastAppliedOrder(response) {
    this._lastAppliedOrder.next(response);
  }

  get lastAppliedOrder() {
    return this._lastAppliedOrder.getValue();
  }

  set queryObject(response) {
    this._queryObject.next(response);
  }

  get queryObject() {
    return this._queryObject.getValue();
  }

  set lastLazyLoadEvent(response) {
    this._lastLazyLoadEvent.next(response);
  }

  get lastLazyLoadEvent() {
    return this._lastLazyLoadEvent.getValue();
  }

  set resultSet(response) {
    this._resultSet.next(response);
  }

  get resultSet() {
    return this._resultSet.getValue();
  }

  set isCubeData(response) {
    this._isCubeData.next(response);
  }

  get isCubeData() {
    return this._isCubeData.getValue();
  }

  set lastAppliedColumnFilters(response) {
    this._lastAppliedColumnFilters.next(response);
  }

  get lastAppliedColumnFilters() {
    return this._lastAppliedColumnFilters.getValue();
  }

  constructor(private cubeJsClientService: CubeJsClientService, private primengConfigService: PrimengConfigService) {
    this.initCubeJsClientService();
  }

  private async initCubeJsClientService() {
    this.cubejs = await firstValueFrom(this.cubeJsClientService.cubeJs$);
  }

  loadCubeMeta(routeParam: string) {
    return this.cubejs.meta({}).pipe(
      map((meta) => meta.cubes),
      map((cubes) => cubes.find((c: any) => c.name === routeParam))
    );
  }

  public getCubeFilter(filters: any) {
    if (!filters) return [];

    const filterKeys = Object.keys(filters);
    let appliedFilters = [];

    for (let fk of filterKeys) {
      if (filters[fk][0].value === undefined || filters[fk][0].value === null) continue;

      for (let filter of filters[fk]) {
        if (filter.value === undefined || filter.value === null) continue;

        let filterConstraint = {
          member: fk,
          operator: CONSTANTS.PRIMENG_CUBEJS_MATCH_MODE_MAPPING[filter.matchMode] as BinaryOperator,
          values: Array.isArray(filter.value) ? filter.value : [`${filter.value}`]
        };

        /* Dates specific case */
        if (this.primengConfigService.getfilterMatchModeOptions().date.includes(filter.matchMode)) {
          const formatedDates = filterConstraint.values.map((date) => moment(new Date(date)).format('YYYY-MM-DD'));

          /* If we want to find exact match or not with Cube we should pass array with same date at position 0 and 1 */
          if (filter.matchMode === FilterMatchMode.DATE_IS || filter.matchMode === FilterMatchMode.DATE_IS_NOT) {
            filterConstraint.values = formatedDates.concat(formatedDates);
          } else {
            filterConstraint.values = formatedDates;
          }
        }
        appliedFilters.push(filterConstraint);
      }

      const reducedFilters = [];
      const reducedArr = appliedFilters.reduce((accumulator, currentValue) => {
        if (accumulator[currentValue.member] && accumulator[currentValue.member].operator === currentValue.operator) {
          accumulator[currentValue.member].values = accumulator[currentValue.member].values.concat(currentValue.values);
        } else {
          accumulator[currentValue.member] = currentValue;
          reducedFilters.push(currentValue);
        }
        return accumulator;
      }, {});

      appliedFilters = reducedFilters;
    }

    return appliedFilters;
  }

  public async createQueryObject(event: LazyLoadEvent, dimensions: any[], measures: any[]) {
    let filters = this.getCubeFilter(event.filters);
    let limit = event.rows;
    let offset = event.first;
    let order = event.sortField
      ? [[event.sortField, event.sortOrder === 1 ? 'asc' : 'desc']]
      : [[this.selectedDimensions[0].name, 'asc']];

    this.queryObject = {
      dimensions: dimensions.map((sd) => sd.name),
      measures: measures.map((sm) => sm.name),
      filters: filters,
      limit: limit ? limit : this.rows,
      offset: offset ? offset : this.first,
      order: order
    };
  }

  public checkFilterHasChanged(queryObject: any): boolean {
    let filterHasChanged = false;

    if (this.lastAppliedFilters.length !== queryObject.filters.length) {
      filterHasChanged = true;
    } else {
      for (let lastFilterValue of this.lastAppliedFilters) {
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
    return filterHasChanged;
  }

  columnIsMeasure(columnName): boolean {
    return !!this.selectedMeasures.find((m) => columnName.split(',').includes(m.name)); // .split is required because the children dataIndexes of the columns in the pivot table are separated by commas
  }

  cleanUpData() {
    this.selectedDimensions = [];
    this.selectedMeasures = [];
    this.selectedColumns = [];
    this.lastAppliedFilters = [];
    this.lastAppliedColumnFilters = {};
    this.lastLazyLoadEvent = null;
    this.lastAppliedOrder = [];
    this.queryObject = {};
    this.resultSet = null;
    this.isCubeData = null;
  }
}

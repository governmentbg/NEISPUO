import { Injectable } from '@angular/core';
import { CondOperator, RequestQueryBuilder } from '@nestjsx/crud-request';
import { LazyLoadEvent, SelectItem } from 'primeng/api';
import { BehaviorSubject, Observable } from 'rxjs';
import { MunicipalInstitution } from './municipal-institution.interface';

type MIProperty =
  | keyof MunicipalInstitution;

export interface MIListColumn {
  field: MIProperty;
  header: string;
  filter?: {
    type: 'select' | 'text';
    operator: CondOperator;
    selectionOptions?: SelectItem[];
  };
}

@Injectable({
  providedIn: 'root',
})
export class MIListColumnService {
  private localStorageKey = 'submissionListColumnSetting';

  private allColumns: MIListColumn[] = [
    { field: 'RIInstitutionID', header: 'Код по НЕИСПУО' },
    {
      field: 'Name',
      header: 'Наименование',
      filter: {
        type: 'text',
        operator: CondOperator.CONTAINS,
      },
    },
  ];

  private fixedVisibleColumnFields: MIProperty[] = [
    'RIInstitutionID',
    'Name',
  ];

  private hiddenFromStudentColumns: MIProperty[] = [];
  // allColumns excluding fixedVisibleColumnFields

  private _optionallyVisibleColumns = new BehaviorSubject(
    this.allColumns
      .filter((v) => !this.fixedVisibleColumnFields.includes(v.field))
      .map((v) => ({ label: v.header, value: v.field })),
  );

  private _visibleColumns = new BehaviorSubject([] as MIListColumn[]);

  public selectedOptionalColumns: MIProperty[] = null;

  public optionallyVisibleColumns: Observable<
  SelectItem[]
  > = this._optionallyVisibleColumns.asObservable();

  // .pipe(
  //   map(opts =>
  //     // Filter out non-selectable options for student
  //     this.userDataService.userIsStudent
  //       ? opts.filter(opt => !this.hiddenFromStudentColumns.includes(opt.value))
  //       : opts
  //   )
  // );
  public visibleColumns = this._visibleColumns.asObservable();

  constructor() { }

  private valueToTranslatedLabelValue(v: string) {
    // return { label: this.translatePipe.transform(v), value: v };
  }

  /** Loads previous configuration from local storage */
  public reloadVisibleColumns() {
    /**
     * add userId properties
     */
    const userId = 1;
    // Retrieve from local storage
    try {
      const stored: MIProperty[] = JSON.parse(window.localStorage.getItem(this.localStorageKey))[userId] || [];

      // Remove persisted stuff that are no longer valid columns (prevents potential break in future updates)
      const validFields = this.allColumns.map((c) => c.field);
      const sanitized = stored.filter((vf) => validFields.includes(vf));
      this.selectedOptionalColumns = sanitized;
    } catch (error) {
      this.selectedOptionalColumns = [];
    }

    // Display fixedVisible + whatever is on local storage
    const visibleCols = this.allColumns
      .filter(
        (c) => this.fixedVisibleColumnFields.includes(c.field)
          || this.selectedOptionalColumns.includes(c.field),
      );
    this._visibleColumns.next(visibleCols);
  }

  public updateVisibleColumns(fields: string[]) {
    // Get localStorage columns
    let localStoredColumns: { [userId: string]: string[] };
    try {
      localStoredColumns = JSON.parse(window.localStorage.getItem(this.localStorageKey));
    } catch (error) {
      localStoredColumns = {};
    }

    // Update localStorage only for current user
    const userId = 1;
    window.localStorage.setItem(
      this.localStorageKey,
      JSON.stringify({ ...localStoredColumns, [userId]: fields }),
    );

    // Trigger reload
    this.reloadVisibleColumns();
  }

  public createSearchFilters(eventCopy: LazyLoadEvent): any[] {
    const searches = [];
    for (const [key, filter] of Object.entries(eventCopy.filters)) {
      if (!key) {
        continue; // catches not yet implemented fields
      }
      searches.push({
        /**
         * Add hardcoded contains condition
         */
        [key]: { $cont: filter.value || '' },
      });
    }

    return searches;
  }

  public createQueryParams(event: LazyLoadEvent, pageSize:number = 20) {
    const eventCopy = { ...event };
    pageSize = event.rows || pageSize;
    const page = eventCopy.first / pageSize + 1;

    const searches = this.createSearchFilters(eventCopy);

    const qb = RequestQueryBuilder.create()
      .search({
        $and: searches,
      })
      .setPage(page)
      .setLimit(pageSize);
    return qb.queryObject;
  }
}

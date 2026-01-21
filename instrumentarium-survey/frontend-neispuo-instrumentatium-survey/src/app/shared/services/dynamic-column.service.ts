import { Injectable } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { TranslatePipe } from '@shared/modules/pipes/translate.pipe';
import { SelectItem } from 'primeng/api';
import { BehaviorSubject, Observable } from 'rxjs';
import { ColumnServiceConfig } from 'src/app/resources/models/column-service.model';
import { IColumn } from 'src/app/resources/models/column.interface';

@Injectable({
  providedIn: 'root'
})
export class DynamicColumnService<T> {
  private localStorageKey = '';
  private allColumns: IColumn<T>[] = [];
  private fixedVisibleColumnFields: ((keyof T) | any)[] = [];
  private hiddenColumnsFields: ((keyof T) | any)[] = [];

  private _optionallyVisibleColumns = new BehaviorSubject(
    this.allColumns
      .filter(v => !this.fixedVisibleColumnFields.includes(v.field))
      .map(v => ({ label: v.header, value: v.field }))
  );
  private _visibleColumns = new BehaviorSubject([] as IColumn<T>[]);

  public selectedOptionalColumns: ((keyof T) | any)[] = [];
  public optionallyVisibleColumns: Observable<
    SelectItem[]
  > = this._optionallyVisibleColumns.asObservable().pipe(/** dynamic piping here */);
  public visibleColumns = this._visibleColumns.asObservable();

  constructor(private authQuery: AuthQuery, private translatePipe: TranslatePipe) { }

  setConfig(config: ColumnServiceConfig<T>) {
    this.localStorageKey = config.localStorageKey;
    this.allColumns = config.allColumns;
    this.fixedVisibleColumnFields = config.fixedVisibleColumnFields;
    this._optionallyVisibleColumns.next(
      this.allColumns
        .filter(v => !this.fixedVisibleColumnFields.includes(v.field))
        .map(v => ({ label: v.header, value: v.field }))
    );
    this.hiddenColumnsFields = config.hiddenColumnsFields;
  }

  public valueToTranslatedLabelValue(v: string) {
    return { label: this.translatePipe.transform(v), value: v };
  }

  /** Loads previous configuration from local storage */
  public reloadVisibleColumns() {
    // const { userId } = this.authQuery.getValue().mySysUser?.SysId;
    // Retrieve from local storage
    try {
      // const stored: (keyof T)[] =
        // JSON.parse(window.localStorage.getItem(this.localStorageKey))[userId] || [];

      // Remove persisted stuff that are no longer valid columns (prevents potential break in future updates)
      // const validFields = this.allColumns.map(c => c.field);
      // const sanitized = stored.filter(vf => validFields.includes(vf));
      // this.selectedOptionalColumns = sanitized;
    } catch (error) {
      this.selectedOptionalColumns = [];
    }

    // Display fixedVisible + whatever is on local storage
    const visibleCols = this.allColumns
      .filter(
        c =>
          this.fixedVisibleColumnFields.includes(c.field) ||
          this.selectedOptionalColumns.includes(c.field)
      )
      .filter(c => {
        // if student remove irrelevant fields
        return !this.hiddenColumnsFields?.includes(c.field);
      });
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
    // const userId = this.authQuery.getValue().mySysUser.SysId;
    // window.localStorage.setItem(
    //   this.localStorageKey,
    //   JSON.stringify({ ...localStoredColumns, [userId]: fields })
    // );

    // Trigger reload
    this.reloadVisibleColumns();
  }
}

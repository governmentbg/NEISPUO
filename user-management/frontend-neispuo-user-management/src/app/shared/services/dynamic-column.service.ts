import { Injectable } from '@angular/core';
import { SelectItem } from 'primeng/api';
import { BehaviorSubject, Observable } from 'rxjs';
import { ColumnServiceConfig } from '@shared/models/column-service.model';
import { IColumn } from '@shared/models/column.interface';
import { AuthQuery } from '@core/authentication/auth.query';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
    providedIn: 'root',
})
export class DynamicColumnService<T> {
    private localStorageKey = '';

    private allColumns: IColumn<T>[] = [];

    private fixedVisibleColumnFields: (keyof T | any)[] = [];

    private hiddenColumnsFields: (keyof T | any)[] = [];

    private _optionallyVisibleColumns = new BehaviorSubject(
        this.allColumns
            .filter((v) => !this.fixedVisibleColumnFields.includes(v.field))
            .map((v) => ({ label: v.header, value: v.field })),
    );

    private _visibleColumns = new BehaviorSubject([] as IColumn<T>[]);

    public selectedOptionalColumns: (keyof T | any)[] = [];

    public optionallyVisibleColumns: Observable<SelectItem[]> = this._optionallyVisibleColumns
        .asObservable()
        .pipe(/** dynamic piping here */);

    public visibleColumns = this._visibleColumns.asObservable();

    constructor(private translateService: TranslateService, private authQuery: AuthQuery) {}

    setConfig(config: ColumnServiceConfig<T>) {
        this.localStorageKey = config.localStorageKey;
        this.allColumns = config.allColumns;
        this.fixedVisibleColumnFields = config.fixedVisibleColumnFields;
        this._optionallyVisibleColumns.next(
            this.allColumns
                .filter((v) => !this.fixedVisibleColumnFields.includes(v.field))
                .map((v) => ({ label: v.header, value: v.field })),
        );
        this.hiddenColumnsFields = config.hiddenColumnsFields as any[];
    }

    public valueToTranslatedLabelValue(v: string) {
        return { label: this.translateService.instant(v), value: v };
    }

    /** Loads previous configuration from local storage */
    public reloadVisibleColumns() {
        // eslint-disable-next-line no-unsafe-optional-chaining
        const sysUserID = this.authQuery.getValue().selected_role?.SysUserID;
        // // Retrieve from local storage
        try {
            const stored: (keyof T | any)[] =
                JSON.parse(window.localStorage.getItem(this.localStorageKey) as string)[sysUserID as number] || [];
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
                (c) =>
                    this.fixedVisibleColumnFields.includes(c.field) || this.selectedOptionalColumns.includes(c.field),
            )
            .filter(
                (c) =>
                    // if student remove irrelevant fields
                    !this.hiddenColumnsFields?.includes(c.field),
            );
        this._visibleColumns.next(visibleCols);
    }

    public updateVisibleColumns(fields: string[]) {
        // Get localStorage columns
        let localStoredColumns: { [sysUserID: number]: string[] };
        try {
            localStoredColumns = JSON.parse(window.localStorage.getItem(this.localStorageKey) as string);
        } catch (error) {
            localStoredColumns = {};
        }

        // Update localStorage only for current user
        const sysUserID = this.authQuery.getValue().selected_role?.SysUserID;
        window.localStorage.setItem(
            this.localStorageKey,
            JSON.stringify({ ...localStoredColumns, [sysUserID as number]: fields }),
        );

        // Trigger reload
        this.reloadVisibleColumns();
    }
}

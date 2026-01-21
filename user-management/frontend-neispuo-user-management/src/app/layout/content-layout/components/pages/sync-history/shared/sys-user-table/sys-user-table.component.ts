import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { UserResponseDTO } from '@shared/business-object-model/responses/user-response.dto';
import { UserService } from '@shared/services/user.service';
import { Table } from 'primeng/table';
import { CONSTANTS } from 'src/app/shared/constants';
import { finalize } from 'rxjs/operators';
import { SysUserTableColumnsConfig } from './sys-user-table-config';

interface ColumnConfig {
    field: keyof UserResponseDTO;
    header: string;
    width?: string;
}

@Component({
    selector: 'app-sys-user-table',
    templateUrl: './sys-user-table.component.html',
    styleUrls: ['./sys-user-table.component.scss'],
})
export class SysUserTableComponent implements OnChanges {
    @Input() personID!: number;

    users: UserResponseDTO[] = [];

    columnsConfig = SysUserTableColumnsConfig;

    columns: ColumnConfig[] = this.columnsConfig.allColumns as ColumnConfig[];

    loading = false;

    loadingError: string | null = null;

    CONSTANTS = CONSTANTS;

    currentYear = new Date().getFullYear();

    @ViewChild('table') tableRef!: Table;

    constructor(private userService: UserService) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.personID && this.personID != null) {
            this.fetchData();
        }
    }

    private fetchData(): void {
        this.loading = true;
        this.loadingError = null;
        this.userService
            .getSysUsersIDByPersonID(this.personID)
            .pipe(
                finalize(() => {
                    this.loading = false;
                }),
            )
            .subscribe({
                next: (resp) => {
                    const payload: any = (resp as any)?.payload ?? resp;
                    this.users = payload;
                },
                error: (err) => {
                    this.loadingError = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
                },
            });
    }

    reload(): void {
        this.fetchData();
    }

    onDateClear(field: string, operator: string): void {
        if (this.tableRef) {
            this.tableRef.filter('', field, operator);
        }
    }
}

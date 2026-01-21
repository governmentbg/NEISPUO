import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { SyncHistoryService } from 'src/app/layout/content-layout/services/sync-history.service';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { CONSTANTS } from 'src/app/shared/constants';
import { AzureEnrollMentsService } from '@shared/services/azure-enrollments.service';
import { DynamicColumnService } from 'src/app/shared/services/dynamic-column.service';
import { ColumnServiceConfig } from 'src/app/shared/models/column-service.model';
import { IColumn } from 'src/app/shared/models/column.interface';
import { finalize } from 'rxjs/operators';
import { Table } from 'primeng/table';
import { AzureEnrollmentsResponseDTO } from '@shared/business-object-model/responses/azure-enrollments-response.dto';
import { EventStatus } from '@shared/enums/event-status.enum';
import { AzureSyncEnrollmentsTableColumnsConfig } from './azure-sync-enrollments-table-config';
import { AzureSyncTablesConfig } from '../shared/config/azure-sync-tables.config';

@Component({
    selector: 'app-azure-sync-enrollments-table',
    templateUrl: './azure-sync-enrollments-table.component.html',
    styleUrls: ['./azure-sync-enrollments-table.component.scss'],
    providers: [DynamicColumnService],
})
export class AzureSyncEnrollmentsTableComponent implements OnInit, OnChanges {
    @Input() personID!: string;

    @ViewChild('table') tableRef!: Table;

    CONSTANTS = CONSTANTS;

    AZURE_SYNC_TABLES_CONFIG = AzureSyncTablesConfig;

    selectedYears: string[] = ['active'];

    columnsConfig = AzureSyncEnrollmentsTableColumnsConfig;

    columnServiceConfig: ColumnServiceConfig<AzureEnrollmentsResponseDTO> = {
        ...AzureSyncEnrollmentsTableColumnsConfig,
        allColumns: AzureSyncEnrollmentsTableColumnsConfig.allColumns as IColumn<AzureEnrollmentsResponseDTO>[],
        localStorageKey: 'azure-sync-enrollments-table',
    };

    mergedData: AzureEnrollmentsResponseDTO[] = [];

    activeYearData: AzureEnrollmentsResponseDTO[] = [];

    archivedDataCache: { [year: string]: AzureEnrollmentsResponseDTO[] } = {};

    loading = false;

    loadingError: string | null = null;

    enrollmentsLoaded = false;

    currentYear = new Date().getFullYear();

    get visibleColumns$() {
        return this.columnService.visibleColumns;
    }

    get optionallyVisibleColumns$() {
        return this.columnService.optionallyVisibleColumns;
    }

    selectedOptionalColumns: string[] = [];

    constructor(
        private syncHistoryService: SyncHistoryService,
        private messageService: MessageService,
        private translateService: TranslateService,
        private azureEnrollmentsService: AzureEnrollMentsService,
        private columnService: DynamicColumnService<AzureEnrollmentsResponseDTO>,
    ) {}

    ngOnInit(): void {
        this.columnService.setConfig(this.columnServiceConfig);
        this.columnService.reloadVisibleColumns();
        this.selectedOptionalColumns = [...this.columnService.selectedOptionalColumns];
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (
            changes.personID &&
            changes.personID.currentValue &&
            changes.personID.currentValue !== changes.personID.previousValue
        ) {
            this.selectedYears = ['active'];
            this.archivedDataCache = {};
            this.mergedData = [];
            this.activeYearData = [];
            this.loadingError = null;
            this.enrollmentsLoaded = false;
        }
    }

    onLoadEnrollmentsClick(): void {
        this.enrollmentsLoaded = true;
        this.loadActiveYear();
    }

    onYearSelectionChange(years: string[]): void {
        this.selectedYears = years || [];
        const hasActiveYear = this.selectedYears.includes('active');
        const archivedYears = this.selectedYears.filter((y) => y !== 'active');
        if (archivedYears.length > 0) {
            this.loadArchivedYears(archivedYears);
        } else {
            this.mergedData = hasActiveYear ? [...this.activeYearData] : [];
        }
    }

    private loadActiveYear(): void {
        this.loading = true;
        this.loadingError = null;
        this.syncHistoryService
            .getActiveAzureEnrollmentData(Number(this.personID))
            .pipe(
                finalize(() => {
                    this.loading = false;
                }),
            )
            .subscribe({
                next: (arr) => {
                    this.activeYearData = arr;
                    this.loadingError = null;
                    this.mergeData();
                },
                error: () => {
                    this.activeYearData = [];
                    this.loadingError = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
                    this.mergeData();
                },
            });
    }

    private loadArchivedYears(years: string[]): void {
        if (!years || years.length === 0) {
            const hasActiveYear = this.selectedYears.includes('active');
            this.mergedData = hasActiveYear ? [...this.activeYearData] : [];
            return;
        }
        this.loading = true;
        const requests = years
            .filter(
                (y: string) =>
                    this.AZURE_SYNC_TABLES_CONFIG.SELECTABLE_YEARS.some((opt) => opt.value === y) &&
                    !this.archivedDataCache[y],
            )
            .map((year: string) =>
                this.syncHistoryService
                    .getArchivedAzureEnrollmentData({ identifier: this.personID, schoolYear: year })
                    .toPromise()
                    .then((data) => {
                        this.archivedDataCache[year] = data || [];
                    })
                    .catch(() => {
                        this.archivedDataCache[year] = [];
                    }),
            );
        Promise.all(requests).then(() => {
            this.mergeData();
            this.loading = false;
        });
    }

    private mergeData(): void {
        const hasActiveYear = this.selectedYears.includes('active');
        const archivedYears = this.selectedYears.filter((y) => y !== 'active');
        const archived = archivedYears.reduce((acc: AzureEnrollmentsResponseDTO[], year: string) => {
            return acc.concat(this.archivedDataCache[year] || []);
        }, []);
        this.mergedData = hasActiveYear ? [...this.activeYearData, ...archived] : [...archived];
    }

    getEnumLabel(options: { label: string; value: string }[], value: string): string {
        const found = options?.find((opt) => opt.value === value);
        return found ? found.label : value;
    }

    isRestartWorkflowButtonEnabled(status: EventStatus, retryAttempts: number): boolean {
        return this.azureEnrollmentsService.isRestartWorkflowButtonEnabled(status, retryAttempts);
    }

    restartWorkflow(rowID: number): void {
        this.azureEnrollmentsService.restartWorkflow(rowID).subscribe({
            next: () => {
                this.loadActiveYear();
                this.messageService.add({
                    severity: 'success',
                    summary: this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_RESTART_WORKFLOW),
                });
            },
            error: (err) => {
                this.messageService.add({
                    severity: 'error',
                    summary: this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_ERROR),
                    detail: err?.message || 'Unknown error',
                });
            },
        });
    }

    trackByRowID(index: number, item: AzureEnrollmentsResponseDTO) {
        return item.enrollemntID;
    }

    reloadActiveYear(): void {
        if (!this.selectedYears.includes('active')) {
            this.selectedYears = [...this.selectedYears, 'active'];
        }
        this.loadActiveYear();
    }

    onColumnChoiceChange(event: any) {
        this.columnService.updateVisibleColumns(this.selectedOptionalColumns);
    }

    onDateClear(field: string, operator: string): void {
        if (this.tableRef) {
            this.tableRef.filter('', field, operator);
        }
    }
}

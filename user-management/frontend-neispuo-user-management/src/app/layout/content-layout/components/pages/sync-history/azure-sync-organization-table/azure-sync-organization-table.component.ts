import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { SyncHistoryService } from 'src/app/layout/content-layout/services/sync-history.service';
import { AzureOrganizationsService } from 'src/app/shared/services/azure-organizations.service';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { CONSTANTS } from 'src/app/shared/constants';
import { DynamicColumnService } from 'src/app/shared/services/dynamic-column.service';
import { ColumnServiceConfig } from 'src/app/shared/models/column-service.model';
import { IColumn } from 'src/app/shared/models/column.interface';
import { Table } from 'primeng/table';
import { AzureOrganizationResponseDTO } from '@shared/business-object-model/responses/azure-organizations-response.dto';
import { finalize } from 'rxjs/operators';
import { EventStatus } from '@shared/enums/event-status.enum';
import { AzureSyncOrganizationTableColumnsConfig } from './azure-sync-organization-table-config';
import { AzureSyncTablesConfig } from '../shared/config/azure-sync-tables.config';

@Component({
    selector: 'app-azure-sync-organization-table',
    templateUrl: './azure-sync-organization-table.component.html',
    styleUrls: ['./azure-sync-organization-table.component.scss'],
    providers: [DynamicColumnService],
})
export class AzureSyncOrganizationTableComponent implements OnInit, OnChanges {
    @Input() institutionID!: string;

    @ViewChild('table') tableRef!: Table;

    CONSTANTS = CONSTANTS;

    AZURE_SYNC_TABLES_CONFIG = AzureSyncTablesConfig;

    selectedYears: string[] = ['active'];

    columnsConfig = AzureSyncOrganizationTableColumnsConfig;

    columnServiceConfig: ColumnServiceConfig<AzureOrganizationResponseDTO> = {
        ...AzureSyncOrganizationTableColumnsConfig,
        allColumns: AzureSyncOrganizationTableColumnsConfig.allColumns as IColumn<AzureOrganizationResponseDTO>[],
        localStorageKey: 'azure-sync-organization-table',
    };

    mergedData: AzureOrganizationResponseDTO[] = [];

    activeYearData: AzureOrganizationResponseDTO[] = [];

    archivedDataCache: { [year: string]: AzureOrganizationResponseDTO[] } = {};

    loading = false;

    loadingError: string | null = null;

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
        private azureOrganizationsService: AzureOrganizationsService,
        private messageService: MessageService,
        private translateService: TranslateService,
        private columnService: DynamicColumnService<AzureOrganizationResponseDTO>,
    ) {}

    ngOnInit(): void {
        this.columnService.setConfig(this.columnServiceConfig);
        this.columnService.reloadVisibleColumns();
        this.selectedOptionalColumns = [...this.columnService.selectedOptionalColumns];
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (
            changes.institutionID &&
            changes.institutionID.currentValue &&
            changes.institutionID.currentValue !== changes.institutionID.previousValue
        ) {
            this.selectedYears = ['active'];
            this.archivedDataCache = {};
            this.mergedData = [];
            this.activeYearData = [];
            this.loadingError = null;
            this.loadActiveYear();
        }
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
            .getActiveAzureOrganizationData(Number(this.institutionID))
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
                    .getArchivedAzureOrganizationData({ identifier: this.institutionID, schoolYear: year })
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
        const archived = archivedYears.reduce((acc: AzureOrganizationResponseDTO[], year: string) => {
            return acc.concat(this.archivedDataCache[year] || []);
        }, []);
        this.mergedData = hasActiveYear ? [...this.activeYearData, ...archived] : [...archived];
    }

    getEnumLabel(options: { label: string; value: string }[], value: string): string {
        const found = options?.find((opt) => opt.value === value);
        return found ? found.label : value;
    }

    isRestartWorkflowButtonEnabled(status: EventStatus, retryAttempts: number): boolean {
        return this.azureOrganizationsService.isRestartWorkflowButtonEnabled(status, retryAttempts);
    }

    restartWorkflow(rowID: number): void {
        this.azureOrganizationsService.restartWorkflow(rowID).subscribe({
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

    trackByOrganizationID(index: number, item: AzureOrganizationResponseDTO) {
        return item.organizationID;
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

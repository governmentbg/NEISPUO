import { WORKFLOW_TYPES, EVENT_STATUSES } from 'src/app/configs/datatables-config';

export const AzureSyncEnrollmentsTableColumnsConfig = {
    allColumns: [
        { field: 'rowID', header: 'Идентификатор', filter: { type: 'text', operator: 'contains' } },
        {
            field: 'workflowType',
            header: 'Тип',
            filter: { type: 'select', operator: 'equals', selectionOptions: WORKFLOW_TYPES },
        },
        { field: 'classAzureID', header: 'Клас', filter: { type: 'text', operator: 'contains' } },
        { field: 'organizationAzureID', header: 'Училище', filter: { type: 'text', operator: 'contains' } },
        { field: 'userAzureID', header: 'Потребител', filter: { type: 'text', operator: 'contains' } },
        { field: 'createdOn', header: 'Дата създаване', filter: { type: 'date', operator: 'dateIs' } },
        { field: 'guid', header: 'GUID', filter: { type: 'text', operator: 'contains' } },
        { field: 'retryAttempts', header: 'Брой опити', filter: { type: 'text', operator: 'contains' } },
        { field: 'errorMessage', header: 'Грешка', filter: { type: 'text', operator: 'contains' } },
        {
            field: 'status',
            header: 'Статус',
            filter: { type: 'select', operator: 'equals', selectionOptions: EVENT_STATUSES },
        },
        { field: 'options', header: 'Контроли', notSortable: true },
    ],
    fixedVisibleColumnFields: [
        'workflowType',
        'classAzureID',
        'organizationAzureID',
        'userAzureID',
        'createdOn',
        'guid',
        'retryAttempts',
        'status',
        'options',
    ],
    localStorageKey: 'azure-sync-enrollments-table',
};

import { WORKFLOW_TYPES, EVENT_STATUSES } from 'src/app/configs/datatables-config';

export const AzureSyncOrganizationTableColumnsConfig = {
    allColumns: [
        { field: 'organizationID', header: 'Номер', filter: { type: 'text', operator: 'contains' } },
        {
            field: 'workflowType',
            header: 'Тип',
            filter: { type: 'select', operator: 'equals', selectionOptions: WORKFLOW_TYPES },
        },
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
        'organizationID',
        'createdOn',
        'guid',
        'retryAttempts',
        'status',
        'options',
    ],
    localStorageKey: 'azure-sync-organization-table',
};

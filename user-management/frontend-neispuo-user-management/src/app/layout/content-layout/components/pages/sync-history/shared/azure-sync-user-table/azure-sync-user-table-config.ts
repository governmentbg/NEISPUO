import { WORKFLOW_TYPES, EVENT_STATUSES } from 'src/app/configs/datatables-config';

export const AzureSyncUserTableColumnsConfig = {
    allColumns: [
        { field: 'userID', header: 'Идентификатор', width: '10%', filter: { type: 'text', operator: 'contains' } },
        {
            field: 'workflowType',
            header: 'Тип',
            width: '12%',
            filter: { type: 'select', operator: 'equals', selectionOptions: WORKFLOW_TYPES },
        },
        { field: 'createdOn', header: 'Дата създаване', width: '14%', filter: { type: 'date', operator: 'dateIs' } },
        { field: 'guid', header: 'GUID', width: '18%', filter: { type: 'text', operator: 'contains' } },
        {
            field: 'retryAttempts',
            header: 'Брой опити',
            width: '8%',
            filter: { type: 'text', operator: 'contains' },
        },
        {
            field: 'status',
            header: 'Статус',
            width: '10%',
            filter: { type: 'select', operator: 'equals', selectionOptions: EVENT_STATUSES },
        },
        { field: 'errorMessage', header: 'Грешка', width: '22%', filter: { type: 'text', operator: 'contains' } },
        { field: 'options', header: 'Контроли', width: '16%', notSortable: true },
    ],
    fixedVisibleColumnFields: ['workflowType', 'createdOn', 'guid', 'retryAttempts', 'status', 'options'],
};

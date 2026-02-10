import { SelectItem } from 'primeng/api';

export const IS_AZURE_USER_OPTIONS: SelectItem<boolean | null>[] = [
    { label: 'PRIMENG.DROPDOWN.PLACEHOLDER', value: null },
    { label: 'YES', value: true },
    { label: 'NO', value: false },
];

export const IS_AZURE_SYNCED_OPTIONS: SelectItem<1 | 0 | null>[] = [
    { label: 'PRIMENG.DROPDOWN.PLACEHOLDER', value: null },
    { label: 'YES', value: 1 },
    { label: 'NO', value: 0 },
];

export const SysUserTableColumnsConfig = {
    allColumns: [
        { field: 'sysUserID', header: 'ID (Сис. потребител)', filter: { type: 'text', operator: 'contains' } },
        { field: 'username', header: 'Потребителско име', filter: { type: 'text', operator: 'contains' } },
        {
            field: 'isAzureUser',
            header: 'Azure потребител',
            filter: { type: 'select', operator: 'equals', selectionOptions: IS_AZURE_USER_OPTIONS },
        },
        {
            field: 'isAzureSynced',
            header: 'Синхронизиран',
            filter: { type: 'select', operator: 'equals', selectionOptions: IS_AZURE_SYNCED_OPTIONS },
        },
        { field: 'personID', header: 'Идентификатор', filter: { type: 'text', operator: 'contains' } },
        { field: 'deletedOn', header: 'Изтрит на', filter: { type: 'date', operator: 'dateIs' } },
    ],
};

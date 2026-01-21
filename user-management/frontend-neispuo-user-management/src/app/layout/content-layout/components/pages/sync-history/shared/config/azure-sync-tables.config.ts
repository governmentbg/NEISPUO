export class AzureSyncTablesConfig {
    static readonly SELECTABLE_YEARS = [
        { label: 'Активна година', value: 'active' },
        ...Array.from({ length: new Date().getFullYear() - 2021 }, (_, i) => {
            const year = new Date().getFullYear() - 1 - i;

            return { label: year.toString(), value: year.toString() };
        }),
    ];

    static readonly DEFAULT_ROWS_COUNT = 10;

    static readonly ROWS_PER_PAGE_OPTIONS = [5, 10, 20, 50];
}

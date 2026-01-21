import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { EntityType, SyncHistoryEntityEnum } from '../types/sync-history.type';
import { InstitutionsTableEntity } from './interfaces/institutions-table-entity.interface';
import { Person } from './interfaces/person.interface';

@Component({
    selector: 'app-neispuo-profile-card',
    templateUrl: './neispuo-profile-card.component.html',
    styleUrls: ['./neispuo-profile-card.component.scss'],
})
export class NeispuoProfileCardComponent implements OnChanges {
    constructor(private translateService: TranslateService) {}

    @Input() data: Partial<Person | InstitutionsTableEntity> | null = null;

    @Input() entityType: EntityType = SyncHistoryEntityEnum.STUDENT;

    @Input() loading = false;

    @Input() error: string | null = null;

    @Input() empty = false;

    @Output() refresh = new EventEmitter<void>();

    processedData: Record<string, unknown> = {};

    onRefreshClick() {
        if (!this.loading) {
            this.refresh.emit();
        }
    }

    private readonly personFieldLabels: { key: keyof Person; label: string }[] = [
        { key: 'personID', label: 'Идентификатор (Person ID)' },
        { key: 'azureID', label: 'Azure ID' },
        { key: 'publicEduNumber', label: 'ЛОН' },
        { key: 'firstName', label: 'Малко Име' },
        { key: 'middleName', label: 'Бащино Име' },
        { key: 'lastName', label: 'Фамилия' },
        { key: 'personalID', label: 'ЕГН/ЛНЧ' },
        { key: 'personalIDType', label: 'Тип Личен Документ' },
        { key: 'birthDate', label: 'Дата на Раждане' },
        { key: 'birthPlace', label: 'Място на Раждане' },
        { key: 'permanentAddress', label: 'Постоянен Адрес' },
        { key: 'currentAddress', label: 'Настоящ Адрес' },
        { key: 'permanentTownID', label: 'Идентификатор на Постоянен Град' },
        { key: 'currentTownID', label: 'Идентификатор на Настоящ Град' },
        { key: 'nationalityID', label: 'Идентификатор на Националност' },
        { key: 'birthPlaceTownID', label: 'Идентификатор на Град на Раждане' },
        { key: 'birthPlaceCountry', label: 'Държава на Раждане' },
        { key: 'gender', label: 'Пол' },
        { key: 'schoolBooksCodesID', label: 'Идентификатор на Кодове за Дневник' },
        { key: 'sysUserType', label: 'Тип Системен Потребител' },
    ];

    private readonly institutionFieldLabels: { key: keyof InstitutionsTableEntity; label: string }[] = [
        { key: 'institutionID', label: 'Идентификатор (Institution ID)' },
        { key: 'institutionName', label: 'Име на Институция' },
        { key: 'username', label: 'Потребителско Име' },
        { key: 'sysUserID', label: 'Идентификатор на Системен Потребител' },
        { key: 'sysRoleID', label: 'Идентификатор на Роля в Системата' },
        { key: 'isAzureUser', label: 'Потребител в Azure' },
        { key: 'financialSchoolTypeName', label: 'Тип на Финансиране' },
        { key: 'baseSchoolTypeName', label: 'Основен Тип на Училището' },
        { key: 'detailedSchoolTypeName', label: 'Подробен Тип на Училището' },
        { key: 'townName', label: 'Име на Град' },
        { key: 'municipalityID', label: 'Идентификатор на Община' },
        { key: 'municipalityName', label: 'Име на Община' },
        { key: 'regionID', label: 'Идентификатор на Област' },
        { key: 'regionName', label: 'Име на Област' },
    ];

    get fieldLabels(): { key: string; label: string }[] {
        if (this.entityType === SyncHistoryEntityEnum.INSTITUTION) {
            return this.institutionFieldLabels;
        }
        return this.personFieldLabels;
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.data) {
            this.buildProcessedData();
        }
    }

    private buildProcessedData(): void {
        this.processedData = {};

        if (!this.data) return;

        for (const field of this.fieldLabels) {
            const raw = (this.data as Record<string, unknown>)[field.key];

            if (typeof raw === 'boolean') {
                const key = raw ? 'YES' : 'NO';
                this.processedData[field.key] = this.translateService.instant(key);
            } else {
                this.processedData[field.key] = raw ?? '';
            }
        }
    }
}

import { Component, Input, OnInit } from '@angular/core';
import { OrganizationInfoResponseDTO } from '@shared/business-object-model/responses/organization-info-response.dto';
import { SyncHistoryService } from 'src/app/layout/content-layout/services/sync-history.service';
import { CONSTANTS } from '@shared/constants';
import { TranslateService } from '@ngx-translate/core';
import { InstitutionsTableEntity } from '../shared/neispuo-profile-card/interfaces/institutions-table-entity.interface';
import { SyncHistoryEntityEnum } from '../shared/types/sync-history.type';

@Component({
    selector: 'app-organization-history',
    templateUrl: './organization-history.component.html',
    styleUrls: ['./organization-history.component.scss'],
})
export class OrganizationHistoryComponent implements OnInit {
    CONSTANTS = CONSTANTS;

    @Input() institutionID!: string;

    SyncHistoryEntityEnum = SyncHistoryEntityEnum;

    neispuoProfile: InstitutionsTableEntity | null = null;

    neispuoLoading = false;

    neispuoError: string | null = null;

    neispuoEmpty = false;

    entityInfo: OrganizationInfoResponseDTO | null = null;

    entityLoading = false;

    entityError: string | null = null;

    entityEmpty = false;

    constructor(private syncHistoryService: SyncHistoryService, private translate: TranslateService) {}

    ngOnInit() {
        this.loadNeispuoProfile();
        this.loadEntityInfo();
    }

    loadNeispuoProfile() {
        this.neispuoLoading = true;
        this.neispuoError = null;
        this.syncHistoryService.getInstitutionById(this.institutionID).subscribe({
            next: (data: InstitutionsTableEntity) => {
                this.neispuoProfile = data;
                this.neispuoEmpty = !data || Object.keys(data).length === 0;
                this.neispuoLoading = false;
            },
            error: (err) => {
                this.neispuoError = this.translate.instant(CONSTANTS.PRIMENG_ERROR_LOADING_DATA);
                this.neispuoProfile = null;
                this.neispuoEmpty = false;
                this.neispuoLoading = false;
            },
        });
    }

    loadEntityInfo() {
        this.entityLoading = true;
        this.entityError = null;
        this.syncHistoryService.getAzureOrganizationInfo({ schoolId: +this.institutionID }).subscribe({
            next: (data) => {
                this.entityInfo = data;
                this.entityEmpty = !data || Object.keys(data).length === 0;
                this.entityLoading = false;
            },
            error: (err) => {
                this.entityError = this.translate.instant(CONSTANTS.PRIMENG_ERROR_LOADING_DATA);
                this.entityInfo = null;
                this.entityEmpty = false;
                this.entityLoading = false;
            },
        });
    }
}

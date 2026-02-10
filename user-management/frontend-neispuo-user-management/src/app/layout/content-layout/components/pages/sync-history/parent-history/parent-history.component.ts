import { Component, Input, OnInit } from '@angular/core';
import { ParentInfoResponseDTO } from '@shared/business-object-model/responses/parent-info-response.dto';
import { PersonResponseDTO } from '@shared/business-object-model/responses/person-response.dto';
import { SyncHistoryService } from 'src/app/layout/content-layout/services/sync-history.service';
import { CONSTANTS } from '@shared/constants';
import { UserService } from '@shared/services/user.service';
import { TranslateService } from '@ngx-translate/core';
import { SyncHistoryEntityEnum } from '../shared/types/sync-history.type';

@Component({
    selector: 'app-parent-history',
    templateUrl: './parent-history.component.html',
    styleUrls: ['./parent-history.component.scss'],
})
export class ParentHistoryComponent implements OnInit {
    CONSTANTS = CONSTANTS;

    @Input() personID!: string;

    SyncHistoryEntityEnum = SyncHistoryEntityEnum;

    neispuoProfile: PersonResponseDTO | null = null;

    neispuoLoading = false;

    neispuoError: string | null = null;

    neispuoEmpty = false;

    entityInfo: ParentInfoResponseDTO | null = null;

    entityLoading = false;

    entityError: string | null = null;

    entityEmpty = false;

    constructor(
        private syncHistoryService: SyncHistoryService,
        private userService: UserService,
        private translate: TranslateService,
    ) {}

    ngOnInit() {
        this.loadNeispuoProfile();
        this.loadEntityInfo();
    }

    loadNeispuoProfile() {
        this.neispuoLoading = true;
        this.neispuoError = null;
        this.userService.getPersonByPersonId(this.personID).subscribe({
            next: (data) => {
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
        this.userService.getPersonByPersonId(this.personID).subscribe({
            next: (person) => {
                const email = person?.publicEduNumber;
                if (!email) {
                    this.entityInfo = null;
                    this.entityEmpty = true;
                    this.entityLoading = false;
                    return;
                }
                this.syncHistoryService.getAzureParentInfo({ email }).subscribe({
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

import { Component, Input, OnInit } from '@angular/core';
import { PersonResponseDTO } from '@shared/business-object-model/responses/person-response.dto';
import { UserInfoResponseDTO } from '@shared/business-object-model/responses/user-info-response.dto';
import { SyncHistoryService } from 'src/app/layout/content-layout/services/sync-history.service';
import { CONSTANTS } from '@shared/constants';
import { GetUserInfoRequestDTO } from '@shared/business-object-model/requests/get-user-info-request.dto';
import { UserService } from '@shared/services/user.service';
import { TranslateService } from '@ngx-translate/core';
import { SyncHistoryEntityEnum, UserLoadMethodState } from '../shared/types/sync-history.type';

@Component({
    selector: 'app-user-history',
    templateUrl: './user-history.component.html',
    styleUrls: ['./user-history.component.scss'],
})
export class UserHistoryComponent implements OnInit {
    CONSTANTS = CONSTANTS;

    @Input() personID!: string;

    SyncHistoryEntityEnum = SyncHistoryEntityEnum;

    neispuoProfile: PersonResponseDTO | null = null;

    neispuoLoading = false;

    neispuoError: string | null = null;

    neispuoEmpty = false;

    entityInfo: UserInfoResponseDTO | null = null;

    entityLoading = false;

    entityError: string | null = null;

    entityEmpty = false;

    userLoadMethodState: UserLoadMethodState = {
        publicEduNumber: null,
        azureID: null,
        loadMethod: 'publicEduNumber',
    };

    noIdentifiersAvailable = false;

    constructor(
        private syncHistoryService: SyncHistoryService,
        private userService: UserService,
        private translate: TranslateService,
    ) {}

    ngOnInit() {
        this.initialLoad();
    }

    initialLoad() {
        this.neispuoLoading = true;
        this.entityLoading = true;
        this.neispuoError = null;
        this.entityError = null;

        this.userService.getPersonByPersonId(this.personID).subscribe({
            next: (person: PersonResponseDTO) => {
                this.setProfileAndIdentifiers(person);
                this.neispuoLoading = false;
                this.loadEntityInfo();
            },
            error: () => {
                this.neispuoError = this.translate.instant(CONSTANTS.PRIMENG_ERROR_LOADING_DATA);
                this.neispuoProfile = null;
                this.neispuoEmpty = false;
                this.neispuoLoading = false;

                this.entityError = this.translate.instant(CONSTANTS.PRIMENG_ERROR_LOADING_DATA);
                this.entityInfo = null;
                this.entityEmpty = false;
                this.entityLoading = false;
            },
        });
    }

    loadPersonData() {
        this.neispuoLoading = true;
        this.neispuoError = null;
        this.userService.getPersonByPersonId(this.personID).subscribe({
            next: (person: PersonResponseDTO) => {
                this.setProfileAndIdentifiers(person);
                this.neispuoLoading = false;
            },
            error: () => {
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
        if (this.noIdentifiersAvailable) {
            this.entityInfo = null;
            this.entityEmpty = false;
            this.entityLoading = false;
            this.entityError = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
            return;
        }
        const payload = this.buildEntityInfoPayload();
        if (payload) {
            this.syncHistoryService.getAzureUserInfo(payload).subscribe({
                next: (data: UserInfoResponseDTO) => {
                    this.entityInfo = data;
                    this.entityEmpty = !data || Object.keys(data).length === 0;
                    this.entityLoading = false;
                },
                error: () => {
                    this.entityInfo = null;
                    this.entityEmpty = false;
                    this.entityLoading = false;
                    this.entityError = this.translate.instant(CONSTANTS.PRIMENG_ERROR_LOADING_DATA);
                },
            });
        } else {
            this.entityInfo = null;
            this.entityEmpty = false;
            this.entityLoading = false;
            this.entityError = this.translate.instant(CONSTANTS.PRIMENG_ERROR_LOADING_DATA);
        }
    }

    private setProfileAndIdentifiers(person: PersonResponseDTO) {
        this.neispuoProfile = person;
        this.neispuoEmpty = !person || Object.keys(person).length === 0;
        this.userLoadMethodState.publicEduNumber = person?.publicEduNumber || null;
        this.userLoadMethodState.azureID = person?.azureID || null;
        if (this.userLoadMethodState.publicEduNumber) {
            this.userLoadMethodState.loadMethod = 'publicEduNumber';
        } else if (this.userLoadMethodState.azureID) {
            this.userLoadMethodState.loadMethod = 'azureID';
        } else {
            this.userLoadMethodState.loadMethod = 'publicEduNumber';
        }
        this.noIdentifiersAvailable = !this.userLoadMethodState.publicEduNumber && !this.userLoadMethodState.azureID;
    }

    onUserLoadMethodStateChange(newState: UserLoadMethodState) {
        this.userLoadMethodState = { ...newState };
        this.loadEntityInfo();
    }

    private buildEntityInfoPayload(): GetUserInfoRequestDTO | null {
        if (this.userLoadMethodState.loadMethod === 'azureID' && this.userLoadMethodState.azureID) {
            return { azureId: this.userLoadMethodState.azureID };
        }
        if (this.userLoadMethodState.loadMethod === 'publicEduNumber' && this.userLoadMethodState.publicEduNumber) {
            return { publicEduNumber: this.userLoadMethodState.publicEduNumber };
        }
        return null;
    }
}

import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CONSTANTS } from '@shared/constants';
import { EntityType, SyncHistoryEntityEnum, UserLoadMethodState } from '../types/sync-history.type';

@Component({
    selector: 'app-profile-tabbar',
    templateUrl: './profile-tabbar.component.html',
    styleUrls: ['./profile-tabbar.component.scss'],
})
export class ProfileTabbarComponent {
    CONSTANTS = CONSTANTS;

    SyncHistoryEntityEnum = SyncHistoryEntityEnum;

    @Input() entityType!: EntityType;

    @Input() entityInfo: any;

    @Input() loading = false;

    @Input() error: string | null = null;

    @Input() empty = false;

    @Input() userLoadMethodState: UserLoadMethodState | null = null;

    @Output() userLoadMethodStateChange = new EventEmitter<UserLoadMethodState>();

    @Output() refresh = new EventEmitter<void>();

    onRefresh() {
        this.refresh.emit();
    }

    isUserEntity(): boolean {
        return this.entityType === SyncHistoryEntityEnum.STUDENT || this.entityType === SyncHistoryEntityEnum.TEACHER;
    }

    onLoadMethodChange(newMethod: 'azureID' | 'publicEduNumber') {
        if (this.userLoadMethodState) {
            this.userLoadMethodStateChange.emit({
                ...this.userLoadMethodState,
                loadMethod: newMethod,
            });
        }
    }
}

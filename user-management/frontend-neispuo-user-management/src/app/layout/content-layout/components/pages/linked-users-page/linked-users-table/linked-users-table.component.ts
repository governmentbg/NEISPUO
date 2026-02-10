import { Component, Input, Output, EventEmitter, TemplateRef } from '@angular/core';
import { LinkedUserResponseDTO } from '@shared/business-object-model/responses/linked-user-response.dto';
import { ConfirmationService } from 'primeng/api';
import { CONSTANTS } from '@shared/constants';
import { TranslateService } from '@ngx-translate/core';
import { LINKED_USERS_TABLE_COLUMNS, LINKED_USERS_TABLE_ROWS } from './linked-users-table.config';

@Component({
    selector: 'app-linked-users-table',
    templateUrl: './linked-users-table.component.html',
    styleUrls: ['./linked-users-table.component.scss'],
    providers: [ConfirmationService],
})
export class LinkedUsersTableComponent {
    CONSTANTS = CONSTANTS;

    @Input() data: LinkedUserResponseDTO[] = [];

    @Input() loading = false;

    @Input() controlsTemplate!: TemplateRef<any>;

    @Input() noDataText: string = 'Няма намерени данни.';

    @Output() actionConfirmed = new EventEmitter<{ row: LinkedUserResponseDTO; action: string }>();

    columns = LINKED_USERS_TABLE_COLUMNS;

    rows = LINKED_USERS_TABLE_ROWS;

    constructor(private confirmationService: ConfirmationService, private translate: TranslateService) {}

    confirmAction(event: Event, row: LinkedUserResponseDTO, action: string) {
        this.confirmationService.confirm({
            target: event.target as HTMLElement,
            message: this.translate.instant(CONSTANTS.PRIMENG_CONFIRM_DIALOG_MESSAGE_ARE_YOU_SURE),
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: this.translate.instant(CONSTANTS.PRIMENG_CONFIRM_DIALOG_BUTTON_CONFIRM),
            rejectLabel: this.translate.instant(CONSTANTS.PRIMENG_CONFIRM_DIALOG_BUTTON_DECLINE),
            accept: () => {
                this.actionConfirmed.emit({ row, action });
            },
            reject: () => {},
        });
    }
}

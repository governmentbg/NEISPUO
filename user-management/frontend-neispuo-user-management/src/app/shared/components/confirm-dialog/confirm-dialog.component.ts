import { Component } from '@angular/core';
import { CONSTANTS } from '@shared/constants';

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './confirm-dialog.component.html',
    styleUrls: ['./confirm-dialog.component.scss'],
})
export class ConfirmDialogComponent {
    private TITLE = CONSTANTS.PRIMENG_CONFIRM_DIALOG_HEADER;

    private CONFIRM = CONSTANTS.PRIMENG_CONFIRM_DIALOG_BUTTON_CONFIRM;

    private DECLINE = CONSTANTS.PRIMENG_CONFIRM_DIALOG_BUTTON_DECLINE;
}

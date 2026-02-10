import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CONSTANTS } from '@shared/constants';
import { ConfirmationService, MessageService } from 'primeng/api';
import { StudentUsersService } from './student-users.service';

@Injectable({
    providedIn: 'root',
})
export class ConfirmUpdateCodeDialogService {
    constructor(
        private confirmationService: ConfirmationService,
        private messageService: MessageService,
        private studentUsersService: StudentUsersService,
        private translateService: TranslateService,
    ) {}

    updateSchoolBookCode(personID: number, componentReference: any) {
        componentReference.loading = true;
        this.confirmationService.confirm({
            message: this.translateService.instant(CONSTANTS.PRIMENG_CONFIRM_DIALOG_MESSAGE_UPDATE_SCHOOL_BOOK_CODE),
            accept: () => {
                componentReference.subSink.sink = this.studentUsersService.assignSchoolBookCode(personID).subscribe(
                    (response) => {
                        this.printSuccessMessage();
                        componentReference.loadByUrl();
                    },
                    (error) => {
                        console.log(error);
                        componentReference.loading = false;
                        this.printErrorMessage();
                    },
                );
            },
            reject: () => {
                componentReference.loading = false;
            },
        });
    }

    printSuccessMessage() {
        this.messageService.add({
            severity: 'success',
            summary: this.translateService.instant(CONSTANTS.PRIMENG_CONFIRM_DIALOG_SUMMARY_UPDATE_SCHOOL_BOOK_CODE),
        });
    }

    printErrorMessage() {
        this.studentUsersService.printErrorMessage();
    }
}

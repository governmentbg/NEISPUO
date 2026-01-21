/* eslint-disable no-multi-assign, no-unused-expressions */
import { Component, OnInit } from '@angular/core';
import { AvailableSchoolBooksResponseDTO } from '@shared/business-object-model/responses/available-school-books-response.dto';
import { CONSTANTS } from '@shared/constants';
import { SchoolBookAccessService } from 'src/app/layout/content-layout/components/pages/school-books-access/state/school-book-access.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Observable } from 'rxjs';
import { finalize, map, switchMap } from 'rxjs/operators';
import { TransformedSchoolBookAccess } from '@shared/business-object-model/interfaces/transformed-school-book-access.interface';
import { TranslateService } from '@ngx-translate/core';
import { SchoolBooksAccessQuery } from '../state/school-books-access.query';

@Component({
    selector: 'app-add-school-book-access',
    templateUrl: './add-school-book-access.component.html',
    styleUrls: ['./add-school-book-access.component.scss'],
})
export class AddSchoolBookAccessComponent implements OnInit {
    availableSchoolBooksOptions: any[] = [];

    personSchoolBooks: AvailableSchoolBooksResponseDTO[] = [];

    selectedSchoolBooks: [number, number][] = [];

    transformedPersonSchoolBooks: any[] = [];

    personId = this.schoolBookAccessQuery.getValue().personId;

    isLoading: boolean = false;

    availableSchoolBooks$: Observable<TransformedSchoolBookAccess[]> = this.schoolBookAccessService
        .getAvailableSchoolBooks()
        .pipe(
            map((schoolBooks: AvailableSchoolBooksResponseDTO[]) => {
                const transformedSchoolBooks = schoolBooks.map((sb) => ({
                    label: `${sb.schoolYear} - ${sb.fullBookName}`,
                    code: [sb.schoolYear, sb.classBookID],
                }));
                return transformedSchoolBooks as TransformedSchoolBookAccess[];
            }),
        );

    CONSTANTS = CONSTANTS;

    constructor(
        public ref: DynamicDialogRef,
        private schoolBookAccessService: SchoolBookAccessService,
        public config: DynamicDialogConfig,
        private schoolBookAccessQuery: SchoolBooksAccessQuery,
        private translateService: TranslateService,
    ) {}

    // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
    ngOnInit() {}

    async saveSchoolBooksAccess() {
        this.isLoading = true;
        this.schoolBookAccessService
            .giveSchoolBookAccessToPerson(this.selectedSchoolBooks)
            .pipe(
                switchMap(() => this.schoolBookAccessService.setPersonalSchoolBooks()),
                finalize(() => {
                    this.isLoading = false;
                    this.closeDialog();
                }),
            )
            .subscribe({
                next: (success) => {
                    this.schoolBookAccessService.showSuccess(
                        this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_GIVE_SCHOOL_BOOK_ACCESS),
                    );
                },
                error: (error) => {
                    this.schoolBookAccessService.showError(error.message);
                },
            });
    }

    closeDialog() {
        this.ref.close(this.personSchoolBooks);
    }
}

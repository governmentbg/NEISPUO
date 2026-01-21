import { Component, OnInit } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { TranslateService } from '@ngx-translate/core';
import { PersonSchoolBookResponseDTO } from '@shared/business-object-model/responses/person-school-book-response.dto';
import { CONSTANTS } from '@shared/constants';
import { SchoolBookAccessService } from 'src/app/layout/content-layout/components/pages/school-books-access/state/school-book-access.service';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { finalize, switchMap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { SubSink } from 'subsink';
import { AddSchoolBookAccessComponent } from '../school-books-addition-modal/add-school-book-access.component';
import { SchoolBooksAccessQuery } from '../state/school-books-access.query';

@Component({
    selector: 'app-school-books-access',
    templateUrl: './school-books-access-page.component.html',
    styleUrls: ['./school-books-access-page.component.scss'],
})
export class SchoolBooksAccessPageComponent implements OnInit {
    selectedDeletionSchoolBooks: PersonSchoolBookResponseDTO[] = [];

    isDeleteMode: boolean = false;

    ref!: DynamicDialogRef;

    CONSTANTS = CONSTANTS;

    personalSchoolBooks$ = this.schoolBooksAccessQuery.personalSchoolBooks$;

    hasAdminAccessOptions = [
        { label: this.translateService.instant(CONSTANTS.NO), value: 0 },
        { label: this.translateService.instant(CONSTANTS.YES), value: 1 },
    ];

    isLoading: boolean = false;

    private subs = new SubSink();

    constructor(
        public apiService: ApiService,
        private confirmationService: ConfirmationService,
        private dialogService: DialogService,
        private schoolBookAccessService: SchoolBookAccessService,
        private translateService: TranslateService,
        private schoolBooksAccessQuery: SchoolBooksAccessQuery,
        private router: ActivatedRoute,
    ) {}

    ngOnInit() {
        this.schoolBookAccessService.personId = this.router.snapshot.params.personId;
        this.schoolBookAccessService.institutionId = this.router.snapshot.params.institutionId;
        this.subs.add(this.schoolBookAccessService.setPersonalSchoolBooks().subscribe());
    }

    openAddNewSchoolBookDialog() {
        this.ref = this.dialogService.open(AddSchoolBookAccessComponent, {
            header: this.translateService.instant(CONSTANTS.PRIMENG_DYNAMIC_DIALOG_SELECT_SCHOOL_BOOKS_HEADER),
            width: '50%',
            closable: false,
        });
    }

    deleteSelectedAccesses() {
        this.confirmationService.confirm({
            message: this.translateService.instant(
                CONSTANTS.PRIMENG_CONFIRM_DIALOG_MESSAGE_DELETE_SELECTED_SCHOOL_BOOK_ACCESSES,
            ),
            accept: async () => {
                const selectedSchoolBooksRowIDs = this.selectedDeletionSchoolBooks.map(
                    (schoolBook) => schoolBook?.rowID,
                ) as number[];
                this.schoolBookAccessService
                    .deleteSelectedSchoolBookAccesses(selectedSchoolBooksRowIDs)
                    .pipe(switchMap(() => this.schoolBookAccessService.setPersonalSchoolBooks()))
                    .subscribe({
                        next: () => {
                            this.schoolBookAccessService.showSuccess(
                                this.translateService.instant(
                                    CONSTANTS.PRIMENG_TOASTR_SUMMARY_DELETE_SCHOOL_BOOK_ACCESS,
                                ),
                            );
                            this.selectedDeletionSchoolBooks = [];
                        },
                        error: (error) => {
                            this.schoolBookAccessService.showError(error.message);
                        },
                    });
            },
        });
    }

    updateAdminAccess(event: any, dto: PersonSchoolBookResponseDTO) {
        this.isLoading = true;
        dto.hasAdminAccess = event.value;
        this.schoolBookAccessService
            .updateAdminAccess(dto)
            .pipe(
                finalize(() => {
                    this.isLoading = false;
                }),
            )
            .subscribe({
                next: () => {
                    this.schoolBookAccessService.showSuccess(
                        this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_UPDATE_SCHOOL_BOOK_ACCESS),
                    );
                },
                error: (error) => {
                    this.schoolBookAccessService.showError(error.message);
                },
            });
    }

    get isDeleteSchoolBooksButtonEnabled() {
        return this.selectedDeletionSchoolBooks?.length > 0;
    }
}

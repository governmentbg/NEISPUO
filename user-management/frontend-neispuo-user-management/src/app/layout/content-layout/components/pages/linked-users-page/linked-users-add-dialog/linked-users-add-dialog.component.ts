import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { of, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap, catchError } from 'rxjs/operators';
import { LinkedUserResponseDTO } from '@shared/business-object-model/responses/linked-user-response.dto';
import { ParentService } from '@shared/services/parent.service';
import { StudentUsersService } from '@shared/services/student-users.service';
import { MessageService } from 'primeng/api';
import { AccessUpsertRequestDTO } from '@shared/business-object-model/requests/access-upsert-request.dto';
import { CONSTANTS } from '@shared/constants';
import { TranslateService } from '@ngx-translate/core';
import { LinkedUserType } from '../shared/linked-users-page.enums';

@Component({
    selector: 'app-linked-users-add-dialog',
    templateUrl: './linked-users-add-dialog.component.html',
    styleUrls: ['./linked-users-add-dialog.component.scss'],
})
export class LinkedUsersAddDialogComponent implements OnInit, OnDestroy {
    CONSTANTS = CONSTANTS;

    @Input() visible = false;

    @Input() userType!: LinkedUserType;

    @Input() title = '';

    @Input() personID!: string;

    @Output() visibleChange = new EventEmitter<boolean>();

    @Output() addUser = new EventEmitter<LinkedUserResponseDTO>();

    searchMode: 'publicEduNumber' | 'personId' = 'personId';

    searchControl = new FormControl('');

    searchLoading = false;

    searchResults: LinkedUserResponseDTO[] = [];

    searchError: string | null = null;

    private searchSub?: Subscription;

    LinkedUserType = LinkedUserType;

    noDataFoundText = this.translate.instant(CONSTANTS.PRIMENG_NO_DATA_FOUND);

    startSearchText = this.translate.instant(CONSTANTS.PRIMENG_INPUT_PLACEHOLDER_START_SEARCH);

    constructor(
        private parentService: ParentService,
        private suService: StudentUsersService,
        private messageService: MessageService,
        private translate: TranslateService,
    ) {}

    ngOnInit(): void {
        this.setupSearch();
    }

    ngOnDestroy(): void {
        this.unsubscribeSearch();
    }

    onHide() {
        this.visibleChange.emit(false);
        this.resetSearch();
    }

    onSearchModeChange(mode: 'publicEduNumber' | 'personId') {
        this.searchMode = mode;
        this.searchControl.setValue('');
        this.searchResults = [];
        this.searchError = null;
    }

    onAdd(row: LinkedUserResponseDTO) {
        const request: AccessUpsertRequestDTO = {
            parentID: this.userType === LinkedUserType.STUDENT ? Number(row.personID) : Number(this.personID),
            childID: this.userType === LinkedUserType.STUDENT ? Number(this.personID) : Number(row.personID),
            hasAccess: 1,
        };
        this.suService.upsertLinkedUsersAccess(request).subscribe({
            next: () => {
                this.messageService.add({
                    severity: 'success',
                    summary: 'Успех',
                    detail: 'Потребителят беше успешно свързан.',
                });
                this.visibleChange.emit(false);
                this.addUser.emit(row);
            },
            error: (err) => {
                this.messageService.add({
                    severity: 'error',
                    summary: 'Грешка',
                    detail: err?.error?.message || 'Възникна грешка при свързването.',
                });
            },
        });
    }

    handleTableAction(event: { row: LinkedUserResponseDTO; action: string }) {
        if (event.action === 'linkUsers') {
            this.onAdd(event.row);
        }
    }

    private setupSearch() {
        this.unsubscribeSearch();
        this.searchSub = this.searchControl.valueChanges
            .pipe(
                debounceTime(400),
                distinctUntilChanged(),
                tap(() => {
                    this.searchLoading = true;
                    this.searchError = null;
                }),
                switchMap((value: string) => {
                    if (!value) {
                        this.searchLoading = false;
                        return of([]);
                    }
                    const query =
                        this.searchMode === 'publicEduNumber' ? { publicEduNumber: value } : { personId: value };
                    if (this.userType === LinkedUserType.STUDENT) {
                        return this.parentService.findParents(query).pipe(
                            catchError(() => {
                                this.searchError = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
                                return of([]);
                            }),
                        );
                    }
                    return this.suService.findStudents(query).pipe(
                        catchError(() => {
                            this.searchError = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
                            return of([]);
                        }),
                    );
                }),
                tap(() => (this.searchLoading = false)),
            )
            .subscribe((results: LinkedUserResponseDTO[]) => {
                this.searchResults = results || [];
            });
    }

    private resetSearch() {
        this.searchControl.setValue('');
        this.searchResults = [];
        this.searchError = null;
    }

    private unsubscribeSearch() {
        if (this.searchSub) {
            this.searchSub.unsubscribe();
            this.searchSub = undefined;
        }
    }
}

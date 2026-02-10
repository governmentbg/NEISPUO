import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LinkedUserResponseDTO } from '@shared/business-object-model/responses/linked-user-response.dto';
import { PersonResponseDTO } from '@shared/business-object-model/responses/person-response.dto';
import { ParentService } from '@shared/services/parent.service';
import { StudentUsersService } from '@shared/services/student-users.service';
import { UserService } from '@shared/services/user.service';
import { MessageService } from 'primeng/api';
import { AccessUpsertRequestDTO } from '@shared/business-object-model/requests/access-upsert-request.dto';
import { CONSTANTS } from '@shared/constants';
import { LinkedUserType } from './shared/linked-users-page.enums';

@Component({
    selector: 'app-linked-users-page',
    templateUrl: './linked-users-page.component.html',
    styleUrls: ['./linked-users-page.component.scss'],
})
export class LinkedUsersPageComponent implements OnInit {
    CONSTANTS = CONSTANTS;

    LinkedUserType = LinkedUserType;

    personID!: string;

    userType!: LinkedUserType;

    personData: PersonResponseDTO | null = null;

    tableData: LinkedUserResponseDTO[] = [];

    loading = false;

    error: string | null = null;

    showAddModal = false;

    addModalTitle = '';

    constructor(
        private route: ActivatedRoute,
        private suService: StudentUsersService,
        private parentService: ParentService,
        private userService: UserService,
        private messageService: MessageService,
    ) {}

    ngOnInit(): void {
        this.extractRouteInfo();
        this.loadPersonData();
        if (this.userType === LinkedUserType.STUDENT) {
            this.loadParentsList();
            this.addModalTitle = CONSTANTS.PRIMENG_TITLE_ADD_PARENT;
        } else if (this.userType === LinkedUserType.PARENT) {
            this.loadStudentsList();
            this.addModalTitle = CONSTANTS.PRIMENG_TITLE_ADD_STUDENT;
        }
    }

    get selectedPersonFullName(): string {
        if (!this.personData) return '';
        const { firstName, middleName, lastName } = this.personData;
        return `${firstName || ''} ${middleName || ''} ${lastName || ''}`.trim();
    }

    get selectedPersonEmail(): string {
        return this.personData?.publicEduNumber || 'N/A';
    }

    get emailLabel(): string {
        return this.userType === LinkedUserType.PARENT
            ? CONSTANTS.PRIMENG_TITLE_EMAIL
            : CONSTANTS.PRIMENG_TITLE_PUBLIC_EDU_NUMBER;
    }

    openAddModal() {
        this.showAddModal = true;
    }

    onAddUser(user: LinkedUserResponseDTO) {
        this.showAddModal = false;
        this.refreshTableData();
    }

    handleTableAction(event: { row: LinkedUserResponseDTO; action: string }) {
        if (event.action === 'unlink') {
            this.unlinkUser(event.row);
        } else if (event.action === 'updateAccess') {
            this.updateUserAccess(event.row);
        }
    }

    private refreshTableData() {
        if (this.userType === LinkedUserType.STUDENT) {
            this.loadParentsList();
        } else if (this.userType === LinkedUserType.PARENT) {
            this.loadStudentsList();
        }
    }

    private loadParentsList() {
        this.loading = true;
        this.error = null;
        this.suService.getLinkedParents(this.personID).subscribe({
            next: (r) => {
                this.tableData = r || [];
                this.loading = false;
            },
            error: () => {
                this.error = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
                this.tableData = [];
                this.loading = false;
            },
        });
    }

    private loadStudentsList() {
        this.loading = true;
        this.error = null;
        this.parentService.getLinkedStudents(this.personID).subscribe({
            next: (r) => {
                this.tableData = r || [];
                this.loading = false;
            },
            error: () => {
                this.error = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
                this.tableData = [];
                this.loading = false;
            },
        });
    }

    private unlinkUser(row: LinkedUserResponseDTO) {
        const id = row.parentChildSchoolBookAccessID;
        if (!id) {
            this.messageService.add({
                severity: 'error',
                summary: 'Грешка',
                detail: 'Липсва идентификатор за връзката.',
            });
            return;
        }
        this.suService.unlinkUsers(id).subscribe({
            next: () => {
                this.messageService.add({
                    severity: 'success',
                    summary: 'Успех',
                    detail: 'Връзката беше успешно премахната.',
                });
                this.refreshTableData();
            },
            error: (err) => {
                this.messageService.add({
                    severity: 'error',
                    summary: 'Грешка',
                    detail: err?.error?.message || 'Възникна грешка при премахване на връзката.',
                });
            },
        });
    }

    private updateUserAccess(row: LinkedUserResponseDTO) {
        const request: AccessUpsertRequestDTO = {
            parentID: this.userType === LinkedUserType.STUDENT ? Number(row.personID) : Number(this.personID),
            childID: this.userType === LinkedUserType.STUDENT ? Number(this.personID) : Number(row.personID),
            hasAccess: row.hasAccess === 1 ? 0 : 1,
        };
        this.suService.upsertLinkedUsersAccess(request).subscribe({
            next: () => {
                this.messageService.add({
                    severity: 'success',
                    summary: 'Успех',
                    detail: 'Достъпът беше успешно обновен.',
                });
                this.refreshTableData();
            },
            error: (err) => {
                this.messageService.add({
                    severity: 'error',
                    summary: 'Грешка',
                    detail: err?.error?.message || 'Възникна грешка при обновяване на достъпа.',
                });
            },
        });
    }

    private loadPersonData(): void {
        this.userService.getPersonByPersonId(this.personID).subscribe({
            next: (person: PersonResponseDTO) => {
                this.personData = person;
            },
            error: () => {
                this.error = CONSTANTS.PRIMENG_ERROR_LOADING_DATA;
            },
        });
    }

    private extractRouteInfo(): void {
        const segments = this.route.snapshot.url;
        const typeSegment = segments.find(
            (seg) => seg.path === LinkedUserType.STUDENT || seg.path === LinkedUserType.PARENT,
        );
        if (typeSegment) {
            this.userType = typeSegment.path as LinkedUserType;
            const typeIndex = segments.findIndex((seg) => seg.path === typeSegment.path);
            if (typeIndex !== -1 && segments.length > typeIndex + 1) {
                this.personID = segments[typeIndex + 1].path;
            }
        }
    }
}

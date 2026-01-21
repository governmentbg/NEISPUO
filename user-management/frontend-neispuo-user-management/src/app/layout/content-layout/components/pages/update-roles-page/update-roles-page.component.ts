import { Component, OnInit, ViewChild } from '@angular/core';
import { CONSTANTS } from '@shared/constants';
import { UserService } from '@shared/services/user.service';
import { MessageService } from 'primeng/api';
import { take } from 'rxjs/operators';
import { UpdateUserRolesRequestDTO } from '@shared/business-object-model/requests/update-user-roles-request.dto';
import { UserRole } from '@shared/business-object-model/responses/user-roles-response.dto';
import { RoleEnum } from 'src/app/shared/enums/roles.enum';
import { RolesService } from 'src/app/shared/services/roles.service';
import { RoleAuditComponent } from './role-audit/role-audit.component';

@Component({
    selector: 'app-update-roles-page',
    templateUrl: './update-roles-page.component.html',
    styleUrls: ['./update-roles-page.component.scss'],
})
export class UpdateRolesPageComponent implements OnInit {
    @ViewChild('roleAudit') roleAudit!: RoleAuditComponent;

    roles: UserRole[] = [];

    sysUserID!: number;

    personID!: number;

    institutionID!: number;

    hasInstitutionAccess!: boolean;

    hasAccountantAccess!: boolean;

    constructor(
        private rolesService: RolesService,
        private messageService: MessageService,
        private userService: UserService,
    ) {}

    ngOnInit(): void {
        const user = JSON.parse(sessionStorage.getItem(CONSTANTS.SESSION_STORAGE_ROLE_UPDATE_INSTITUTION_ID)!);
        this.personID = +user.personID!;
        this.institutionID = +user.institutionID!;
        this.fillUserRoleCheckboxes();
    }

    fillUserRoleCheckboxes() {
        this.userService.getSysUserIDByPersonID(this.personID).subscribe({
            next: (data: any) => {
                this.sysUserID = data.payload.sysUserID;
                this.getUserRoles(this.sysUserID);
            },
            error: (err: any) => {
                console.log('Error getting sysUserID');
            },
        });
    }

    getUserRoles(userID: number) {
        this.rolesService
            .getUserRoles(userID)
            .pipe(take(1))
            .subscribe(
                (response) => {
                    this.roles = response.payload.data[0];
                    this.hasAccountantAccess = this.getAccountantAccess();
                    this.hasInstitutionAccess = this.getInstitutionAccess();
                },
                (error) => {
                    console.log('error fetching paginated data', error);
                },
            );
    }

    getInstitutionAccess(): boolean {
        return !!this.roles.find((r) => r.sysRoleID === RoleEnum.INSTITUTION && r.institutionID === this.institutionID);
    }

    getAccountantAccess(): boolean {
        return !!this.roles.find((r) => r.sysRoleID === RoleEnum.ACCOUNTANT && r.institutionID === this.institutionID);
    }

    updateInstitutionRole(checkboxState: boolean) {
        this.updateRole(checkboxState, RoleEnum.INSTITUTION);
    }

    updateAccountantRole(checkboxState: boolean) {
        this.updateRole(checkboxState, RoleEnum.ACCOUNTANT);
    }

    updateRole(checkboxState: boolean, role: RoleEnum): void {
        const updateUserRolesRequestDTO: UpdateUserRolesRequestDTO = {
            sysRoleID: role,
            sysUserID: this.sysUserID,
            isDeleted: !checkboxState,
            institutionID: this.institutionID,
        };
        // this is a workaround the checbox fire 2 function calls.
        this.rolesService.updateUserRoles(updateUserRolesRequestDTO).subscribe(
            (resp) => {
                this.messageService.add({
                    severity: 'success',
                    summary: updateUserRolesRequestDTO.isDeleted
                        ? 'Ролята беше успешно премахната'
                        : 'Ролята беше успешно добавена',
                });
                console.log('error saving role', resp);
                this.roleAudit.load({});
            },
            (err) => {
                this.messageService.add({
                    severity: 'error',
                    summary: 'Възникна грешка.',
                    detail: 'Моля опитайте отново',
                });
                console.log('error saving role', err);
                this.roleAudit.load({});
            },
        );
    }
}

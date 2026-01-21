import { Injectable } from '@angular/core';
import { AuthQuery } from '@core/authentication/auth.query';
import { Query } from '@datorama/akita';
import { filter, map, take } from 'rxjs/operators';
import { RoleEnum } from 'src/app/shared/enums/roles.enum';
import { ImpersonationState, ImpersonationStore } from './impersopnation.store';

@Injectable({
    providedIn: 'root',
})
export class ImpersonationQuery extends Query<ImpersonationState> {
    private readonly ALLOWED_CIOO_IMPERSONATION = [RoleEnum.MON_ADMIN];

    private readonly ALLOWED_RUO_IMPERSONATION = [...this.ALLOWED_CIOO_IMPERSONATION, RoleEnum.CIOO];

    /**
     * Commenting out roles: RoleEnum.RUO, RoleEnum.RUO_EXPERT,
     * because they are not allowed to impersonate as per latest business requirements.
     */
    private readonly ALLOWED_INSTITUTION_IMPERSONATION = [
        ...this.ALLOWED_RUO_IMPERSONATION,
        // RoleEnum.RUO,
        // RoleEnum.RUO_EXPERT,
    ];

    /**
     * Commenting out roles: RoleEnum.RUO, RoleEnum.RUO_EXPERT,
     * because they are not allowed to impersonate as per latest business requirements.
     */
    private readonly ALLOWED_TEACHER_STUDENT_IMPERSONATION = [
        ...this.ALLOWED_INSTITUTION_IMPERSONATION,
        // RoleEnum.INSTITUTION,
        // RoleEnum.TECHNICAL_INSTITUTION,
    ];

    private readonly selectedRole$ = this.authQuery.select('selected_role').pipe(
        filter((resp) => !!resp),
        take(1),
    );

    public readonly canImpersonateRUO$ = this.selectedRole$.pipe(
        map((role: any) => this.ALLOWED_RUO_IMPERSONATION.includes(role?.SysRoleID)),
    );

    public readonly canImpersonateInstitution$ = this.selectedRole$.pipe(
        map((role: any) => this.ALLOWED_INSTITUTION_IMPERSONATION.includes(role?.SysRoleID)),
    );

    public readonly canImpersonateTeacherStudent$ = this.selectedRole$.pipe(
        map((role: any) => this.ALLOWED_TEACHER_STUDENT_IMPERSONATION.includes(role?.SysRoleID)),
    );

    constructor(protected store: ImpersonationStore, private authQuery: AuthQuery) {
        super(store);
    }
}

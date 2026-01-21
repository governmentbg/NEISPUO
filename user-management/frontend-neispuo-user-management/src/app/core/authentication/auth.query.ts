import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { combineLatest } from 'rxjs';
import { filter, map, take } from 'rxjs/operators';
import { AuthState } from '@shared/business-object-model/interfaces/auth-state.interface';
import { RoleEnum } from 'src/app/shared/enums/roles.enum';
import { AuthStore } from './auth.store';

// this gets the info, stored in the angular project, for the logged in user
@Injectable({
    providedIn: 'root',
})
export class AuthQuery extends Query<AuthState> {
    email$ = this.select().pipe(map((u) => u.sub));

    fullName$ = combineLatest([this.select('FirstName'), this.select('LastName')]).pipe(
        map(([firstName, lastName]) => {
            return `${firstName} ${lastName}`;
        }),
    );

    /** Add first two letters from email atm. This ideally should be FistName and LastName concatenated first letters */
    abbreviatedName$ = this.email$.pipe(
        filter((resp) => !!resp),
        map((fn: string | undefined) => fn!.substring(0, 2).toUpperCase()),
    );

    jwt$ = this.select('jwt');

    authReady$ = this.select('authReady');

    oidcAccessToken$ = this.select('oidcAccessToken');

    isLoggedIn$ = this.select('sub').pipe(map((sub) => sub && sub?.length > 0));

    selectedRole$ = this.select('selected_role');

    isBudgetingInstitution$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.BUDGETING_INSTITUTION),
    );

    isInstitution$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.INSTITUTION),
    );

    isCIOO$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.CIOO),
    );

    isMon$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.MON_ADMIN || role?.SysRoleID === RoleEnum.MON_USER_ADMIN),
    );

    isMunicipality$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.MUNICIPALITY),
    );

    isRuo$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.RUO || role?.SysRoleID === RoleEnum.RUO_EXPERT),
    );

    isStudent$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.STUDENT),
    );

    isTeacher$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.TEACHER),
    );

    isImpersonator$ = this.select('impersonator');

    constructor(protected store: AuthStore) {
        super(store);
    }
}

import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { AuthStore } from './auth.store';
import { AuthState } from './auth-state.interface';
import { filter, map, take } from 'rxjs/operators';
import { RoleEnum } from '@shared/enums/role.enum';

@Injectable({
  providedIn: 'root'
})
export class AuthQuery extends Query<AuthState> {
  email$ = this.select().pipe(map((u) => u.sub));
  fullName$ = this.select().pipe(map((u) => `${u.FirstName} ${u.LastName}`));
  /** Add first two letters from email atm. This ideally should be FistName and LastName concatenated first letters */
  abbreviatedName$ = this.select().pipe(map((u) => `${u.FirstName.substring(0,1)}${u.LastName.substring(0,1)}`));
  jwt$ = this.select('jwt');
  oidcAccessToken$ = this.select('oidcAccessToken');
  authReady$ = this.select('authReady');
  isLoggedIn$ = this.select('sub').pipe(map((sub) => sub?.length > 0));
  selectedRole$ = this.select('selected_role');


  isParent$ = this.select('selected_role').pipe(
    filter((resp) => !!resp),
    take(1),
    map((role) => role?.SysRoleID === RoleEnum.PARENT)
  );


  isMon$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.MON_ADMIN || role?.SysRoleID === RoleEnum.MON_USER_ADMIN),
  );

  isHelpdesk$ = this.selectedRole$.pipe(
        filter((resp) => !!resp),
        take(1),
        map((role) => role?.SysRoleID === RoleEnum.MON_ADMIN || role?.SysRoleID === RoleEnum.CONSORTIUM_HELPDESK),
  );


  constructor(protected store: AuthStore) {
    super(store);
  }
}

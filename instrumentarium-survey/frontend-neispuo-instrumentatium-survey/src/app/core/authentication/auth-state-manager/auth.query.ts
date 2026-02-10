import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { AuthStore } from './auth.store';
import { AuthState } from './interfaces/auth-state.interface';
import { distinctUntilChanged, filter, map } from 'rxjs/operators';
import { RoleEnum } from '../models/role.enum';

@Injectable({
  providedIn: 'root'
})
export class AuthQuery extends Query<AuthState> {
  email$ = this.select().pipe(map((u) => u.sub));
  selectedRole$ = this.select('selected_role');
  fullName$ = this.select().pipe(map((u) => `${u.FirstName} ${u.LastName}`));
  /** Add first two letters from email atm. This ideally should be FistName and LastName concatenated first letters */
  abbreviatedName$ = this.select().pipe(map((u) => `${u.FirstName.substring(0,1)}${u.LastName.substring(0,1)}`));
  jwt$ = this.select('jwt');
  authReady$ = this.select('authReady');

  isLoggedIn$ = this.select('sub').pipe(map((sub) => sub?.length > 0));

  mySysUser$ = this.select('mySysUser').pipe(distinctUntilChanged((v1, v2) => v1?.SysUserID === v2?.SysUserID));
  myMunicipality$ = this.select('myMunicipality').pipe(
    distinctUntilChanged((v1, v2) => v1?.MunicipalityID === v2?.MunicipalityID)
  );

  isMon$ = this.select('selected_role').pipe(
    map(
      (sr) =>
        sr.SysRoleID === RoleEnum.MON_ADMIN ||
        sr.SysRoleID === RoleEnum.MON_EXPERT ||
        sr.SysRoleID === RoleEnum.MON_OBGUM ||
        sr.SysRoleID === RoleEnum.MON_OBGUM_FINANCES ||
        sr.SysRoleID === RoleEnum.MON_CHRAO
    )
  );
  isNio$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.NIO));
  isInstitution$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.INSTITUTION));
  isTeacher$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.TEACHER));
  isParent$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.PARENT));
  isStudent$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.STUDENT));

  constructor(protected store: AuthStore) {
    super(store);
  }
}

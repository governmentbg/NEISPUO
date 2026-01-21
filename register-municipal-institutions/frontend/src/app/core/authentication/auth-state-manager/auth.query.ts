import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import {
  distinctUntilChanged, filter, map,
} from 'rxjs/operators';
import { AuthStore } from './auth.store';
import { AuthState } from './auth-state.interface';
import { RoleEnum } from '../models/role.enum';

@Injectable({
  providedIn: 'root',
})

export class AuthQuery extends Query<AuthState> {
  email$ = this.select().pipe(map((u) => u.sub));

  selectedRole$ = this.select('selected_role');

  FirstName$ = this.select('FirstName');
  
  LastName$ = this.select('LastName');


  /** Add first two letters from email atm. This ideally should be FistName and LastName concatenated first letters */
  abbreviatedName$ = this.email$
    .pipe(
      filter((fn) => fn?.length > 0),
      map((fn: string) => fn.substring(0, 2).toUpperCase()),
    );

  jwt$ = this.select('jwt');

  authReady$ = this.select('authReady');

  isLoggedIn$ = this.select('sub').pipe(map((sub) => sub?.length > 0));

  mySysUser$ = this.select('mySysUser').pipe(distinctUntilChanged((v1, v2) => v1?.SysUserID === v2?.SysUserID));

  myMunicipality$ = this.select('myMunicipality').pipe(distinctUntilChanged((v1, v2) => v1?.MunicipalityID === v2?.MunicipalityID));

  isMunicipality$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.MUNICIPALITY));

  isRuo$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.RUO));

  isMon$ = this.select('selected_role').pipe(map((sr) => sr.SysRoleID === RoleEnum.MON || sr.SysRoleID === RoleEnum.MON_OBGUM || sr.SysRoleID === RoleEnum.MON_OBGUM_FINANCES || sr.SysRoleID === RoleEnum.MON_CHRAO));

  isRuo() {

  }

  constructor(protected store: AuthStore) {
    super(store);
  }

}

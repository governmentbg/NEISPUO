import { Injectable } from '@angular/core';
import { Query } from '@datorama/akita';
import { AuthState } from './auth-state.interface';
import { filter, map, take } from 'rxjs/operators';
import { RoleEnum } from '@shared/enums/role.enum';
import { AuthStore } from './auth.store';

@Injectable({
  providedIn: 'root'
})
export class AuthQuery extends Query<AuthState> {
  email$ = this.select().pipe(map((u) => u.sub));
  fullName$ = this.select().pipe(map((u) => `${u.FirstName} ${u.LastName}`));
  abbreviatedName$ = this.fullName$.pipe(
    filter((fn) => !!fn),
    map((fn: string) => {
      return fn
        .split(' ')
        .map((n) => n[0])
        .join('');
    })
  );
  jwt$ = this.select('jwt');
  oidcAccessToken$ = this.select('oidcAccessToken');
  authReady$ = this.select('authReady');
  isLoggedIn$ = this.select('sub').pipe(map((sub) => sub?.length > 0));

  isParent$ = this.select('selected_role').pipe(
    filter((resp) => !!resp),
    take(1),
    map((role) => role?.SysRoleID === RoleEnum.PARENT)
  );

  constructor(protected override store: AuthStore) {
    super(store);
  }
}

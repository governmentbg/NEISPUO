import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { of } from 'rxjs/internal/observable/of';
import {
  catchError, distinctUntilChanged, filter, switchMap, take, tap,
} from 'rxjs/operators';
import { AuthState } from './auth-state.interface';
import { AuthQuery } from './auth.query';
import { AuthStore } from './auth.store';
import { Profile } from './profile.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private httpClient: HttpClient,
    private authStore: AuthStore,
    private authQuery: AuthQuery,
    private readonly envService: EnvironmentService,
  ) {
    this.loadUserAndMunicipalityOnAuth();
  }

  private readonly environment = this.envService.environment;

  /**
   *
   * @param user Set the whole user object
   */
  setAuthUser(user: AuthState) {
    this.authStore.update(user);
  }

  /**
  *
  * @param user Update the
  */
  async updateAuthUser(user: AuthState) {
    this.authStore.update({ ...user });
    await this.httpClient.get<Profile>(
      `${this.environment.OIDC_BASE_URL}/me`,
      { headers: { Authorization: `Bearer ${this.authStore.getValue().oidcAccessToken}` } },
    ).toPromise();
  }

  /**
   * set all to null and set authReady to true so we know the app is loaded
   */
  removeAuthUser() {
    this.authStore.update({ authReady: true } as AuthState);
  }

  getMeProfile(): Promise<Profile> {
    return this.httpClient.get<Profile>(`${this.environment.OIDC_BASE_URL}/me`).toPromise();
  }

  loadUserAndMunicipalityOnAuth() {
    this.authQuery.authReady$
      .pipe(
        filter((ar) => !!ar),
        take(1),
        switchMap(() => this.authQuery.email$),
        distinctUntilChanged(),
        tap(async (email) => {
          if (email) {
            const selectedRole = this.authStore.getValue().selected_role;
            const mySysUser = await this.httpClient
              .get(`${this.environment.BACKEND_URL}/v1/sys-user/${selectedRole.SysUserID}`)
              .toPromise();
            const myMunicipality = await this.httpClient
              .get(`${this.environment.BACKEND_URL}/v1/municipality/${selectedRole.MunicipalityID}`)
              .toPromise();

            this.authStore.update((state) => ({ ...state, mySysUser, myMunicipality }));
          } else {
            this.authStore.update((state) => ({ ...state, mySysUser: null, myMunicipality: null }));
          }
        }),
        catchError((err) => {
          this.authStore.update((state) => ({ ...state, mySysUser: null, myMunicipality: null }));
          return of(err);
        }),
      )
      .subscribe();
  }

  // async updateAuthUser() {
  //   let userMetadata: Profile = await this.getMeProfile().toPromise();
  //   this.authStore.update(state => ({ fullName: userMetadata.fullName }));
  // }
}

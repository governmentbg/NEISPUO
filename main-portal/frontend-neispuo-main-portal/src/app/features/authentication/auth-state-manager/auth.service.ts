import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@shared/services/environment.service';
import { AuthState } from './auth-state.interface';
import { AuthQuery } from './auth.query';
import { AuthStore } from './auth.store';
import { Profile } from './profile.interface';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private environment = this.envService.environment;

  constructor(
    private httpClient: HttpClient,
    private authStore: AuthStore,
    private authQuery: AuthQuery,
    private envService: EnvironmentService
  ) {}

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
    await this.httpClient
      .get<Profile>(`${this.environment.OIDC_BASE_URL}/me`, {
        headers: { Authorization: `Bearer ${this.authStore.getValue().oidcAccessToken}` }
      })
      .toPromise();
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
}

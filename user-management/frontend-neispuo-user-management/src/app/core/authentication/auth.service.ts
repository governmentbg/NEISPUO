import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { AuthState } from '@shared/business-object-model/interfaces/auth-state.interface';
import { Profile } from '@shared/business-object-model/interfaces/profile.interface';
import { AuthStore } from './auth.store';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    constructor(private httpClient: HttpClient, private authStore: AuthStore, private envService: EnvironmentService) {}

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
        this.httpClient
            .get<Profile>(`${this.environment.OIDC_BASE_URL}/me`, {
                headers: {
                    Authorization: `Bearer ${this.authStore.getValue().oidcAccessToken}`,
                },
            })
            .toPromise();
    }

    /**
     * set all to null and set authReady to true so we know the app is loaded
     */
    removeAuthUser() {
        this.authStore.update({ authReady: true } as AuthState);
    }

    async getMeProfile(): Promise<Profile> {
        return this.httpClient
            .get<Profile>(`${this.environment.OIDC_BASE_URL}/me`, {
                headers: {
                    Authorization: `Bearer ${this.authStore.getValue().oidcAccessToken}`,
                },
            })
            .toPromise();
    }
}

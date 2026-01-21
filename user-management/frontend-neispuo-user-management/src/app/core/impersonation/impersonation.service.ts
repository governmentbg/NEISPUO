import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';

@Injectable({
    providedIn: 'root',
})
export class ImpersonationService {
    private oidcEndpoint = this.envService.environment.OIDC_BASE_URL;

    constructor(private http: HttpClient, private envService: EnvironmentService) {}

    impersonate(username: string, sysRoleID: number) {
        const body = new URLSearchParams();
        body.set('targetAccountUsername', username);
        body.set('targetAccountSysRoleID', `${sysRoleID}`);
        const headers = new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded');

        return this.http.post<any[]>(`${this.oidcEndpoint}/impersonate`, body.toString(), {
            withCredentials: true,
            headers,
        });
    }

    endImpersonate() {
        const headers = new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded');
        return this.http.post<any[]>(`${this.oidcEndpoint}/end_impersonate`, null, {
            withCredentials: true,
            headers,
        });
    }
}

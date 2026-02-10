import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class RUOService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getRUOs(): Promise<UserManagementResponse> {
        return this.httpClientService.get<UserManagementResponse>(`${this.environment.BACKEND_URL}/v1/ruo`).toPromise();
    }
}

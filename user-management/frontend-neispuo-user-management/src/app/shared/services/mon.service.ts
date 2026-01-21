import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { MonResponseDTO } from '@shared/business-object-model/responses/mon-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class MonService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getMon(): Promise<UserManagementResponse<MonResponseDTO>> {
        return this.httpClientService
            .get<UserManagementResponse<MonResponseDTO>>(`${this.environment.BACKEND_URL}/v1/mon`)
            .toPromise();
    }
}

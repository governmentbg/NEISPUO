import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { ErrorsResponseDTO } from '@shared/business-object-model/responses/errors-response.dto';
import { GetManyDefaultResponse } from '@shared/models/get-many-default-response';
import { Observable } from 'rxjs';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class ErrorsService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getErrors(params: { [key: string]: any }): Observable<GetManyDefaultResponse<ErrorsResponseDTO>> {
        return this.httpClientService.get<GetManyDefaultResponse<ErrorsResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/log`,
        );
    }
}

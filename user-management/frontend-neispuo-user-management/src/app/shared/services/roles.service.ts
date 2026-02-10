import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@core/services/environment.service';
import { UpdateUserRolesRequestDTO } from '../business-object-model/requests/update-user-roles-request.dto';
import { UpdateUserRolesResponse } from '../business-object-model/responses/update-user-roles-response.dto';
import { UserRolesResponse } from '../business-object-model/responses/user-roles-response.dto';

@Injectable({
    providedIn: 'root',
})
export class RolesService {
    private readonly environment = this.envService.environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {}

    getUserRoles(userID: number): Observable<UserRolesResponse> {
        return this.httpClientService.put<UserRolesResponse>(
            `${this.environment.BACKEND_URL}/v1/role-management/assignments/get-all?from=0&numberOfElements=25`,
            [{ sysUserID: userID }],
        );
    }

    updateUserRoles(updateUserRolesRequestDTO: UpdateUserRolesRequestDTO): Observable<UpdateUserRolesResponse> {
        return this.httpClientService.post<UpdateUserRolesResponse>(
            `${this.environment.BACKEND_URL}/v1/role-management/assignment/manage?from=0&numberOfElements=25`,
            updateUserRolesRequestDTO,
        );
    }
}

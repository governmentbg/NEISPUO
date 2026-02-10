import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FilterMetadata } from 'primeng/api';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@core/services/environment.service';
import { EventStatus } from '@shared/enums/event-status.enum';
import { AzureUsersResponseDTO } from '@shared/business-object-model/responses/azure-users-response.dto';
import { Paging } from '../business-object-model/paging';
import { EditUserRequestDTO } from '../business-object-model/requests/edit-user-request.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class AzureUsersService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getAzureUsers(
        paging: Paging,
        filters?: { [s: string]: FilterMetadata },
    ): Observable<UserManagementResponse<AzureUsersResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<AzureUsersResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/users/get-all`,
            filters,
            { params: { ...paging } },
        );
    }

    updateAzureUserTeacher(
        editUserRequestDTO: EditUserRequestDTO,
    ): Observable<UserManagementResponse<AzureUsersResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<AzureUsersResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/teacher`,
            editUserRequestDTO,
            { params: {} },
        );
    }

    updateAzureUserStudent(
        editUserRequestDTO: EditUserRequestDTO,
    ): Observable<UserManagementResponse<AzureUsersResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<AzureUsersResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/student/fe`,
            editUserRequestDTO,
            { params: {} },
        );
    }

    restartWorkflow(id: number): Observable<UserManagementResponse<AzureUsersResponseDTO>> {
        return this.httpClientService.patch<UserManagementResponse<AzureUsersResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/users/workflow/restart/${id}`,
            { params: {} },
        );
    }

    isRestartWorkflowButtonEnabled(status: EventStatus, retryAttempts: number) {
        if (
            (status === EventStatus.FAILED ||
                status === EventStatus.FAILED_CREATION ||
                status === EventStatus.FAILED_SYNCRONIZATION) &&
            retryAttempts > 4
        ) {
            return true;
        }

        return false;
    }
}

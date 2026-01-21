import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FilterMetadata } from 'primeng/api';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@core/services/environment.service';
import { EventStatus } from '@shared/enums/event-status.enum';
import { AzureOrganizationResponseDTO } from '@shared/business-object-model/responses/azure-organizations-response.dto';
import { Paging } from '../business-object-model/paging';
import { EditInstitutionResponseDTO } from '../business-object-model/responses/edit-institution-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class AzureOrganizationsService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getAzureOrganizations(
        paging: Paging,
        filters?: { [s: string]: FilterMetadata },
    ): Observable<UserManagementResponse<AzureOrganizationResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<AzureOrganizationResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/school/get-all`,
            filters,
            { params: { ...paging } },
        );
    }

    updateAzureOrganization(
        editOrganizationRequestDTO: EditInstitutionResponseDTO,
    ): Observable<UserManagementResponse<AzureOrganizationResponseDTO>> {
        return this.httpClientService.post<UserManagementResponse<AzureOrganizationResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/school`,
            editOrganizationRequestDTO,
            { params: {} },
        );
    }

    restartWorkflow(id: number): Observable<UserManagementResponse<AzureOrganizationResponseDTO>> {
        return this.httpClientService.patch<UserManagementResponse<AzureOrganizationResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/school/workflow/restart/${id}`,
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

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FilterMetadata } from 'primeng/api';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@core/services/environment.service';
import { EventStatus } from '@shared/enums/event-status.enum';
import { AzureClassesResponseDTO } from '@shared/business-object-model/responses/azure-classes-response.dto';
import { Paging } from '../business-object-model/paging';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class AzureClassesService {
    private readonly environment = this.envService.environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {}

    getAzureClasses(
        paging: Paging,
        filters?: { [s: string]: FilterMetadata },
    ): Observable<UserManagementResponse<AzureClassesResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<AzureClassesResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/class/get-all`,
            filters,
            { params: { ...paging } },
        );
    }

    restartWorkflow(id: number): Observable<UserManagementResponse<AzureClassesResponseDTO>> {
        return this.httpClientService.patch<UserManagementResponse<AzureClassesResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/class/workflow/restart/${id}`,
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

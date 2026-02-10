import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FilterMetadata } from 'primeng/api';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@core/services/environment.service';
import { EventStatus } from '@shared/enums/event-status.enum';
import { AzureEnrollmentsResponseDTO } from '@shared/business-object-model/responses/azure-enrollments-response.dto';
import { Paging } from '../business-object-model/paging';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class AzureEnrollMentsService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getAzureEnrollments(
        paging: Paging,
        filters?: { [s: string]: FilterMetadata },
    ): Observable<UserManagementResponse<AzureEnrollmentsResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<AzureEnrollmentsResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/enrollment/get-all`,
            filters,
            { params: { ...paging } },
        );
    }

    restartWorkflow(id: number): Observable<UserManagementResponse<AzureEnrollmentsResponseDTO>> {
        return this.httpClientService.patch<UserManagementResponse<AzureEnrollmentsResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/azure-integrations/enrollment/workflow/restart/${id}`,
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

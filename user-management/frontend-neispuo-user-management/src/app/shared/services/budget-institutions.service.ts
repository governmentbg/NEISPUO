import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { BudgetingInstitutionResponseDTO } from '@shared/business-object-model/responses/budgeting-institution-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class BudgetingInstitutionsService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getBudgetingInstitutions(): Promise<UserManagementResponse<BudgetingInstitutionResponseDTO>> {
        return this.httpClientService
            .get<UserManagementResponse<BudgetingInstitutionResponseDTO>>(
                `${this.environment.BACKEND_URL}/v1/budgeting-institutions`,
            )
            .toPromise();
    }
}

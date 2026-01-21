import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EditInstitutionResponseDTO } from '@shared/business-object-model/responses/edit-institution-response.dto';
import { FilterMetadata } from 'primeng/api';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@core/services/environment.service';
import { InstitutionResponseDTO } from '@shared/business-object-model/responses/institution-response.dto';
import { Paging } from '../business-object-model/paging';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class InstitutionsService {
    private readonly environment = this.envService.environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {}

    getInstitutions(
        paging: Paging,
        filters?: { [s: string]: FilterMetadata },
    ): Observable<UserManagementResponse<InstitutionResponseDTO>> {
        return this.httpClientService.put<UserManagementResponse<InstitutionResponseDTO>>(
            `${this.environment.BACKEND_URL}/v1/institutions`,
            filters,
            { params: { ...paging } },
        );
    }

    getInstitution(institutionID: number): Observable<EditInstitutionResponseDTO> {
        return this.httpClientService.get<EditInstitutionResponseDTO>(
            `${this.environment.BACKEND_URL}/v1/institutions/${institutionID}`,
        );
    }
}

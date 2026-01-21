import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { EnvironmentService } from '@core/services/environment.service';
import { Observable } from 'rxjs';
import { SchoolBookCodeAssignRequestDTO } from '@shared/business-object-model/responses/school-book-code-assign-request.dto';
import { SchoolBookCodeAssignResponseDTO } from '@shared/business-object-model/responses/school-book-code-assign-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class SchoolBookCodeService {
    private readonly environment;

    constructor(private envService: EnvironmentService, private apiService: ApiService) {
        this.environment = this.envService.environment;
    }

    updateSchoolBookCode(personID: number): Observable<UserManagementResponse<SchoolBookCodeAssignResponseDTO>> {
        const dto: SchoolBookCodeAssignRequestDTO = {
            personIDs: [personID],
        };
        return this.apiService.post<UserManagementResponse<SchoolBookCodeAssignResponseDTO>>(
            `/v1/school-book-code/assign`,
            dto,
        );
    }
}

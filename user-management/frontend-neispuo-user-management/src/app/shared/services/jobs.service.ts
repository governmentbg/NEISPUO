import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { JobsResponseDTO } from '@shared/business-object-model/responses/jobs-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class JobsService {
    private readonly environment;

    constructor(private httpClientService: HttpClient, private envService: EnvironmentService) {
        this.environment = this.envService.environment;
    }

    getAllJobs(): Promise<UserManagementResponse<JobsResponseDTO[]>> {
        return this.httpClientService
            .get<UserManagementResponse<JobsResponseDTO[]>>(`${this.environment.BACKEND_URL}/v1/jobs`)
            .toPromise();
    }

    updateJob(dto: JobsResponseDTO): Promise<UserManagementResponse<JobsResponseDTO[]>> {
        const { name } = dto;
        return this.httpClientService
            .post<UserManagementResponse<JobsResponseDTO[]>>(`${this.environment.BACKEND_URL}/v1/jobs/update`, dto)
            .toPromise();
    }
}

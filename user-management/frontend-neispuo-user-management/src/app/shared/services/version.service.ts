import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { VersionResponseDTO } from '@shared/business-object-model/responses/version-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class VersionService {
    constructor(private apiService: ApiService) {}

    getBackendVersion(componentReference: any) {
        this.apiService.get('/v1/version').subscribe((versionBE: UserManagementResponse<VersionResponseDTO>) => {
            componentReference.appVersion += `, ${versionBE.payload.name}: ${versionBE.payload.version}`;
        });
    }
}

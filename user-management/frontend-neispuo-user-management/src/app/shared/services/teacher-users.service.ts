import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { SyncAzureUserDTO } from '@shared/business-object-model/requests/sync-azure-user.dto';
import { TeacherResponseDTO } from '@shared/business-object-model/responses/teacher-response.dto';
import { MessageService } from 'primeng/api';

import { Observable, Subscription } from 'rxjs';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class TeacherUsersService {
    constructor(private apiService: ApiService, private messageService: MessageService) {}

    callSyncAzureTeacher(dto: TeacherResponseDTO): Observable<UserManagementResponse<SyncAzureUserDTO>> {
        const { personID } = dto;
        const syncDTO: SyncAzureUserDTO = {
            personID,
        };
        return this.apiService.post<UserManagementResponse<SyncAzureUserDTO>>(
            `/v1/azure-integrations/teacher/azure-sync`,
            syncDTO,
        );
    }

    enableSyncButton(dto: TeacherResponseDTO) {
        const { publicEduNumber, azureID } = dto;
        if (!publicEduNumber || !azureID) return true;
        return false;
    }

    printSuccessMessage() {
        this.messageService.add({
            severity: 'success',
            summary: 'Успех.',
            detail: `Изчакайте 5 минути и проверете записа отново.`,
        });
    }

    printErrorMessage(error?: any) {
        this.messageService.add({
            severity: 'error',
            summary: 'Възникна грешка.',
            detail: error?.message || `Свържете се с администратор.`,
        });
    }
}

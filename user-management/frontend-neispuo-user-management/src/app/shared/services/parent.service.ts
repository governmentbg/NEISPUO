import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { ParentResponseDTO } from '@shared/business-object-model/responses/parent-response.dto';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { UserRoleType } from 'src/app/layout/content-layout/components/pages/linked-users-page/shared/linked-users-page.enums';
import { LinkedUserResponseDTO } from '@shared/business-object-model/responses/linked-user-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class ParentService {
    constructor(private apiService: ApiService, private messageService: MessageService) {}

    callSyncParent(dto: ParentResponseDTO): Observable<UserManagementResponse<ParentResponseDTO>> {
        const { personID, username } = dto;
        const syncDTO: ParentResponseDTO = {
            personID,
            username,
        };
        console.log(syncDTO);
        return this.apiService.post<UserManagementResponse<ParentResponseDTO>>(`/v1/parent/azure-sync`, syncDTO);
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

    isSyncButtonEnabled(dto: ParentResponseDTO) {
        if (dto.azureID === null && dto.username !== null) {
            return true;
        }
        return false;
    }

    getLinkedStudents(personID: string): Observable<LinkedUserResponseDTO[]> {
        return this.apiService.get('/v1/parent-access/access-list', { personID, userRoleType: UserRoleType.PARENT });
    }

    findParents(query: { publicEduNumber?: string; personId?: string }): Observable<LinkedUserResponseDTO[]> {
        const params: any = {
            userRoleType: UserRoleType.PARENT,
        };

        if (query.publicEduNumber) {
            params.email = query.publicEduNumber;
        }

        if (query.personId) {
            params.personID = query.personId;
        }

        return this.apiService.get<LinkedUserResponseDTO[]>('/v1/user/find-many', params);
    }
}

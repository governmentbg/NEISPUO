import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { SyncAzureUserDTO } from '@shared/business-object-model/requests/sync-azure-user.dto';
import { SchoolBookCodeAssignResponseDTO } from '@shared/business-object-model/responses/school-book-code-assign-response.dto';
import { StudentResponseDTO } from '@shared/business-object-model/responses/student-response.dto';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { UserRoleType } from 'src/app/layout/content-layout/components/pages/linked-users-page/shared/linked-users-page.enums';
import { LinkedUserResponseDTO } from '@shared/business-object-model/responses/linked-user-response.dto';
import { AccessUpsertRequestDTO } from '@shared/business-object-model/requests/access-upsert-request.dto';
import { ParentChildAccessResponseDTO } from '@shared/business-object-model/responses/parent-child-access-response.dto';
import { DeleteParentChildSchoolBookAccessResponseDTO } from '@shared/business-object-model/responses/delete-parent-child-access-response';
import { SchoolBookCodeService } from './school-book-code.service';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class StudentUsersService {
    constructor(
        private schoolBookCodeService: SchoolBookCodeService,
        private apiService: ApiService,
        private messageService: MessageService,
    ) {}

    assignSchoolBookCode(personID: number): Observable<UserManagementResponse<SchoolBookCodeAssignResponseDTO>> {
        return this.schoolBookCodeService.updateSchoolBookCode(personID);
    }

    callSyncAzureStudent(dto: StudentResponseDTO): Observable<UserManagementResponse<SyncAzureUserDTO>> {
        const { personID } = dto;
        const syncDTO: SyncAzureUserDTO = {
            personID,
        };
        return this.apiService.post<UserManagementResponse<SyncAzureUserDTO>>(
            `/v1/azure-integrations/student/azure-sync`,
            syncDTO,
        );
    }

    callCovertAzureStudentToTeacher(dto: StudentResponseDTO): Observable<UserManagementResponse<SyncAzureUserDTO>> {
        const { personID } = dto;
        return this.apiService.post<UserManagementResponse<SyncAzureUserDTO>>(
            `/v1/azure-integrations/student/convert`,
            { personID },
        );
    }

    enableSyncButton(dto: StudentResponseDTO) {
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

    getLinkedParents(personID: string): Observable<LinkedUserResponseDTO[]> {
        return this.apiService.get('/v1/parent-access/access-list', { personID, userRoleType: UserRoleType.STUDENT });
    }

    findStudents(query: { publicEduNumber?: string; personId?: string }): Observable<LinkedUserResponseDTO[]> {
        const params: any = {
            userRoleType: UserRoleType.STUDENT,
        };

        if (query.publicEduNumber) {
            params.email = query.publicEduNumber;
        }

        if (query.personId) {
            params.personID = query.personId;
        }

        return this.apiService.get<LinkedUserResponseDTO[]>('/v1/user/find-many', params);
    }

    upsertLinkedUsersAccess(request: AccessUpsertRequestDTO): Observable<ParentChildAccessResponseDTO> {
        return this.apiService.put('/v1/parent-access/access-upsert', request);
    }

    unlinkUsers(parentChildSchoolBookAccessID: number): Observable<DeleteParentChildSchoolBookAccessResponseDTO> {
        return this.apiService.delete(`/v1/parent-access/${parentChildSchoolBookAccessID}`);
    }
}

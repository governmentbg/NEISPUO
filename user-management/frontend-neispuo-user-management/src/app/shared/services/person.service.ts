import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { AzureUsersResponseDTO } from '@shared/business-object-model/responses/azure-users-response.dto';
import { NonSyncedPersonResponseDTO } from '@shared/business-object-model/responses/non-synced-person-response.dto';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class PersonService {
    constructor(private apiService: ApiService, private messageService: MessageService) {}

    callNonSyncedPersonCreateEndpoint(
        dto: NonSyncedPersonResponseDTO,
    ): Observable<UserManagementResponse<AzureUsersResponseDTO>> {
        return this.apiService.post<UserManagementResponse<AzureUsersResponseDTO>>(`/v1/non-synced-person/create`, dto);
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

    printFiveMinutesWarrningMessage(error?: any) {
        this.messageService.add({
            severity: 'warn',
            summary: 'Неуспех.',
            detail: `Вече сте опитали да създадете потребител преди по-малко от 5 минути.`,
        });
    }
}

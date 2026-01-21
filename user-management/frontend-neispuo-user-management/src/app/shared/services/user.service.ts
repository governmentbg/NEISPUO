import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { UserResponseDTO } from '@shared/business-object-model/responses/user-response.dto';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PersonResponseDTO } from '@shared/business-object-model/responses/person-response.dto';
import { UserManagementResponse } from '../business-object-model/responses/user-management-response';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    constructor(private apiService: ApiService) {}

    getSysUserIDByPersonID(personID: number) {
        return this.apiService.get<UserManagementResponse<UserResponseDTO>>('/v1/user', { personID });
    }

    getSysUsersIDByPersonID(personID: number): Observable<UserManagementResponse<UserResponseDTO[]>> {
        return this.apiService
            .get<UserManagementResponse<UserResponseDTO[] | UserResponseDTO | null>>('/v1/user/get-all', { personID })
            .pipe(
                map((resp) => {
                    let users: UserResponseDTO[] = [];
                    if (Array.isArray(resp.payload)) {
                        users = resp.payload;
                    } else if (resp.payload) {
                        users = [resp.payload];
                    }
                    users = users.map((user) => ({
                        ...user,
                        deletedOn: user.deletedOn ? new Date(user.deletedOn) : undefined,
                    }));
                    return { ...resp, payload: users };
                }),
            );
    }

    getPersonByPersonId(personID: string): Observable<PersonResponseDTO> {
        return this.apiService.get(`/v1/person/${personID}`);
    }
}

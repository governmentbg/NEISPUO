import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
import { ArchivedAzureEntityRequestDTO } from '@shared/business-object-model/requests/archived-azure-entity-request.dto';
import { GetOrganizationInfoRequestDTO } from '@shared/business-object-model/requests/get-organization-info-request.dto';
import { GetParentInfoRequestDTO } from '@shared/business-object-model/requests/get-parent-info-request.dto';
import { GetUserInfoRequestDTO } from '@shared/business-object-model/requests/get-user-info-request.dto';
import { AzureUsersResponseDTO } from '@shared/business-object-model/responses/azure-users-response.dto';
import { OrganizationInfoResponseDTO } from '@shared/business-object-model/responses/organization-info-response.dto';
import { ParentInfoResponseDTO } from '@shared/business-object-model/responses/parent-info-response.dto';
import { PersonResponseDTO } from '@shared/business-object-model/responses/person-response.dto';
import { UserInfoResponseDTO } from '@shared/business-object-model/responses/user-info-response.dto';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AzureOrganizationResponseDTO } from '@shared/business-object-model/responses/azure-organizations-response.dto';
import { AzureEnrollmentsResponseDTO } from '@shared/business-object-model/responses/azure-enrollments-response.dto';
import { InstitutionsTableEntity } from '../components/pages/sync-history/shared/neispuo-profile-card/interfaces/institutions-table-entity.interface';

@Injectable({
    providedIn: 'root',
})
export class SyncHistoryService {
    constructor(private apiService: ApiService) {}

    getAzureOrganizationInfo({ schoolId }: GetOrganizationInfoRequestDTO): Observable<OrganizationInfoResponseDTO> {
        return this.apiService.get(`/v1/graph-api/organization`, { schoolId });
    }

    getAzureUserInfo(request: GetUserInfoRequestDTO): Observable<UserInfoResponseDTO> {
        return this.apiService.get(`/v1/graph-api/user`, { ...request });
    }

    getAzureParentInfo({ email }: GetParentInfoRequestDTO): Observable<ParentInfoResponseDTO> {
        return this.apiService.get(`/v1/graph-api/parent`, { email });
    }

    getArchivedAzureUserData(request: ArchivedAzureEntityRequestDTO): Observable<AzureUsersResponseDTO[]> {
        return this.apiService
            .get('/v1/azure-integrations/users/archived-previous-years', request)
            .pipe(map((arr) => this.mapCreatedOnToDate(arr)));
    }

    getActiveAzureUserData(personID: number): Observable<AzureUsersResponseDTO[]> {
        const qb = RequestQueryBuilder.create()
            .search({
                $and: [
                    {
                        personID: { $eq: personID },
                    },
                ],
            })
            .setPage(1)
            .setLimit(99999);

        return this.apiService.get<{ data: AzureUsersResponseDTO[] }>('/v1/azure-users', qb.queryObject).pipe(
            map((resp) => {
                const arr = resp?.data ?? [];
                return this.mapCreatedOnToDate(arr);
            }),
        );
    }

    getActiveAzureOrganizationData(institutionID: number): Observable<AzureOrganizationResponseDTO[]> {
        const qb = RequestQueryBuilder.create()
            .search({
                $and: [
                    {
                        organizationID: { $eq: institutionID },
                    },
                ],
            })
            .setPage(1)
            .setLimit(99999);

        return this.apiService
            .get<{ data: AzureOrganizationResponseDTO[] }>('/v1/azure-organizations', qb.queryObject)
            .pipe(
                map((resp) => {
                    const arr = resp?.data ?? [];
                    return this.mapCreatedOnToDate(arr);
                }),
            );
    }

    getArchivedAzureOrganizationData(
        request: ArchivedAzureEntityRequestDTO,
    ): Observable<AzureOrganizationResponseDTO[]> {
        return this.apiService
            .get<AzureOrganizationResponseDTO[]>('/v1/azure-integrations/school/archived-previous-years', request)
            .pipe(map((resp) => this.mapCreatedOnToDate(resp)));
    }

    getActiveAzureEnrollmentData(personID: number): Observable<AzureEnrollmentsResponseDTO[]> {
        const qb = RequestQueryBuilder.create()
            .search({
                $and: [
                    {
                        userPersonID: { $eq: personID },
                    },
                ],
            })
            .setPage(1)
            .setLimit(99999);

        return this.apiService
            .get<{ data: AzureEnrollmentsResponseDTO[] }>('/v1/azure-enrollments', qb.queryObject)
            .pipe(
                map((resp) => {
                    const arr = resp?.data ?? [];
                    return this.mapCreatedOnToDate(arr);
                }),
            );
    }

    getArchivedAzureEnrollmentData(request: ArchivedAzureEntityRequestDTO): Observable<AzureEnrollmentsResponseDTO[]> {
        return this.apiService
            .get<AzureEnrollmentsResponseDTO[]>('/v1/azure-integrations/enrollment/archived-previous-years', request)
            .pipe(map((resp) => this.mapCreatedOnToDate(resp)));
    }

    getInstitutionById(institutionID: string): Observable<InstitutionsTableEntity> {
        return this.apiService.get(`/v1/institutions/${institutionID}`);
    }

    private mapCreatedOnToDate<T extends { createdOn?: any }>(arr: T[]): T[] {
        if (!Array.isArray(arr)) return arr;

        return arr.map((item) => ({
            ...item,
            createdOn: item.createdOn ? new Date(item.createdOn) : undefined,
        }));
    }
}

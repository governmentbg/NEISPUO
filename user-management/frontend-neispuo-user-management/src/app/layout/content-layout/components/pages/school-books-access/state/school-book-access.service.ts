import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { EnvironmentService } from '@core/services/environment.service';
import { TranslateService } from '@ngx-translate/core';
import { AvailableSchoolBooksResponseDTO } from '@shared/business-object-model/responses/available-school-books-response.dto';
import { PersonSchoolBookResponseDTO } from '@shared/business-object-model/responses/person-school-book-response.dto';
import { CONSTANTS } from '@shared/constants';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserManagementResponse } from '../../../../../../shared/business-object-model/responses/user-management-response';
import { SchoolBooksAccessQuery } from './school-books-access.query';
import { SchoolBooksAccessStore } from './school-books-access.store';

@Injectable({
    providedIn: 'root',
})
export class SchoolBookAccessService {
    private readonly environment;

    constructor(
        private envService: EnvironmentService,
        private apiService: ApiService,
        private messageService: MessageService,
        private translateService: TranslateService,
        private schoolBooksAccessQuery: SchoolBooksAccessQuery,
        private schoolBooksAccessStore: SchoolBooksAccessStore,
    ) {
        this.environment = this.envService.environment;
    }

    set institutionId(institutionId: number) {
        this.schoolBooksAccessStore.update({ institutionId });
    }

    get institutionId() {
        return this.schoolBooksAccessQuery.getValue().institutionId;
    }

    set personId(personId: number) {
        this.schoolBooksAccessStore.update({ personId });
    }

    get personId() {
        return this.schoolBooksAccessQuery.getValue().personId;
    }

    setPersonalSchoolBooks() {
        return this.getSchoolBooksAccessByPersonID(this.personId).pipe(
            map((response) => {
                this.schoolBooksAccessStore.update({
                    assignedSchoolBooks: [...(response.payload as PersonSchoolBookResponseDTO[])],
                });
            }),
        );
    }

    getAvailableSchoolBooks() {
        return this.getSchoolBooksByInstitutionId(this.institutionId).pipe(
            map((schoolBooksResponse: UserManagementResponse<AvailableSchoolBooksResponseDTO[]>) => {
                return (schoolBooksResponse.payload as AvailableSchoolBooksResponseDTO[])?.filter(
                    (schoolBook: AvailableSchoolBooksResponseDTO) =>
                        !this.schoolBooksAccessQuery
                            .getValue()
                            .assignedSchoolBooks.some(
                                (schoolBookToBeRemoved: AvailableSchoolBooksResponseDTO) =>
                                    schoolBookToBeRemoved.classBookID === schoolBook.classBookID &&
                                    schoolBookToBeRemoved.schoolYear === schoolBook.schoolYear,
                            ),
                );
            }),
        );
    }

    updateAdminAccess(dto: PersonSchoolBookResponseDTO) {
        const updateAdminAccessDTO = {
            rowID: dto.rowID,
            hasAdminAccess: dto.hasAdminAccess,
        };
        return this.apiService.patch(`/v1/school-book-access`, updateAdminAccessDTO);
    }

    getSchoolBooksByInstitutionId(
        institutionID: number,
    ): Observable<UserManagementResponse<AvailableSchoolBooksResponseDTO[]>> {
        return this.apiService.get<UserManagementResponse<AvailableSchoolBooksResponseDTO[]>>(
            `/v1/school-books?institutionID=${institutionID}&personID=${this.personId}`,
        );
    }

    getSchoolBooksAccessByPersonID(
        personID: number,
    ): Observable<UserManagementResponse<AvailableSchoolBooksResponseDTO[]>> {
        return this.apiService.get<UserManagementResponse<AvailableSchoolBooksResponseDTO[]>>(
            `/v1/school-book-access?personID=${personID}`,
        );
    }

    giveSchoolBookAccessToPerson(
        transformedSchoolBooksAccess: [number, number][],
    ): Observable<UserManagementResponse<PersonSchoolBookResponseDTO[]>> {
        const schoolBooks: PersonSchoolBookResponseDTO[] = transformedSchoolBooksAccess.map((tsba) => ({
            schoolYear: tsba[0],
            classBookID: tsba[1],
            personID: +this.personId,
            hasAdminAccess: false,
        }));
        return this.apiService.post<UserManagementResponse<PersonSchoolBookResponseDTO[]>>(
            `/v1/school-book-access`,
            schoolBooks,
        );
    }

    deleteSelectedSchoolBookAccesses(selectedSchoolBooksRowIDs: number[]) {
        return this.apiService.delete(`/v1/school-book-access?rowIDs=${selectedSchoolBooksRowIDs}`);
    }

    showSuccess(successMessage: string) {
        this.messageService.add({
            severity: 'success',
            summary: successMessage,
            life: 2000,
        });
    }

    showError(message: string) {
        this.messageService.add({
            severity: 'error',
            summary: this.translateService.instant(CONSTANTS.PRIMENG_TOASTR_SUMMARY_ERROR),
            detail: message,
        });
    }
}

import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { Paging } from 'src/common/dto/paging.dto';
import { SysUserResponseDTO } from 'src/common/dto/responses/sys-user.response.dto';
import { AzureEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/azure-enrollments.service';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncAzureEnrollmentsRepository } from './sync-azure-enrollments.repository';

@Injectable()
export class SyncAzureEnrollmentsService {
    constructor(
        private syncAzureEnrollmentsRepository: SyncAzureEnrollmentsRepository,
        private educationalStateService: EducationalStateService,
        private readonly graphApiService: GraphApiService,
        private readonly azureEnrollmentService: AzureEnrollmentsService,
    ) {}

    async getStudentTeacherUsersCount() {
        return this.syncAzureEnrollmentsRepository.getStudentTeacherUsersCount();
    }

    async getPaginatedStudentTeacherUsers(paging: Paging) {
        return this.syncAzureEnrollmentsRepository.getPaginatedStudentTeacherUsers(paging);
    }

    async getUserNeispuoEnrollments(personID: number) {
        return (
            await this.educationalStateService.getUserEducationalStatesByPersonID({
                personID,
            })
        ).map((r) => r.institutionID);
    }

    async syncEnrollments() {
        console.log(`Started: ${Date.now()}`);

        const studentTeacherUsersCount = await this.getStudentTeacherUsersCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalPages = studentTeacherUsersCount / itemsPerPage;

        for (let i = 0; i < totalPages; i += 1) {
            let studentTeacherUsers: SysUserResponseDTO[] = [];
            try {
                console.log(
                    `FROM: ${i * itemsPerPage} ---- TO:  ${i * itemsPerPage + itemsPerPage} ---- TO:  ${
                        itemsPerPage * totalPages
                    } `,
                );
                studentTeacherUsers = await this.getPaginatedStudentTeacherUsers({
                    from: i * itemsPerPage,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                i -= 1;
                console.log(e);
                continue;
            }

            for (const studentTeacher of studentTeacherUsers) {
                try {
                    const { personID } = studentTeacher;
                    // check enrollments (NEISPUO is the single source of truth)
                    const azureEnrollments = await this.graphApiService.getUserSchoolEnrollmentInfo(
                        studentTeacher.username,
                    );
                    const neispuoEnrollments = await this.getUserNeispuoEnrollments(personID);
                    // const azureObjectExists = await this.azureObjectExists(azureEnrollments, personID);
                    if (azureEnrollments?.status !== GraphApiResponseEnum.SUCCESS || azureEnrollments?.response) {
                        console.log(`Skipping. No Azure object for personID: ${personID}`);
                        continue;
                    }
                    // check Azure Enrollments, if they are not in NEISPUO -> delete them
                    const azureEnrollmentsArray = [];
                    for (const azureEnr of azureEnrollments?.response) {
                        const userRole = UserRoleType.STUDENT;
                        const { schoolNumber: institutionID } = azureEnr;
                        if (!neispuoEnrollments.includes(+institutionID)) {
                            await this.azureEnrollmentService.deleteAzureEnrollmentUserToSchool({
                                userRole,
                                personID,
                                institutionID: +institutionID,
                            });
                        } else azureEnrollmentsArray.push(institutionID);
                    }

                    // check for missing enrollments
                    for (const n of neispuoEnrollments) {
                        if (!azureEnrollmentsArray.includes(n)) {
                            const userRole = UserRoleType.STUDENT;
                            await this.azureEnrollmentService.createAzureEnrollmentUserToSchool({
                                personID,
                                institutionID: n,
                                userRole,
                            });
                        }
                    }
                } catch (e) {
                    console.log(e);
                    continue;
                }
            }
        }
    }
}

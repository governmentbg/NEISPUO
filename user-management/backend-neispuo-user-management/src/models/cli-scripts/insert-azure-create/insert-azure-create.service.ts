import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { AzureUserResponseFactory } from 'src/common/factories/azure-user-response-dto.factory';
import { StudentService } from 'src/models/student/routing/student.service';
import { TeacherService } from 'src/models/teacher/routing/teacher.service';
import { InsertAzureCreateRepository } from './insert-azure-create.repository';

@Injectable()
export class InsertAzureCreateService {
    constructor(
        private insertAzureCreateRepository: InsertAzureCreateRepository,
        private teacherService: TeacherService,
        private studentService: StudentService,
    ) {}

    async syncAllPersonsWithNoCreateWorkflow() {
        console.log(`Started: ${Date.now()}`);
        // this script is outdated and no longer nessesary.
        const institutionsCount = await this.insertAzureCreateRepository.getAllAzureAccountsWithNoCreateWorkflowCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalCount = institutionsCount;
        let currentTotalCount = institutionsCount;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let persons: PersonResponseDTO[];
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                persons = await this.insertAzureCreateRepository.getAllAzureAccountsWithNoCreateWorkflow({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }

            for (const person of persons) {
                let result, result2;
                const { personID } = person;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                try {
                    result = await this.insertTeacherUser(personID);
                } catch (e) {
                    try {
                        result = await this.insertStudentUser(personID);
                    } catch (e) {
                        console.log(e);
                    }
                }
                if (!result) {
                    console.log(`${currentArrayIndex}. Skipping. Cannot find personID: ${personID}`);
                    failedTotalCount++;
                    continue;
                }
                try {
                    result2 = await this.updateAzureUser(result);
                } catch (e) {
                    console.log(e);
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Inserted Person: ${personID}`);
            }
        }
        await this.fillUsersAzureIDs();
        await this.fillUsersPersonIDs();
        await this.fillClassesAzureIDs();
        await this.fillOrganizationsAzureIDs();
        await this.linkEnrollmentsWithOtherEntities();

        console.log(`Ended: ${Date.now()}`);
    }

    async insertStudentUser(personID: number) {
        const student = await this.studentService.getStudentByPersonID(personID);
        if (!student?.personID) throw new DataNotFoundException();
        const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
        azureStudentDTO.status = EventStatus[EventStatus[EventStatus.SYNCHRONIZED]];
        const result = this.insertAzureCreateRepository.insertAzureCreate(azureStudentDTO);
        return result;
    }

    async insertTeacherUser(personID: number) {
        const teacher = await this.teacherService.getTeacherByPersonID(personID);
        if (!teacher?.personID) throw new DataNotFoundException();
        const azureTeacherDTO = AzureUserResponseFactory.createFromTeacherResponseDTO(teacher);
        azureTeacherDTO.status = EventStatus[EventStatus[EventStatus.SYNCHRONIZED]];
        const result = await this.insertAzureCreateRepository.insertAzureCreate(azureTeacherDTO);
        return result;
    }

    async updateAzureUser(dto: AzureUsersResponseDTO) {
        dto.status = EventStatus[EventStatus[EventStatus.SYNCHRONIZED]];
        const result = await this.insertAzureCreateRepository.updateAzureUser(dto);
        return result;
    }

    async fillUsersAzureIDs() {
        await this.insertAzureCreateRepository.fillUsersAzureIDs();
    }

    async fillUsersPersonIDs() {
        await this.insertAzureCreateRepository.fillUsersPersonIDs();
    }

    async fillClassesAzureIDs() {
        await this.insertAzureCreateRepository.fillClassesAzureIDs();
    }

    async fillOrganizationsAzureIDs() {
        await this.insertAzureCreateRepository.fillOrganizationsAzureIDs();
    }

    async linkEnrollmentsWithOtherEntities() {
        await this.insertAzureCreateRepository.linkEnrollmentsWithOtherEntities();
    }
}

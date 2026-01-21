/* eslint-disable @typescript-eslint/no-loop-func */
import { Injectable } from '@nestjs/common';
import { InjectConnection } from '@nestjs/typeorm';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { EnrollmentUserToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-class-create-request.dto';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { ClassesResponseDTO } from 'src/common/dto/responses/classes-response.dto';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { Connection, EntityManager, getManager } from 'typeorm';
import { SyncCurriculumOptions } from './sync-curriculums-without-azureid.command';
import { SyncCurriculumsWithoutAzureIDRepository } from './sync-curriculums-without-azureid.repository';

@Injectable()
export class SyncCurriculumsWithoutAzureIDService {
    public readConnection: Connection;

    public writeConnection: Connection;

    entityManager = getManager();

    constructor(
        private scwRepository: SyncCurriculumsWithoutAzureIDRepository,
        @InjectConnection('mssql-read') readConnection: Connection,
        @InjectConnection('mssql-write') writeConnection: Connection,
    ) {
        this.readConnection = readConnection;
        this.writeConnection = writeConnection;
    }

    async setSyncCurriculumOptions(options: SyncCurriculumOptions) {
        this.scwRepository.setSyncCurriculumOptions(options);
    }

    async syncCurriculumsWithoutAzureIDs(options: SyncCurriculumOptions) {
        this.setSyncCurriculumOptions(options);
        const failedCurriculums = [];
        /*
        These lines were commented out because we plan on using the script from multiple isntances to be more parallel and we need them to be executed only once. so in this case its best to run these queries in dbeaver manualy.
*/
        // await this.scwRepository.dropCurriculumTempTable();
        // await this.scwRepository.createCurriculumTempTable();
        // await this.scwRepository.fillCurriculumTempTable();
        const curriculumIDsCount = await this.scwRepository.getCurriculumsWithoutAzureIDCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalCount = curriculumIDsCount[0].count;
        let currentTotalCount = curriculumIDsCount[0].count;
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let curriculumIDs;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                curriculumIDs = await this.scwRepository.getCurriculumsWithoutAzureID({
                    from: syncedTotalCount + failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e?.message);
                console.log(`FAILED to: getCurriculumsWithoutAzureID`);
                continue;
            }
            const currentSchoolYear = await this.getCurrentSchoolYear();
            const termEndYear = (await this.getCurrentSchoolYear()) + 1;
            // take small portions of data to avoid blocking the app.
            for (const currentElement of curriculumIDs) {
                const curriculumID = currentElement.CurriculumID;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                console.log(`${currentArrayIndex}. Syncing class: ${curriculumID}`);
                currentTotalCount--;
                try {
                    let resultClasses: AzureClassesResponseDTO;
                    const curriculums = await this.scwRepository.getCurriculumsByCurriculumID(curriculumID);
                    const generatedClassDTO = await this.generateClassesDTOFromSubjects(curriculums);
                    const azureClassesDto: AzureClassesResponseDTO = {
                        title: generatedClassDTO.generatedClassTitle.substring(0, 230),
                        orgID: generatedClassDTO.institutionID.toString(),
                        termStartDate: generatedClassDTO.startDate || `${currentSchoolYear}-09-15`,
                        termEndDate: generatedClassDTO.endDate || `${termEndYear}-06-30`,
                        classID: `${curriculumID}`,
                        azureID: null,
                    };
                    await this.writeConnection.transaction(async (manager) => {
                        resultClasses = await this.scwRepository.insertAzureClass(azureClassesDto, manager);
                        if (!resultClasses?.rowID) throw new EntityNotCreatedException();
                    });
                } catch (e) {
                    console.log(`${currentArrayIndex}. FAILED to sync class: ${curriculumID}`);
                    await this.scwRepository.markCurriculumAsFailed(curriculumID);
                    failedCurriculums.push(curriculumID);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Synced class: ${curriculumID}`);
            }
        }
        console.log(`Number of failed records: ${failedTotalCount}`);
        console.log(`Ended: ${Date.now()}`);
    }

    async syncEnrollmentsForCurriculumsWithoutAzureIDs(options: SyncCurriculumOptions) {
        this.setSyncCurriculumOptions(options);
        const failedCurriculums = [];
        // await this.scwRepository.dropCurriculumTempTable();
        // await this.scwRepository.createCurriculumTempTable();
        // await this.scwRepository.fillCurriculumTempTable();
        const curriculumIDsCount = await this.scwRepository.getCurriculumsWithoutAzureIDCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalCount = curriculumIDsCount[0].count;
        let currentTotalCount = curriculumIDsCount[0].count;
        let currentArrayIndex = 0;
        const syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let curriculumIDs;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                curriculumIDs = await this.scwRepository.getCurriculumsWithoutAzureID({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e?.message);
                console.log(`FAILED TO: getCurriculumsWithoutAzureID `);
                continue;
            }
            // take small portions of data to avoid blocking the app.
            for (const currentElement of curriculumIDs) {
                const curriculumID = currentElement.CurriculumID;
                currentArrayIndex = failedTotalCount;
                console.log(`${currentArrayIndex}. Syncing class: ${curriculumID}`);
                currentTotalCount--;
                try {
                    let resultClasses: AzureClassesResponseDTO;
                    await this.writeConnection.transaction(async (manager) => {
                        const classRow = await this.getCreatedAzureClass(curriculumID);
                        if (!classRow) {
                            throw new DataNotFoundException();
                        }
                        const studentPersonIDs = await this.scwRepository.getStudentPersonIDsByCurriculumID(
                            curriculumID,
                            this.readConnection,
                        );
                        let userRole = UserRoleType.STUDENT;
                        for (const person of studentPersonIDs) {
                            const { personID } = person;
                            console.log(`Creating enrollment for: ${personID} .`);
                            try {
                                await this.createAzureEnrollmentUserToClass(
                                    { personID, curriculumID, userRole },
                                    manager,
                                );
                            } catch (e) {
                                console.log(e?.message);
                                console.log(`${currentArrayIndex}. FAILED to sync personID: ${personID}`);
                            }
                        }
                        const teacherPersonIDs = await this.scwRepository.getTeacherPersonIDsByCurriculumID(
                            curriculumID,
                            this.readConnection,
                        );
                        console.log(
                            `${curriculumID} has ${studentPersonIDs?.length} students and ${teacherPersonIDs?.length} teachers.`,
                        );
                        userRole = UserRoleType.TEACHER;
                        for (const person of teacherPersonIDs) {
                            const { personID } = person;
                            console.log(`Creating enrollment for: ${personID} .`);
                            try {
                                await this.createAzureEnrollmentUserToClass(
                                    { personID, curriculumID, userRole },
                                    manager,
                                );
                            } catch (e) {
                                console.log(e?.message);
                                console.log(`${currentArrayIndex}. FAILED to sync personID: ${personID}`);
                            }
                        }
                        await this.scwRepository.deleteCurriculumsWithoutAzureID(curriculumID, manager);
                    });
                } catch (e) {
                    console.log(`${currentArrayIndex}. FAILED to sync class: ${curriculumID}`);
                    await this.scwRepository.markCurriculumAsFailed(curriculumID);
                    failedTotalCount++;
                    continue;
                }
                console.log(`${currentArrayIndex}. Synced class: ${curriculumID}`);
            }
        }
        console.log(`Number of failed records: ${failedTotalCount}`);
        console.log(`Ended: ${Date.now()}`);
    }

    async getCreatedAzureClass(curriculumID: number) {
        const result = await this.scwRepository.getCreatedAzureClass({ classID: curriculumID.toString() });
        return result;
    }

    async getCurrentSchoolYear() {
        const result = await this.entityManager.query(
            `
            
            SELECT 
                TOP 1 CurrentYearID as currentYearID
            FROM inst_basic.CurrentYear cy
            WHERE cy.IsValid =1 
            ORDER BY CurrentYearID DESC
            `,
        );
        return result[0].currentYearID;
    }

    generateClassTitle(classesResponseDTO: ClassesResponseDTO[]) {
        // looks at all rows from the database and concats a name for the class
        let name = `${classesResponseDTO[0].subjectName} - `;
        if (classesResponseDTO.length > 1) {
            for (let i = 0; i < classesResponseDTO.length; i += 1) {
                if (i === 0) {
                    name += classesResponseDTO[i].className;
                } else {
                    name += `, ${classesResponseDTO[i].className}`;
                }
            }
        } else {
            name += classesResponseDTO[0].className;
        }
        name = `${name} - ${classesResponseDTO[0].subjectTypeName}`;
        return name;
    }

    async generateClassesDTOFromSubjects(curriculums: ClassesResponseDTO[]) {
        const generatedClassTitle = await this.generateClassTitle(curriculums);

        const result: ClassesResponseDTO = {
            institutionAzureID: curriculums[0]?.institutionAzureID,
            institutionID: curriculums[0]?.institutionID,
            generatedClassTitle,
            curriculumID: curriculums[0]?.curriculumID,
            azureID: curriculums[0]?.azureID,
            startDate: curriculums[0]?.startDate,
            endDate: curriculums[0]?.endDate,
        };
        return result;
    }

    async getCreatedAzureUser(personID: number) {
        const result = await this.readConnection.manager.query(
            `
            SELECT
                RowID,
                UserID
            FROM
                azure_temp.Users
            WHERE
                1 = 1
                AND PersonID = @1
                AND WorkflowType = @0
                AND Status <> ${EventStatus.FAILED_USERNAME_GENERATION}
                AND Status <> ${EventStatus.IN_USERNAME_GENERATION}
            ORDER BY
                CreatedOn DESC
            `,
            [WorkflowType.USER_CREATE, personID],
        );
        return { rowID: result[0].RowID, userID: result[0].UserID };
    }

    async createAzureEnrollmentUserToClass(dto: EnrollmentUserToClassCreateRequestDTO, entityManager?: EntityManager) {
        const azureEnrollmentDTO: AzureEnrollmentsResponseDTO = {
            curriculumID: dto.curriculumID,
            organizationPersonID: null,
            userPersonID: dto.personID,
            userRole: dto.userRole,
        };
        await this.insertAzureClassEnrollment(azureEnrollmentDTO, entityManager);
    }

    async insertAzureClassEnrollment(dto: AzureEnrollmentsResponseDTO, entityManager?: EntityManager) {
        await entityManager.query(
            `           
                INSERT
                INTO
                azure_temp.Enrollments (
                    WorkflowType,
                    UserRole,
                    UserPersonID,
                    CurriculumID
                )
                OUTPUT 
                    INSERTED.RowID as rowID
                VALUES (@0,@1,@2,@3);s
            `,
            [WorkflowType.ENROLLMENT_CLASS_CREATE, dto.userRole, dto.userPersonID, dto.curriculumID],
        );
    }
}

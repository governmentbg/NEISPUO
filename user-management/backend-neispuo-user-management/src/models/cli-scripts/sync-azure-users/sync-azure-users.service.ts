/* eslint-disable prefer-const */
import { Injectable, Logger } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { GraphApiUserTypeEnum } from 'src/common/constants/enum/graph-api-user-type';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { IAzureSyncUserSearchInput } from 'src/common/dto/azure-sync-user-search-input.dto';
import { Paging } from 'src/common/dto/paging.dto';
import { SysUserCreateDTO } from 'src/common/dto/sys-user-create.dto';
import { StripZeroesService } from 'src/common/services/strip-zeroes/strip-zeroes.service';
import { DateToUTCTransformService } from 'src/common/services/to-utc-date-transform/status-transform.service';
import { AzureStudentService } from 'src/models/azure/azure-student/routing/azure-student.service';
import { AzureTeacherService } from 'src/models/azure/azure-teacher/routing/azure-teacher.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { SyncUserOptions, SyncUserPosition } from './sync-azure-users.command';
import { SyncAzureUsersRepository } from './sync-azure-users.repository';

@Injectable()
export class SyncAzureUsersService {
    constructor(
        private syncAzureUsersRepository: SyncAzureUsersRepository,
        private graphApiService: GraphApiService,
        private azureTeacherService: AzureTeacherService,
        private azureStudentService: AzureStudentService,
    ) {}

    async getNonSyncedUsersPersonalIDs(paging: Paging) {
        return this.syncAzureUsersRepository.getNonSyncedUsersPersonalIDs(paging);
    }

    async getNonSyncedUsersPersonalIDsCount() {
        return this.syncAzureUsersRepository.getNonSyncedUsersPersonalIDsCount();
    }

    async updateUserPublicEduNumber(publicEduNumber: string, personalID: string) {
        return this.syncAzureUsersRepository.updateUserPublicEduNumber(publicEduNumber, personalID);
    }

    async updateUserAzureID(azureID: string, personalID: string) {
        return this.syncAzureUsersRepository.updateUserAzureID(azureID, personalID);
    }

    async getSysUserByPersonID(personID: number) {
        return this.syncAzureUsersRepository.getSysUserByPersonID(personID);
    }

    async createSysUser(createSysUserDTO: SysUserCreateDTO) {
        return this.syncAzureUsersRepository.createSysUser(createSysUserDTO);
    }

    async getInstitutionWithoutSyncedTeachers(validFrom: Date) {
        const formattedDate = DateToUTCTransformService.transform(validFrom).toISOString();
        return this.syncAzureUsersRepository.getInstitutionWithoutSyncedTeachers(formattedDate);
    }

    async getInstitutionWithoutSyncedStudents(validFrom: Date) {
        const formattedDate = DateToUTCTransformService.transform(validFrom).toISOString();
        return this.syncAzureUsersRepository.getInstitutionWithoutSyncedStudents(formattedDate);
    }

    async getNonSyncedAzureUser(dto: IAzureSyncUserSearchInput) {
        const { position, institutionID } = dto;
        if (position === 'teacher') {
            return this.syncAzureUsersRepository.getNonSyncedTeacherUserByInstitutionID(institutionID);
        } else if (position === 'student') {
            return this.syncAzureUsersRepository.getNonSyncedStudentUserByInstitutionID(institutionID);
        } else {
            throw new Error(`Invalid dto param supplied.`);
        }
    }

    async azureUserRecordExists(personalID: string) {
        const result = await this.syncAzureUsersRepository.azureUserRecordExists(personalID);
        if (result?.length > 0) {
            return true;
        }
        return false;
    }

    async syncExistingNEISPUOUsersWithAzureUsers() {
        console.log(`Started: ${Date.now()}`);
        const personalIDsCount = await this.getNonSyncedUsersPersonalIDsCount();
        const itemsPerPage = CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_GET_PERSONAL_IDS;
        const totalPages = personalIDsCount / itemsPerPage;
        const totalCount = personalIDsCount;
        let currentTotalCount = personalIDsCount;
        // estimated to run for 30 seconds for every 100 personalIDs.
        // 2 days for 500 000 aproximately
        let currentArrayIndex = 0;
        let syncedTotalCount = 0;
        let failedTotalCount = 0;
        while (currentTotalCount > 0) {
            let personalIDs;
            try {
                console.log(`CURRENT: ${currentArrayIndex} ---- TOTAL:  ${totalCount} `);
                personalIDs = await this.getNonSyncedUsersPersonalIDs({
                    from: failedTotalCount,
                    numberOfElements: itemsPerPage,
                });
            } catch (e) {
                console.log(e);
                console.log(`Exiting script due to faulure `);
                break;
            }
            // take small portions of data to avoid blocking the app.
            for (const currentElement of personalIDs) {
                let result, result2, result3, result4;
                const personalID = currentElement.PersonalID;
                currentArrayIndex = syncedTotalCount + failedTotalCount;
                currentTotalCount--;
                const strippedPersonalID = StripZeroesService.transform(personalID);
                try {
                    result = await this.graphApiService.callUserInfoByPersonalIDEndpoint(
                        strippedPersonalID,
                        GraphApiUserTypeEnum.ALL,
                    );
                } catch (e) {
                    failedTotalCount++;
                    continue;
                }
                // if there is no graph api result dont update DB
                if (!result?.response) {
                    console.log(`${currentArrayIndex}. Skipping. No Azure object for personalID: ${personalID}`);
                    failedTotalCount++;
                    continue;
                }
                const { mailNickname, userPrincipalName } = result.response;
                const publicEduNumber = mailNickname;
                try {
                    result2 = await this.updateUserPublicEduNumber(publicEduNumber, personalID);
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                if (!result2?.personID) continue;
                try {
                    result3 = await this.getSysUserByPersonID(result2.personID);
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                if (result3?.sysUserID) continue;
                const sysUserCreateDTO: SysUserCreateDTO = {
                    username: userPrincipalName,
                    personID: result2.personID,
                };
                try {
                    result4 = await this.createSysUser(sysUserCreateDTO);
                } catch (e) {
                    console.log(e);
                    failedTotalCount++;
                    continue;
                }
                syncedTotalCount++;
                console.log(`${currentArrayIndex}. Synced user: ${mailNickname}`);
            }
        }
        console.log(`Ended: ${Date.now()}`);
    }

    async syncNEISPUOUsersWithAzure(dto: IAzureSyncUserSearchInput) {
        console.time('process-users');
        const nonSyncedPersons = await this.getNonSyncedAzureUser(dto);
        const { position, institutionID } = dto;
        console.info(`
Beginning sync for ${nonSyncedPersons.length} total users for institutionID ${institutionID}`);
        for (const person of nonSyncedPersons) {
            const { personalID, personID, validFrom, azureID, publicEduNumber } = person;
            try {
                // const strippedPersonalID = StripZeroesService.transform(personalID);
                const createAzureUserRecordExists = await this.azureUserRecordExists(personalID);
                if (createAzureUserRecordExists) {
                    console.log(`Record already exists for USER_CREATE for ${personalID}. Skipping...`);
                    continue;
                }

                const userInfoResult = await this.graphApiService.callUserInfoByPersonalIDEndpoint(
                    personalID,
                    GraphApiUserTypeEnum.ALL,
                );

                if (!userInfoResult?.response) {
                    console.log(`No Azure object for personalID: ${personalID}.`);
                    if (
                        !!dto.enableUserManagementSync &&
                        new Date(validFrom).getTime() >= new Date(dto.fromPersonCreationDate).getTime()
                    ) {
                        if (position === 'teacher') {
                            console.log(`Calling azureTeacherService.createAzureTeacher for person ${personID}`);
                            await this.azureTeacherService.createAzureTeacher({ personID });
                        } else if (position === 'student') {
                            console.log(`Calling azureStudentService.createAzureStudent for person ${personID}`);
                            await this.azureStudentService.createAzureStudent({ personID });

                            console.log(
                                `Calling azureStudentService.createAzureEnrollmentSchool for person ${personID} and institutionID ${dto.institutionID}`,
                            );
                            await this.azureStudentService.createAzureEnrollmentSchool({
                                userRole: UserRoleType.STUDENT,
                                personID,
                                institutionID,
                            });
                        }
                    }
                    continue;
                }
                const { mailNickname, userPrincipalName, id } = userInfoResult.response;
                const publicEduNumber = mailNickname;

                const updatedPerson = await this.updateUserPublicEduNumber(publicEduNumber, personalID);
                if (!azureID) await this.updateUserAzureID(id, personalID);
                const sysUser = await this.getSysUserByPersonID(updatedPerson.personID);
                if (!!sysUser?.sysUserID) {
                    console.log(`Skipping. User already exists ${sysUser.sysUserID} for ${personalID}`);
                    continue;
                }

                const sysUserCreateDTO: SysUserCreateDTO = {
                    username: userPrincipalName,
                    personID: updatedPerson.personID,
                };

                const createdSysUser = await this.createSysUser(sysUserCreateDTO);
                console.log(`Successfully created user ${createdSysUser.username} for ${personalID}.`);
            } catch (e) {
                console.log(`Failed to sync ${personalID}.\nError: ${e}`);
            }
        }

        console.info(`Processed ${nonSyncedPersons.length} total users for institutionID ${institutionID}`);
        console.timeEnd('process-users');
    }

    async syncUsers(options: SyncUserOptions) {
        let { position, date, school, sync } = options;
        date = date ?? '2019-12-12';
        const isTeacher = position === SyncUserPosition.TEACHER;
        const isStudent = position === SyncUserPosition.STUDENT;
        const isBothTypes = position === SyncUserPosition.ALL || !position;
        const isSchoolPassed = !isNaN(school);
        const passedParams = {
            institutionID: school,
            position,
            enableUserManagementSync: true,
            fromPersonCreationDate: new Date(date),
        } as IAzureSyncUserSearchInput;
        if (isBothTypes) {
            Logger.log(`Syncing all teacher and student users for all institutions...`);
            await this.syncExistingNEISPUOUsersWithAzureUsers();
        } else if ((isTeacher || isStudent) && isSchoolPassed) {
            Logger.log(`Syncing all ${position} for institution ${school}...`);
            await this.syncNEISPUOUsersWithAzure(passedParams);
        } else if (isTeacher && !isSchoolPassed) {
            const teacherInstitutionsToSync = await this.getInstitutionWithoutSyncedTeachers(new Date(date));
            Logger.log(`Syncing all ${position} for institution ${teacherInstitutionsToSync.length}...`);
            for (const institution of teacherInstitutionsToSync) {
                await this.syncNEISPUOUsersWithAzure({
                    ...passedParams,
                    institutionID: institution.InstitutionID,
                });
            }
        } else if (isStudent && !isSchoolPassed) {
            const studentsInstitutionsToSync = await this.getInstitutionWithoutSyncedStudents(new Date(date));
            Logger.log(`Syncing all ${position} for institution ${studentsInstitutionsToSync.length}...`);
            for (const institution of studentsInstitutionsToSync) {
                await this.syncNEISPUOUsersWithAzure({
                    ...passedParams,
                    institutionID: institution.InstitutionID,
                });
            }
        } else {
            throw new Error(`Invalid input arguments supplied. E.g. npm run script all OR npm run sync teacher 12345`);
        }
    }

    async createTeachers() {
        const teachers = await this.getTeachersForCreation();
        for (const teacher of teachers) {
            try {
                await this.azureTeacherService.createAzureTeacher({ personID: teacher?.personID });
            } catch (e) {
                Logger.error(e);
            }
        }
    }

    async createStudents() {
        const students = await this.getStudentsForCreation();
        for (const student of students) {
            try {
                await this.azureStudentService.createAzureStudent({ personID: student?.personID });
            } catch (e) {
                Logger.error(e);
            }
        }
    }

    async getTeachersForCreation() {
        return this.syncAzureUsersRepository.getTeachersForCreation();
    }

    async getStudentsForCreation() {
        return this.syncAzureUsersRepository.getStudentsForCreation();
    }
}

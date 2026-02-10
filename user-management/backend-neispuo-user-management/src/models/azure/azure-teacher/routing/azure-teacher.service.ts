import { Inject, Injectable, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { EnrollmentTeacherToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-class-create-request.dto';
import { EnrollmentTeacherToClassDeleteRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-class-delete-request.dto';
import { EnrollmentTeacherToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-school-create-request.dto';
import { EnrollmentTeacherToSchoolDeleteRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-school-delete-request.dto';
import { EnrollmentUserToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-school-create-request.dto';
import { TeacherCreateRequestDTO } from 'src/common/dto/requests/teacher-create-request.dto';
import { DeleteTeacherDtoRequest } from 'src/common/dto/requests/teacher-delete-request.dto';
import { TeacherUpdateRequestDTO } from 'src/common/dto/requests/teacher-update-request.dto';
import { TeacherUpdateRoleRequestDTO } from 'src/common/dto/requests/teacher-update-role-request.dto';
import { SysUserCreateDTO } from 'src/common/dto/sys-user-create.dto';
import { AlreadyInCreationException } from 'src/common/exceptions/already-in-creation.exception';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { UserAlreadyExistsException } from 'src/common/exceptions/user-already-exists.exception';
import { UserNotFoundException } from 'src/common/exceptions/user-not-found.exception';
import { AzureUserResponseFactory } from 'src/common/factories/azure-user-response-dto.factory';
import { SyncEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/sync-enrollments-service';
import { ClassService } from 'src/models/class/routing/class.service';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { EntitiesInGenerationService } from 'src/models/entities-in-generation/routing/entities-in-generation.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { RoleManagementService } from 'src/models/role-management/routing/role-management.service';
import { TeacherService } from 'src/models/teacher/routing/teacher.service';
import { UserService } from 'src/models/user/routing/user.service';
import { Connection, EntityManager } from 'typeorm';
import { AzureEnrollmentsService } from '../../azure-enrollments/routing/azure-enrollments.service';
import { AzureUsersService } from '../../azure-users/routing/azure-users.service';
import { AzureTeachersRepository } from '../azure-teacher.repository';

@Injectable()
@EnableDatabaseLogging({
    excludedMethods: ['getUnemployedTeachersForDelete'],
})
export class AzureTeacherService {
    constructor(
        private azureTeacherRepository: AzureTeachersRepository,
        @Inject(forwardRef(() => UserService))
        private userService: UserService,
        private teacherService: TeacherService,
        @Inject(forwardRef(() => AzureEnrollmentsService))
        private azureEnrollmentService: AzureEnrollmentsService,
        private roleManagementService: RoleManagementService,
        private azureUserService: AzureUsersService,
        private graphApiService: GraphApiService,
        private personService: PersonService,
        private classService: ClassService,
        private syncEnrollmentsService: SyncEnrollmentsService,
        private educationalStateService: EducationalStateService,
        private connection: Connection,
        private entitiesInGenerationService: EntitiesInGenerationService,
    ) {}

    async createAzureTeacher(dto: TeacherCreateRequestDTO, entityManager?: EntityManager, request?: AuthedRequest) {
        const teacher = await this.teacherService.getTeacherByPersonID(dto.personID);
        const manager = entityManager;
        if (!teacher?.personID) throw new DataNotFoundException();
        const entityInGenerationDTO = { identifier: dto.personID.toString() };
        const alreadyExists = await this.entitiesInGenerationService.entitiesInGenerationExists(entityInGenerationDTO);
        if (alreadyExists) throw new AlreadyInCreationException();
        const azureTeacherDTO = AzureUserResponseFactory.createFromTeacherResponseDTO(teacher);
        let result1, result2, result3;
        await this.connection.transaction(async (manager) => {
            result1 = await this.azureTeacherRepository.insertAzureTeacher(azureTeacherDTO, manager, request);
            if (!result1?.rowID) throw new EntityNotCreatedException();
            const { rowID: userRowID } = result1;
            azureTeacherDTO.rowID = userRowID;
            result2 = await this.azureUserService.generateUsername(azureTeacherDTO, manager);
            if (this.azureUserService.userHasFailedUserNameGeneration(result2)) throw new UserAlreadyExistsException();
            const enrollmentRequestDTO: EnrollmentUserToSchoolCreateRequestDTO = {
                userRole: UserRoleType.TEACHER,
                personID: teacher.personID,
                institutionID: +teacher.institutionID,
            };
            result3 = await this.azureEnrollmentService.createAzureEnrollmentUserToSchool(
                enrollmentRequestDTO,
                manager,
            );
            await this.entitiesInGenerationService.insertEntitiesInGeneration({ identifier: dto.personID.toString() });
        });
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: result1.rowID,
                [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: result3.data,
            },
        };
    }

    async deleteAzureTeacher(teacherDtoRequest: DeleteTeacherDtoRequest, request: AuthedRequest) {
        const teacher = await this.teacherService.getTeacherByPersonID(teacherDtoRequest.personID);
        if (!teacher?.personID) throw new DataNotFoundException();
        const azureTeacherDTO = AzureUserResponseFactory.createFromTeacherResponseDTO(teacher);
        azureTeacherDTO.isForArchivation = azureTeacherDTO?.azureID ? 0 : 1;
        const result = await this.azureTeacherRepository.deleteAzureTeacher(azureTeacherDTO, request);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: result.rowID } };
    }

    async deleteAzureEnrollmentTeacherToSchool(dto: EnrollmentTeacherToSchoolDeleteRequestDTO) {
        const { institutionID, personID } = dto;
        const result = await this.azureEnrollmentService.deleteAzureEnrollmentUserToSchool(dto);
        const sysUser = await this.userService.getSysUserByPersonID(personID);
        if (!sysUser) {
            console.error(`SysUser with ${personID} was not found for deleteAzureEnrollmentTeacherToSchool.`);
            return;
        }
        const institutionSysUserSysRoleDto = {
            sysRoleID: RoleEnum.INSTITUTION,
            sysUserID: sysUser.sysUserID,
            institutionID,
            isDeleted: true,
        };

        const accountantSysUserSysRoleDto = {
            sysRoleID: RoleEnum.ACCOUNTANT,
            sysUserID: sysUser.sysUserID,
            institutionID,
            isDeleted: true,
        };
        try {
            //there is an exception in removeRole which is thrown an we dont want it to be thrown
            await this.roleManagementService.removeRole(institutionSysUserSysRoleDto);
            await this.roleManagementService.removeRole(accountantSysUserSysRoleDto);
        } catch (e) {}
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: result?.data } };
    }

    async createAzureEnrollmentTeacherToSchool(enrollmentRequestDTO: EnrollmentTeacherToSchoolCreateRequestDTO) {
        const teacher = await this.teacherService.getTeacherByPersonID(enrollmentRequestDTO.personID);
        if (!teacher?.personID) throw new UserNotFoundException();
        const result = await this.azureEnrollmentService.createAzureEnrollmentUserToSchool(enrollmentRequestDTO);
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: result.data } };
    }

    async createAzureEnrollmentTeacherToClass(dto: EnrollmentTeacherToClassCreateRequestDTO) {
        const results = [];
        const { personID, curriculumIDs, userRole } = dto;
        const teacher = await this.teacherService.getTeacherByPersonID(personID);
        if (!teacher?.personID) throw new UserNotFoundException();
        await this.connection.transaction(async (manager) => {
            for (const curriculumID of curriculumIDs) {
                const result = await this.azureEnrollmentService.createAzureEnrollmentUserToClass(
                    {
                        userRole,
                        personID,
                        curriculumID,
                    },
                    manager,
                );
                results.push(result.data);
            }
        });
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: results } };
    }

    async deleteAzureEnrollmentTeacherToClass(enrollmentRequestDTO: EnrollmentTeacherToClassDeleteRequestDTO) {
        const resultEnrollments = [];
        const { personID, curriculumIDs, userRole } = enrollmentRequestDTO;
        const teacher = await this.teacherService.getTeacherByPersonID(personID);
        if (!teacher?.personID) throw new UserNotFoundException();
        await this.connection.transaction(async (manager) => {
            for (const curriculumID of curriculumIDs) {
                if (Number.isInteger(curriculumID) && curriculumID >= 2147483647) continue;
                const result = await this.azureEnrollmentService.deleteAzureEnrollmentUserToClass(
                    {
                        userRole,
                        personID,
                        curriculumID,
                    },
                    manager,
                );
                if (!result?.data) throw new EntityNotCreatedException();
                resultEnrollments.push(result.data);
            }
        });

        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: resultEnrollments,
            },
        };
    }

    async updateAzureTeacher(teacherDtoRequest: TeacherUpdateRequestDTO, request?: AuthedRequest) {
        const { personID } = teacherDtoRequest;
        const teacher = await this.teacherService.getTeacherByPersonID(personID);
        if (!teacher?.personID) throw new DataNotFoundException();
        const sysUserID = await this.teacherService.hasAccountantRole(personID);
        if (sysUserID) {
            const institutions = await this.teacherService.getAccountantInstitutions(sysUserID);
            const hasNoInstitutions = institutions?.length === 0;
            const isTeacher = await this.educationalStateService.isTeacher({ personID: teacher?.personID });
            teacher.assignedAccountantSchools = institutions.join(',');
            if (isTeacher) teacher.additionalRole = RoleEnum.TEACHER;
            if (hasNoInstitutions && !isTeacher) teacher.additionalRole = null;
            if (!hasNoInstitutions && isTeacher) teacher.sisAccessSecondaryRole = RoleEnum.ACCOUNTANT;
            if (!hasNoInstitutions && !isTeacher) teacher.sisAccessSecondaryRole = RoleEnum.ACCOUNTANT;
        }
        const hasVURole = await this.teacherService.hasVURole(personID);
        if (hasVURole) {
            teacher.additionalRole = RoleEnum.VU_TEACHER;
        }
        const azureTeacherDTO = AzureUserResponseFactory.createFromTeacherResponseDTO(teacher);
        azureTeacherDTO.isForArchivation = azureTeacherDTO?.azureID ? 0 : 1;
        const result = await this.azureTeacherRepository.updateAzureTeacher(azureTeacherDTO, request);
        if (!result) throw new EntityNotCreatedException();
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: azureTeacherDTO.rowID } };
    }

    async azureSyncTeacher(dto: TeacherCreateRequestDTO) {
        const userRole = UserRoleType.TEACHER;
        const teacher = await this.teacherService.getTeacherByPersonID(dto.personID);
        if (!teacher?.personID) throw new DataNotFoundException();
        const { personID, azureID, publicEduNumber } = teacher;
        const azureTeacher = await this.syncEnrollmentsService.getUserInfoFromAzure(teacher, false);
        let result;
        let azureClassCurriculumIDs = [];
        let azureSchoolInsitutionIDs = [];
        let hasUsername;
        let sysUserDTO: SysUserCreateDTO;
        const isLocalPublicEduNumberDifferentFromAzure = publicEduNumber !== azureTeacher?.publicEduNumber;
        if (azureTeacher) {
            azureClassCurriculumIDs = await this.syncEnrollmentsService.getAzureClassEnrollmentsByPublicEduNumber(
                azureTeacher?.publicEduNumber,
            );
            azureSchoolInsitutionIDs = await this.syncEnrollmentsService.getAzureSchoolEnrollmentsByPublicEduNumber(
                azureTeacher?.publicEduNumber,
            );
            hasUsername = await this.userService.userNameExists({ username: azureTeacher.email });
            if (!hasUsername) sysUserDTO = { username: azureTeacher.email, personID };
        }
        if (!azureID && !publicEduNumber && azureTeacher) {
            result = await this.personService.updateAzureIDByPersonID(azureTeacher);
            await this.personService.updatePublicEduNumberByPersonID(azureTeacher);
            await this.updateAzureTeacher({ personID });
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        if (!azureID && azureTeacher) {
            result = await this.personService.updateAzureIDByPersonID(azureTeacher);
            await this.personService.updatePublicEduNumberByPersonID(azureTeacher);
            await this.updateAzureTeacher({ personID });
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        if (!publicEduNumber && azureTeacher) {
            result = await this.personService.updateAzureIDByPersonID(azureTeacher);
            await this.personService.updatePublicEduNumberByPersonID(azureTeacher);
            await this.updateAzureTeacher({ personID });
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        if (isLocalPublicEduNumberDifferentFromAzure && azureTeacher) {
            await this.personService.updatePublicEduNumberByPersonID(azureTeacher);
            await this.personService.updateAzureIDByPersonID(azureTeacher);
        }
        if ((!azureID || !publicEduNumber) && !azureTeacher) {
            await this.userService.deleteAzureIDByPersonID(personID);
            await this.userService.deleteSysUserByPersonID(personID);
            result = await this.createAzureTeacher({ personID });
        }
        if (azureID && publicEduNumber && !azureTeacher) {
            /* After Pavkata's deletion script of non active users we hit a problem and we are pasting this code in order to recreate some accounts. */
            await this.userService.deleteAzureIDByPersonID(personID);
            await this.userService.deleteSysUserByPersonID(personID);
            result = await this.createAzureTeacher({ personID });
        }
        if (azureID && publicEduNumber && azureTeacher && !hasUsername) {
            /* there are some users who are missing sysusers but are ok in person and azure */
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        let curriculumIDs = await this.syncEnrollmentsService.getMissingAzureTeacherClassEnrollments(
            personID,
            azureClassCurriculumIDs,
        );
        await this.syncEnrollmentsService.createMissingCurriculums(curriculumIDs);
        await this.createAzureEnrollmentTeacherToClass({ personID, curriculumIDs, userRole });
        curriculumIDs = await this.syncEnrollmentsService.getExtraAzureTeacherClassEnrollments(
            personID,
            azureClassCurriculumIDs,
        );
        await this.deleteAzureEnrollmentTeacherToClass({ personID, curriculumIDs, userRole });

        const enrollments = [];
        let institutionIDs = await this.syncEnrollmentsService.getMissingAzurеSchoolEnrollments(
            personID,
            azureSchoolInsitutionIDs,
        );
        for (const institutionID of institutionIDs) {
            const enrollment = await this.createAzureEnrollmentTeacherToSchool({
                institutionID,
                personID,
                userRole: UserRoleType.TEACHER,
            });
            enrollments.push(enrollment);
        }
        institutionIDs = await this.syncEnrollmentsService.getExtraAzureSchoolEnrollments(
            personID,
            azureSchoolInsitutionIDs,
        );
        for (const institutionID of institutionIDs) {
            const enrollment = await this.deleteAzureEnrollmentTeacherToSchool({
                institutionID,
                personID,
                userRole: UserRoleType.TEACHER,
            });
            enrollments.push(enrollment);
        }

        return {
            data: { [CONSTANTS.RESPONSE_PARAM_NAME_PERSON_ID]: enrollments },
        };
    }

    async getUnemployedTeachersForDelete() {
        const results = [];
        const teachers = await this.userService.getUnemployedTeachersForDelete();

        for (const teacher of teachers) {
            const result = await this.azureTeacherRepository.deleteAzureTeacher(teacher);
            results.push(result);
        }

        return {
            // data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: result[0].PersonalID },
        };
    }

    async addAccountantRole(teacherDtoRequest: TeacherUpdateRoleRequestDTO) {
        const teacher = await this.teacherService.getTeacherByUserID(teacherDtoRequest.sysUserID);
        if (!teacher?.personID) throw new DataNotFoundException();
        // eslint-disable-next-line prefer-const
        let azureTeacherDTO = AzureUserResponseFactory.createFromTeacherResponseDTO(teacher);
        azureTeacherDTO.sisAccessSecondaryRole = RoleEnum.ACCOUNTANT;
        azureTeacherDTO.isForArchivation = azureTeacherDTO?.azureID ? 0 : 1;
        const result = await this.azureTeacherRepository.updateAzureTeacher(azureTeacherDTO);
        if (!result) throw new EntityNotCreatedException();
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: azureTeacherDTO.rowID } };
    }

    async adjustAccountantRole(teacherDtoRequest: TeacherUpdateRoleRequestDTO) {
        const teacher = await this.teacherService.getTeacherByUserID(teacherDtoRequest.sysUserID);
        if (!teacher?.personID) throw new DataNotFoundException();
        const institutions = await this.teacherService.getAccountantInstitutions(teacherDtoRequest.sysUserID);
        const hasNoInstitutions = institutions?.length === 0;
        teacher.assignedAccountantSchools = institutions.join(',');
        const azureTeacherDTO = AzureUserResponseFactory.createFromTeacherResponseDTO(teacher);
        // Do not remove role from azure if the user is accountant in more instituttions
        const isTeacher = await this.educationalStateService.isTeacher({ personID: teacher?.personID });
        if (isTeacher) azureTeacherDTO.additionalRole = RoleEnum.TEACHER;
        if (hasNoInstitutions && !isTeacher) azureTeacherDTO.additionalRole = null;
        if (!hasNoInstitutions && isTeacher) azureTeacherDTO.sisAccessSecondaryRole = RoleEnum.ACCOUNTANT;
        if (!hasNoInstitutions && !isTeacher) azureTeacherDTO.sisAccessSecondaryRole = RoleEnum.ACCOUNTANT;
        azureTeacherDTO.isForArchivation = azureTeacherDTO?.azureID ? 0 : 1;
        const result = await this.azureTeacherRepository.updateAzureTeacher(azureTeacherDTO);
        if (!result) throw new EntityNotCreatedException();

        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: azureTeacherDTO.rowID } };
    }
}

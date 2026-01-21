import { Inject, Injectable, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { EnrollmentStudentToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-student-to-class-create-request.dto';
import { EnrollmentStudentToClassDeleteRequestDTO } from 'src/common/dto/requests/enrollment-student-to-class-delete-request.dto';
import { EnrollmentStudentToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-student-to-school-create-request.dto';
import { EnrollmentStudentToSchoolDeleteRequestDTO } from 'src/common/dto/requests/enrollment-student-to-school-delete-request.dto';
import { StudentCreateRequestDTO } from 'src/common/dto/requests/student-create-request.dto';
import { StudentDeleteDisableRequestDTO } from 'src/common/dto/requests/student-delete-disable-request.dto';
import { StudentUpdateRequestDTO } from 'src/common/dto/requests/student-update-request.dto';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { SysUserCreateDTO } from 'src/common/dto/sys-user-create.dto';
import { AlreadyInCreationException } from 'src/common/exceptions/already-in-creation.exception';
import { CustomException } from 'src/common/exceptions/custom.exception';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { PersonIsAttendingException } from 'src/common/exceptions/person-is-attending.exception';
import { UserAlreadyExistsException } from 'src/common/exceptions/user-already-exists.exception';
import { UserNotFoundException } from 'src/common/exceptions/user-not-found.exception';
import { AzureUserResponseFactory } from 'src/common/factories/azure-user-response-dto.factory';
import { SyncEnrollmentsService } from 'src/models/azure/azure-enrollments/routing/sync-enrollments-service';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { EntitiesInGenerationService } from 'src/models/entities-in-generation/routing/entities-in-generation.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { SchoolBookCodeService } from 'src/models/school-book-code/routing/school-book-code.service';
import { StudentService } from 'src/models/student/routing/student.service';
import { UserService } from 'src/models/user/routing/user.service';
import { Connection, EntityManager } from 'typeorm';
import { AzureEnrollmentsService } from '../../azure-enrollments/routing/azure-enrollments.service';
import { AzureTeacherService } from '../../azure-teacher/routing/azure-teacher.service';
import { AzureUsersService } from '../../azure-users/routing/azure-users.service';
import { AzureStudentRepository } from '../azure-student.repository';

@Injectable()
@EnableDatabaseLogging({
    excludedMethods: ['getUnattendingStudentsForDelete'],
})
export class AzureStudentService {
    constructor(
        private azureStudentRepository: AzureStudentRepository,
        @Inject(forwardRef(() => UserService))
        private userService: UserService,
        @Inject(forwardRef(() => StudentService))
        private studentService: StudentService,
        private azureTeacherService: AzureTeacherService,
        private azureEnrollmentsService: AzureEnrollmentsService,
        private azureUsersService: AzureUsersService,
        private connection: Connection,
        private educationalStateService: EducationalStateService,
        private syncEnrollmentsService: SyncEnrollmentsService,
        private personService: PersonService,
        private entitiesInGenerationService: EntitiesInGenerationService,
        private schoolBookCodeService: SchoolBookCodeService,
    ) {}

    async createAzureStudent(dto: StudentCreateRequestDTO, request?: AuthedRequest) {
        const student = await this.studentService.getStudentByPersonID(dto.personID);
        if (!student?.personID) throw new DataNotFoundException();
        const entityInGenerationDTO = { identifier: dto.personID.toString() };
        const alreadyExists = await this.entitiesInGenerationService.entitiesInGenerationExists(entityInGenerationDTO);
        if (alreadyExists) throw new AlreadyInCreationException();
        const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
        const result = await this.azureStudentRepository.insertAzureStudent(azureStudentDTO, request);
        if (!result?.rowID) throw new EntityNotCreatedException();
        const { rowID } = result;
        azureStudentDTO.rowID = rowID;
        await this.entitiesInGenerationService.insertEntitiesInGeneration(entityInGenerationDTO);
        const result2 = await this.azureUsersService.generateUsername(azureStudentDTO);
        if (this.azureUsersService.userHasFailedUserNameGeneration(result2)) throw new UserAlreadyExistsException();
        await this.schoolBookCodeService.assignSchoolBookCodes(
            {
                personIDs: [dto.personID],
            },
            request,
        );
        return {
            data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: result.rowID },
        };
    }

    async updateAzureStudent(
        updateUserRequestDTO: StudentUpdateRequestDTO,
        entityManager?: EntityManager,
        request?: AuthedRequest,
    ) {
        const student = await this.studentService.getStudentByPersonID(updateUserRequestDTO.personID);
        if (!student?.personID) throw new DataNotFoundException();
        const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
        const studentGrade = await this.studentService.getStudentGradeByPersonID(student.personID, entityManager);
        azureStudentDTO.grade = studentGrade?.grade;
        const result = await this.azureStudentRepository.updateAzureStudent(azureStudentDTO, entityManager, request);
        if (!result) throw new EntityNotCreatedException();
        return {
            data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: azureStudentDTO.rowID },
        };
    }

    async createAzureEnrollmentSchool(enrollmentRequestDTO: EnrollmentStudentToSchoolCreateRequestDTO) {
        const student = await this.studentService.getStudentByPersonID(enrollmentRequestDTO.personID);
        if (!student?.personID) throw new DataNotFoundException();
        const result = await this.azureEnrollmentsService.createAzureEnrollmentUserToSchool(enrollmentRequestDTO);
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: result.data } };
    }

    async deleteAzureEnrollmentStudentToSchool(
        enrollmentStudentToSchoolDeleteRequestDTO: EnrollmentStudentToSchoolDeleteRequestDTO,
    ) {
        const student = await this.studentService.getStudentByPersonID(
            enrollmentStudentToSchoolDeleteRequestDTO.personID,
        );
        if (!student?.personID) throw new UserNotFoundException();
        const result = await this.azureEnrollmentsService.deleteAzureEnrollmentUserToSchool(
            enrollmentStudentToSchoolDeleteRequestDTO,
        );
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: result.data } };
    }

    async createAzureEnrollmentStudentToClass(dto: EnrollmentStudentToClassCreateRequestDTO, request?: AuthedRequest) {
        const results = [];
        const { personID, curriculumIDs, userRole } = dto;
        const student = await this.studentService.getStudentByPersonID(personID);
        if (!student?.personID) throw new UserNotFoundException();
        const isTeacher = await this.educationalStateService.isTeacher({
            personID,
        });
        if (isTeacher) {
            throw new CustomException(
                'Enrollment failed: The provided PersonID corresponds to a teacher account. This endpoint only supports student enrollments.',
            );
        }
        await this.connection.transaction(async (manager) => {
            if (dto.basicClassID) {
                student.grade = dto.basicClassID.toString();
                const azureStudent = AzureUserResponseFactory.createFromStudentResponseDTO(student);
                await this.azureStudentRepository.updateAzureStudent(azureStudent, manager, request);
            }
            for (const curriculumID of curriculumIDs) {
                const result = await this.azureEnrollmentsService.createAzureEnrollmentUserToClass(
                    {
                        userRole,
                        personID: student.personID,
                        curriculumID: curriculumID,
                    },
                    manager,
                );
                results.push(result.data);
            }
        });
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: results } };
    }

    async deleteAzureEnrollmentStudentToClass(enrollmentRequestDTO: EnrollmentStudentToClassDeleteRequestDTO) {
        const resultEnrollments = [];
        const { personID, curriculumIDs, userRole } = enrollmentRequestDTO;
        const student = await this.studentService.getStudentByPersonID(personID);
        if (!student?.personID) throw new UserNotFoundException();

        await this.connection.transaction(async (manager) => {
            for (const curriculumID of curriculumIDs) {
                if (Number.isInteger(curriculumID) && curriculumID >= 2147483647) continue;
                // i added the above because of old class names which cannot be inserted into the table. and i dont want to switch column types on azure_temp.CurriculumID
                const result = await this.azureEnrollmentsService.deleteAzureEnrollmentUserToClass(
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

    async deleteOrDisableAzureStudent(dto: StudentDeleteDisableRequestDTO, request?: AuthedRequest) {
        const { personID, positionID } = dto;
        const student = await this.studentService.getStudentByPersonID(personID);
        if (!student?.personID) throw new DataNotFoundException();
        let result;
        student.positionID = positionID;
        const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
        azureStudentDTO.isForArchivation = azureStudentDTO?.azureID ? 0 : 1;
        const isGraduated = await this.studentService.isStudentGraduate(personID);
        if (positionID === PositionEnum.UNATTENTDING) {
            result = await this.disableAzureStudent(azureStudentDTO, request);
        } else if (isGraduated) {
            result = await this.deleteAzureStudent(azureStudentDTO, null, request);
        } else {
            throw new PersonIsAttendingException();
        }
        return {
            data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: result.rowID },
        };
    }

    async deleteAzureStudent(dto: AzureUsersResponseDTO, entityManager?: EntityManager, request?: AuthedRequest) {
        const result = await this.azureStudentRepository.deleteAzureStudent(dto, entityManager, request);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return result;
    }

    async disableAzureStudent(dto: AzureUsersResponseDTO, request?: AuthedRequest) {
        const result = await this.azureStudentRepository.disableAzureStudent(dto, request);
        if (!result?.rowID) throw new EntityNotCreatedException();
        return result;
    }

    async createAzureEnrollmentStudentToSchool(dto: EnrollmentStudentToSchoolCreateRequestDTO) {
        const result = await this.azureEnrollmentsService.createAzureEnrollmentUserToSchool(dto);
        return { data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: [result.data] } };
    }

    async getUnattendingStudentsForDelete() {
        const result = [];
        const students = await this.userService.getUnattendingStudentsForDelete();

        for (const student of students) {
            if (!student?.personID) continue;
            const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
            azureStudentDTO.isForArchivation = azureStudentDTO?.azureID ? 0 : 1;
            const singleResult = await this.azureStudentRepository.deleteAzureStudent(azureStudentDTO);
            if (!singleResult?.rowID) continue;
            result.push(singleResult);
        }

        return {
            // data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: result[0].PersonalID },
        };
    }

    async convertAzureStudentToTeacher(dto: StudentCreateRequestDTO, request: AuthedRequest) {
        const { personID } = dto;
        const customExceptionMessage = `No teacher educational states found for personID:`;
        const student = await this.studentService.getStudentByPersonID(personID);
        if (!student?.personID) throw new DataNotFoundException();
        let result;
        const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
        await this.connection.transaction(async (manager) => {
            result = await this.userService.deleteSysUserByPersonID(personID, manager);
            azureStudentDTO.isForArchivation = azureStudentDTO?.azureID ? 0 : 1;
            result = await this.azureStudentRepository.deleteAzureStudent(dto, manager, request);
            if (!result?.rowID) throw new EntityNotCreatedException();
            const states = await this.educationalStateService.getTeacherEducationalStatesByPersonID({ personID });
            if (!states || states.length === 0) throw new CustomException(`${customExceptionMessage} ${personID}`);
            result = await this.azureTeacherService.createAzureTeacher({ personID }, manager, request);
        });
        return {
            data: {
                [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: result.data,
            },
        };
    }

    async azureSyncStudent(dto: StudentCreateRequestDTO, request: AuthedRequest) {
        const userRole = UserRoleType.STUDENT;
        const student = await this.studentService.getStudentByPersonID(dto.personID);
        if (!student?.personID) throw new DataNotFoundException();
        const { personID, azureID, publicEduNumber } = student;
        const azureStudent = await this.syncEnrollmentsService.getUserInfoFromAzure(student, true);
        let result;
        let azureClassCurriculumIDs = [];
        let azureSchoolInsitutionIDs = [];
        let hasUsername;
        let sysUserDTO: SysUserCreateDTO;
        const isLocalPublicEduNumberDifferentFromAzure = publicEduNumber !== azureStudent?.publicEduNumber;
        if (azureStudent) {
            azureClassCurriculumIDs = await this.syncEnrollmentsService.getAzureClassEnrollmentsByPublicEduNumber(
                azureStudent?.publicEduNumber,
            );
            azureSchoolInsitutionIDs = await this.syncEnrollmentsService.getAzureSchoolEnrollmentsByPublicEduNumber(
                azureStudent?.publicEduNumber,
            );
            hasUsername = await this.userService.userNameExists({ username: azureStudent.email });
            if (!hasUsername) sysUserDTO = { username: azureStudent.email, personID };
        }
        if (!azureID && !publicEduNumber && azureStudent) {
            result = await this.personService.updateAzureIDByPersonID(azureStudent);
            await this.personService.updatePublicEduNumberByPersonID(azureStudent);
            await this.updateAzureStudent({ personID }, null, request);
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        if (!azureID && azureStudent) {
            result = await this.personService.updateAzureIDByPersonID(azureStudent);
            await this.personService.updatePublicEduNumberByPersonID(azureStudent);
            await this.updateAzureStudent({ personID }, null, request);
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        if (!publicEduNumber && azureStudent) {
            result = await this.personService.updateAzureIDByPersonID(azureStudent);
            await this.personService.updatePublicEduNumberByPersonID(azureStudent);
            await this.updateAzureStudent({ personID }, null, request);
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        if (isLocalPublicEduNumberDifferentFromAzure && azureStudent) {
            await this.personService.updatePublicEduNumberByPersonID(azureStudent);
            await this.personService.updateAzureIDByPersonID(azureStudent);
        }
        if ((!azureID || !publicEduNumber) && !azureStudent) {
            await this.userService.deleteAzureIDByPersonID(personID);
            await this.userService.deleteSysUserByPersonID(personID);
            result = await this.createAzureStudent({ personID });
        }
        if (azureID && publicEduNumber && !azureStudent) {
            /* After Pavkata's deletion script of non active users we hit a problem and we are pasting this code in order to recreate some accounts. */
            await this.userService.deleteAzureIDByPersonID(personID);
            await this.userService.deleteSysUserByPersonID(personID);
            result = await this.createAzureStudent({ personID });
        }
        if (azureID && publicEduNumber && azureStudent && !hasUsername) {
            /* there are some users who are missing sysusers but are ok in person and azure */
            if (!hasUsername) await this.userService.createSysUser(sysUserDTO);
        }
        let curriculumIDs = await this.syncEnrollmentsService.getMissingAzureStudentClassEnrollments(
            personID,
            azureClassCurriculumIDs,
        );
        await this.syncEnrollmentsService.createMissingCurriculums(curriculumIDs);
        await this.createAzureEnrollmentStudentToClass(
            { personID, curriculumIDs, userRole, basicClassID: null },
            request,
        );
        curriculumIDs = await this.syncEnrollmentsService.getExtraAzureStudentClassEnrollments(
            personID,
            azureClassCurriculumIDs,
        );
        await this.deleteAzureEnrollmentStudentToClass({ personID, curriculumIDs, userRole });

        const enrollments = [];
        let institutionIDs = await this.syncEnrollmentsService.getMissingAzurеSchoolEnrollments(
            personID,
            azureSchoolInsitutionIDs,
        );
        for (const institutionID of institutionIDs) {
            const enrollment = await this.createAzureEnrollmentSchool({
                institutionID,
                personID,
                userRole: UserRoleType.STUDENT,
            });
            enrollments.push(enrollment);
        }
        institutionIDs = await this.syncEnrollmentsService.getExtraAzureSchoolEnrollments(
            personID,
            azureSchoolInsitutionIDs,
        );
        for (const institutionID of institutionIDs) {
            const enrollment = await this.deleteAzureEnrollmentStudentToSchool({
                institutionID,
                personID,
                userRole: UserRoleType.STUDENT,
            });
            enrollments.push(enrollment);
        }
        return {
            data: { [CONSTANTS.RESPONSE_PARAM_NAME_PERSON_ID]: enrollments },
        };
    }
}

import { Inject, Injectable, forwardRef } from '@nestjs/common';
import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { GraphApiUserTypeEnum } from 'src/common/constants/enum/graph-api-user-type';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { EnrollmentStudentToSchoolCreateRequestDTO } from 'src/common/dto/requests/enrollment-student-to-school-create-request.dto';
import { EnrollmentUserToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-user-to-class-create-request.dto';
import { StudentResponseDTO } from 'src/common/dto/responses/student-response.dto';
import { TeacherResponseDTO } from 'src/common/dto/responses/teacher-response.dto';
import { ClassService } from 'src/models/class/routing/class.service';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { Connection } from 'typeorm';
import { AzureClassesService } from '../../azure-classes/routing/azure-classes.service';
import { AzureEnrollmentsService } from './azure-enrollments.service';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: [
        'getUserInfoFromAzure',
        'getAzureClassEnrollmentsByPublicEduNumber',
        'getAzureSchoolEnrollmentsByPublicEduNumber',
        'getMissingAzureStudentClassEnrollments',
        'getMissingAzurеSchoolEnrollments',
        'getExtraAzureStudentClassEnrollments',
        'getExtraAzureSchoolEnrollments',
        'getMissingAzureTeacherClassEnrollments',
        'getExtraAzureTeacherClassEnrollments',
    ],
})
export class SyncEnrollmentsService {
    constructor(
        private connection: Connection,
        private graphApiService: GraphApiService,
        private educationalStateService: EducationalStateService,
        private classService: ClassService,
        @Inject(forwardRef(() => AzureClassesService))
        private azureClassesService: AzureClassesService,
        private azureEnrollmentService: AzureEnrollmentsService,
    ) {}

    async createMissingCurriculums(curriculumIDs: number[]) {
        for (const curriculumID of curriculumIDs) {
            const curriculums = await this.classService.getCurriculumByCurriculumID(curriculumID);
            const curriculumExists = curriculums?.length > 0;
            const isCurriculumInterger = Number.isInteger(curriculumID);
            if (!curriculumExists || !isCurriculumInterger) return;
            const azureID = curriculums[0]?.azureID;
            const isCurriculumSynced = !!azureID;
            const personIDs = [];
            const institutionID = curriculums[0]?.institutionID;
            if (isCurriculumSynced) {
                const azureCurriculum = await this.graphApiService.getClassInfo(azureID);
                if (azureCurriculum.status === GraphApiResponseEnum.SUCCESS) return;
            }
            await this.azureClassesService.createAzureClassesAndEnrollUsers({
                curriculumID,
                personIDs,
                institutionID,
            });
        }
    }

    async getUserInfoFromAzure(dto: StudentResponseDTO | TeacherResponseDTO, isStudent: boolean) {
        let azureUser;
        const { personID, azureID, publicEduNumber, personalID } = dto;
        if (azureID) {
            azureUser = await this.graphApiService.getUserInfoByAzureID(azureID);
        }
        if (azureUser) return { ...azureUser, personID };
        if (publicEduNumber) {
            azureUser = await this.graphApiService.getUserInfoByPublicEduNumber(publicEduNumber);
        }
        if (azureUser) return { ...azureUser, personID };
        if (personalID) {
            azureUser = isStudent
                ? await this.graphApiService.getUserInfoByPersonalID(personalID, GraphApiUserTypeEnum.STUDENT)
                : (azureUser = await this.graphApiService.getUserInfoByPersonalID(
                      personalID,
                      GraphApiUserTypeEnum.TEACHER,
                  ));
        }
        if (azureUser) return { ...azureUser, personID };
        return null;
    }

    async getAzureClassEnrollmentsByPublicEduNumber(publicEduNumber: string) {
        const azureClassEnrollments = await this.graphApiService.getUserClassEnrollmentInfo(
            `${publicEduNumber}@edu.mon.bg`,
        );
        const azureClassCurriculumIDs = [];
        for (const azureClassEnrollment of azureClassEnrollments?.response) {
            if (+azureClassEnrollment?.externalId) azureClassCurriculumIDs.push(+azureClassEnrollment?.externalId);
        }
        return azureClassCurriculumIDs;
    }

    async getAzureSchoolEnrollmentsByPublicEduNumber(publicEduNumber: string) {
        const azureSchoolEnrollments = await this.graphApiService.getUserSchoolEnrollmentInfo(
            `${publicEduNumber}@edu.mon.bg`,
        );
        const azureClassCurriculumIDs = [];
        for (const azureSchoolEnrollment of azureSchoolEnrollments?.response) {
            if (+azureSchoolEnrollment?.externalId) azureClassCurriculumIDs.push(+azureSchoolEnrollment?.externalId);
        }
        return azureClassCurriculumIDs;
    }

    async getMissingAzureStudentClassEnrollments(personID: number, azureEnrollments: number[]) {
        const classes = await this.classService.getCurriculumsStudentByPersonID(personID);
        const curriculumIDs = classes.map((classes) => classes.curriculumID);
        const missingCurriculums = [];
        for (const curriculumID of curriculumIDs) {
            if (!azureEnrollments.includes(+curriculumID)) missingCurriculums.push(+curriculumID);
        }
        return missingCurriculums;
    }

    async getMissingAzurеSchoolEnrollments(personID: number, azureInstitutionIDs: number[]) {
        const states = await this.educationalStateService.getUserEducationalStatesByPersonID({ personID });

        const missingInstiutions = [];
        for (const state of states) {
            if (!azureInstitutionIDs.includes(+state?.institutionID)) missingInstiutions.push(+state?.institutionID);
        }
        return missingInstiutions;
    }

    async getExtraAzureStudentClassEnrollments(personID: number, azureEnrollments: number[]) {
        const classes = await this.classService.getCurriculumsStudentByPersonID(personID);
        const curriculumIDs = classes.map((classes) => classes.curriculumID);
        const extraCurriculums = [];
        for (const azureEnrollment of azureEnrollments) {
            if (!curriculumIDs.includes(+azureEnrollment)) extraCurriculums.push(+azureEnrollment);
        }
        return extraCurriculums;
    }

    async getExtraAzureSchoolEnrollments(personID: number, azureInstitutionIDs: number[]) {
        const states = await this.educationalStateService.getUserEducationalStatesByPersonID({ personID });
        const institutionIDs = states.map((state) => state.institutionID);
        const extraInstiutions = [];
        for (const azureEnrollment of azureInstitutionIDs) {
            if (!institutionIDs.includes(+azureEnrollment)) extraInstiutions.push(azureEnrollment);
        }
        return extraInstiutions;
    }

    async getMissingAzureTeacherClassEnrollments(personID: number, azureEnrollments: number[]) {
        const classes = await this.classService.getCurriculumsTeacherByPersonID(personID);
        const curriculumIDs = classes.map((classes) => classes.curriculumID);
        const missingCurriculums = [];
        for (const curriculumID of curriculumIDs) {
            if (!azureEnrollments.includes(+curriculumID)) missingCurriculums.push(+curriculumID);
        }
        return missingCurriculums;
    }

    async getExtraAzureTeacherClassEnrollments(personID: number, azureEnrollments: number[]) {
        const classes = await this.classService.getCurriculumsTeacherByPersonID(personID);
        const curriculumIDs = classes.map((classes) => classes.curriculumID);
        const extraCurriculums = [];
        for (const azureEnrollment of azureEnrollments) {
            if (!curriculumIDs.includes(+azureEnrollment)) extraCurriculums.push(+azureEnrollment);
        }
        return extraCurriculums;
    }

    async syncNotRecievedAzureSchoolEnrollments() {
        await this.syncNotRecievedAzureSchoolEnrollmentsForTeachers();
        await this.syncNotRecievedAzureSchoolEnrollmentsForStudents();
    }

    async syncNotRecievedAzureClassEnrollments() {
        await this.syncNotRecievedAzureClassEnrollmentsForTeachers();
        await this.syncNotRecievedAzureClassEnrollmentsForStudents();
    }

    private async syncNotRecievedAzureSchoolEnrollmentsForTeachers() {
        const missingEnrollments = await this.educationalStateService.getMissingEducationalStatesInAzureTempTeacher();
        for (const enrollment of missingEnrollments) {
            const { personID, institutionID } = enrollment;
            await this.connection.transaction(async (manager) => {
                const result = await this.azureEnrollmentService.createAzureEnrollmentUserToSchool(
                    {
                        personID,
                        institutionID,
                        userRole: UserRoleType.TEACHER,
                    } as EnrollmentStudentToSchoolCreateRequestDTO,
                    manager,
                );
            });
        }
    }

    private async syncNotRecievedAzureSchoolEnrollmentsForStudents() {
        const missingEnrollments = await this.educationalStateService.getMissingEducationalStatesInAzureTempStudent();
        for (const enrollment of missingEnrollments) {
            const { personID, institutionID } = enrollment;
            await this.connection.transaction(async (manager) => {
                const result = await this.azureEnrollmentService.createAzureEnrollmentUserToSchool(
                    {
                        personID,
                        institutionID,
                        userRole: UserRoleType.STUDENT,
                    } as EnrollmentStudentToSchoolCreateRequestDTO,
                    manager,
                );
            });
        }
    }

    private async syncNotRecievedAzureClassEnrollmentsForTeachers() {
        const missingEnrollments = await this.classService.getTeachersNotEnrolledForCurriculums();
        for (const enrollment of missingEnrollments) {
            const { personID, curriculumID } = enrollment;
            await this.connection.transaction(async (manager) => {
                const result = await this.azureEnrollmentService.createAzureEnrollmentUserToClass(
                    {
                        personID,
                        curriculumID,
                        userRole: UserRoleType.TEACHER,
                    } as EnrollmentUserToClassCreateRequestDTO,
                    manager,
                );
            });
        }
    }

    private async syncNotRecievedAzureClassEnrollmentsForStudents() {
        const missingEnrollments = await this.classService.getStudentsNotEnrolledForCurriculums();
        for (const enrollment of missingEnrollments) {
            const { personID, curriculumID } = enrollment;
            await this.connection.transaction(async (manager) => {
                const result = await this.azureEnrollmentService.createAzureEnrollmentUserToClass(
                    {
                        personID,
                        curriculumID,
                        userRole: UserRoleType.STUDENT,
                    } as EnrollmentUserToClassCreateRequestDTO,
                    manager,
                );
            });
        }
    }
}

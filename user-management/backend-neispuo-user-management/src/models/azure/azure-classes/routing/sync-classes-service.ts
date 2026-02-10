import { Inject, Injectable, Logger, forwardRef } from '@nestjs/common';
import { UserRoleType } from 'src/common/constants/enum/role-type-enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { EnrollmentTeacherToClassCreateRequestDTO } from 'src/common/dto/requests/enrollment-teacher-to-class-create-request.dto';
import { ClassService } from 'src/models/class/routing/class.service';
import { StudentService } from 'src/models/student/routing/student.service';
import { TeacherService } from 'src/models/teacher/routing/teacher.service';
import { Connection } from 'typeorm';
import { AzureTeacherService } from '../../azure-teacher/routing/azure-teacher.service';
import { AzureClassesService } from './azure-classes.service';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['syncUnsyncedClasses'],
})
export class SyncClassesService {
    constructor(
        private classesService: ClassService,
        private azureClassesService: AzureClassesService,
        @Inject(forwardRef(() => AzureTeacherService))
        private azureTeacherService: AzureTeacherService,
        private studentService: StudentService,
        private teacherService: TeacherService,
        private connection: Connection,
    ) {}

    getUnsyncedClasses() {
        return this.classesService.getUnsyncedClasses();
    }

    async syncUnsyncedClasses() {
        const classes = await this.getUnsyncedClasses();
        for (const classs of classes) {
            const { curriculumID } = classs;
            try {
                const students = await this.studentService.getStudentPersonIDsByCurriculumID(curriculumID);
                const teachers = await this.teacherService.getTeacherPersonIDsByCurriculumID(curriculumID);
                const studentsPersonIDs = students.map((student) => student.personID);
                const teachersPersonIDs = teachers.map((teacher) => teacher.personID);
                const dto = {
                    curriculumID,
                    institutionID: +students[0]?.institutionID,
                    personIDs: studentsPersonIDs,
                };

                await this.azureClassesService.createAzureClassesAndEnrollUsers(dto);
                for (const personID of teachersPersonIDs) {
                    const dto: EnrollmentTeacherToClassCreateRequestDTO = {
                        curriculumIDs: [curriculumID],
                        personID,
                        userRole: UserRoleType.TEACHER,
                    };
                    await this.azureTeacherService.createAzureEnrollmentTeacherToClass(dto);
                }
            } catch (e) {
                Logger.error(`syncUnsyncedClasses FAILED FOR curriculumID: ${curriculumID}`);
            }
        }
    }
}

import { Inject, Injectable, Logger, forwardRef } from '@nestjs/common';
import { AzureUserResponseFactory } from 'src/common/factories/azure-user-response-dto.factory';
import { AzureStudentService } from 'src/models/azure/azure-student/routing/azure-student.service';
import { EntityManager } from 'typeorm';
import { StudentRepository } from '../student.repository';

@Injectable()
export class StudentService {
    private logger = new Logger(StudentService.name);

    private readonly SELECT_TOP_LIMIT: number = 1000;

    constructor(
        private studentRepository: StudentRepository,
        @Inject(forwardRef(() => AzureStudentService))
        private azureStudentService: AzureStudentService,
    ) {}

    async getStudentByPersonID(personID: number) {
        const result = await this.studentRepository.getStudentByPersonID(personID);
        return result;
    }

    isStudentGraduate(personID): Promise<boolean> {
        return this.studentRepository.isStudentGraduate(personID);
    }

    async getGraduatedStudents() {
        const result = await this.studentRepository.getGraduatedStudents(this.SELECT_TOP_LIMIT);
        return result;
    }

    async deleteGraduatedStudents() {
        const students = await this.getGraduatedStudents();
        for (const student of students) {
            const personID = student?.personID;
            if (!personID) continue;
            const azureStudentDTO = AzureUserResponseFactory.createFromStudentResponseDTO(student);
            azureStudentDTO.isForArchivation = azureStudentDTO?.azureID ? 0 : 1;
            try {
                await this.azureStudentService.deleteAzureStudent(azureStudentDTO, null);
            } catch (error) {
                this.logger.error(`Graduated student with personID: ${personID} could not be deleted`);
            }
        }
    }

    async getStudentPersonIDsByCurriculumID(curriculumID: number) {
        return this.studentRepository.getStudentPersonIDsByCurriculumID(curriculumID);
    }

    async getStudentGradeByPersonID(personID: number, entityManager?: EntityManager) {
        return this.studentRepository.getStudentGradeByPersonID(personID, entityManager);
    }
}

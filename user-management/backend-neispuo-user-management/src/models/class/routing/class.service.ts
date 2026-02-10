import { Injectable } from '@nestjs/common';
import { AzureClassesResponseDTO } from 'src/common/dto/responses/azure-classes-response.dto';
import { AzureEnrollmentsResponseDTO } from 'src/common/dto/responses/azure-enrollments-response.dto';
import { ClassesResponseDTO } from 'src/common/dto/responses/classes-response.dto';
import { CurriculumNotFoundException } from 'src/common/exceptions/curriculum-not-found.exception';
import { ClassRepository } from '../class.repository';

@Injectable()
export class ClassService {
    constructor(private classRepository: ClassRepository) {}

    async getCurriculumsByCurriculumID(curriculumID: number) {
        const result = await this.classRepository.getCurriculumsByCurriculumID(curriculumID);
        if (!result || result.length === 0) throw new CurriculumNotFoundException();
        return result;
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

    async syncCreateAzureClass(azureClass: AzureClassesResponseDTO) {
        return this.classRepository.createAzureClassProcedure(azureClass);
    }

    async syncUpdateAzureClass(azureClass: AzureClassesResponseDTO) {
        return this.classRepository.updateAzureClassProcedure(azureClass);
    }

    async syncDeleteAzureClass(azureClass: AzureClassesResponseDTO) {
        return this.classRepository.deleteAzureClassProcedure(azureClass);
    }

    async getCurriculumsTeacherByPersonID(personID: number) {
        return this.classRepository.getCurriculumsTeacherByPersonID(personID);
    }

    async getCurriculumsStudentByPersonID(personID: number) {
        return this.classRepository.getCurriculumsStudentByPersonID(personID);
    }

    async getCurriculumByCurriculumID(curriculumID: number) {
        return this.classRepository.getCurriculumByCurriculumID(curriculumID);
    }

    async getUnsyncedClasses() {
        return this.classRepository.getUnsyncedClasses();
    }

    async setIsAzureEnrolledForTeacher(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        return this.classRepository.setIsAzureEnrolledForTeacher(dtos);
    }

    async setIsAzureEnrolledForStudent(dtos: AzureEnrollmentsResponseDTO[]) {
        if (!dtos?.length) return;
        return this.classRepository.setIsAzureEnrolledForStudent(dtos);
    }

    async getStudentsNotEnrolledForCurriculums() {
        return this.classRepository.getStudentsNotEnrolledForCurriculums();
    }

    async getTeachersNotEnrolledForCurriculums() {
        return this.classRepository.getTeachersNotEnrolledForCurriculums();
    }
}

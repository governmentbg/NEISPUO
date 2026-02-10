import { Injectable } from '@nestjs/common';
import { EducationalStateDTO } from 'src/common/dto/responses/educational-state.dto';
import { EducationalStateRepository } from '../educational-state.repository';

@Injectable()
export class EducationalStateService {
    constructor(private educationalStateRepository: EducationalStateRepository) {}

    async getUserEducationalStatesByPersonID(educationalStateDTO: EducationalStateDTO) {
        return this.educationalStateRepository.getUserEducationalStatesByPersonID(educationalStateDTO);
    }

    async getTeacherEducationalStatesByPersonID(educationalStateDTO: EducationalStateDTO) {
        return this.educationalStateRepository.getTeacherEducationalStatesByPersonID(educationalStateDTO);
    }

    async getUserEducationalStatesByInstitutionID(educationalStateDTO: EducationalStateDTO) {
        return this.educationalStateRepository.getUserEducationalStatesByInstituionID(educationalStateDTO);
    }

    async isTeacher(educationalStateDTO: EducationalStateDTO) {
        const result = await this.educationalStateRepository.getTeacherEducationalStatesByPersonID(educationalStateDTO);
        return result && result?.length > 0 ? true : false;
    }

    async getMissingEducationalStatesInAzureTempTeacher() {
        return this.educationalStateRepository.getMissingEducationalStatesForTeacherInAzureTemp();
    }

    async getMissingEducationalStatesInAzureTempStudent() {
        return this.educationalStateRepository.getMissingEducationalStatesForStudentInAzureTemp();
    }
}

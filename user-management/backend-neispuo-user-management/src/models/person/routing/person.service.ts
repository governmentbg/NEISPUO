import { Injectable } from '@nestjs/common';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { PersonEntity } from 'src/common/entities/person.entity';
import { EntityManager } from 'typeorm';
import { PersonRepository } from '../person.repository';

@Injectable()
export class PersonService {
    constructor(private readonly personRepo: PersonRepository) {}

    async createPerson(createPersonDTO: Partial<PersonEntity>, entityManager: EntityManager) {
        return this.personRepo.createPerson(createPersonDTO, entityManager);
    }

    async getPersonByPersonID(personID: number) {
        return this.personRepo.getPersonByPersonID(personID);
    }

    async getPersonByPersonalID(personalID: string) {
        return this.personRepo.getPersonByPersonalID(personalID);
    }

    async getPersonByInstitutionID(institutionID: number) {
        return this.personRepo.getPersonByInstitutionID(institutionID);
    }

    async getPersonByPersonalIDAndSchoolCode(personalID: string, code: string) {
        return this.personRepo.getPersonByPersonalIDAndChildSchoolCode(personalID, code);
    }

    async getPersonChildren(personID: number) {
        return this.personRepo.getPersonChildren(personID);
    }

    async getPersonByUsername(username: string) {
        return this.personRepo.getPersonByUserName(username);
    }

    async updatePublicEduNumberByPersonID(dto: PersonResponseDTO) {
        return this.personRepo.updatePublicEduNumberByPersonID(dto);
    }

    async updateAzureIDByPersonID(dto: PersonResponseDTO) {
        return this.personRepo.updateAzureIDByPersonID(dto);
    }

    async updateAzureIDByPublicEduNumber(dto: PersonResponseDTO) {
        return this.personRepo.updateAzureIDByPublicEduNumber(dto);
    }
}

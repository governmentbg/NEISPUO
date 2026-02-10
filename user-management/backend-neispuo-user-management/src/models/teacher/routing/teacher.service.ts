import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { InstitutionRepository } from 'src/models/institution/institution.repository';
import { TeacherRepository } from '../teacher.repository';

@Injectable()
export class TeacherService {
    constructor(
        @InjectRepository(TeacherRepository) private teacherRepository: TeacherRepository,
        @InjectRepository(InstitutionRepository) private institutionRepository: InstitutionRepository,
    ) {}

    async getTeacherByUserID(userID: number) {
        const result = await this.teacherRepository.getTeacherByUserID(userID);
        return result;
    }

    async getTeacherByPersonID(personID: number) {
        const result = await this.teacherRepository.getTeacherByPersonID(personID);
        return result;
    }

    async getAccountantInstitutions(sysUserID: number) {
        const result = await this.institutionRepository.getAccountantInstitutionsByUserID(sysUserID);
        return result.map((r) => r.institutionID);
    }

    async hasAccountantRole(personID: number) {
        const result = await this.institutionRepository.getAccountantSysUserIDByPersonID(personID);
        return result;
    }

    async hasVURole(personID: number) {
        const result = await this.institutionRepository.getVUSysUserIDByPersonID(personID);
        return result;
    }

    async getTeacherPersonIDsByCurriculumID(curriculumID: number) {
        return this.teacherRepository.getTeacherPersonIDsByCurriculumID(curriculumID);
    }
}

import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InstitutionsTableEntity } from 'src/common/entities/institutions-table-view.entity';
import { InstitutionRepository } from '../institution.repository';

@Injectable()
export class InstitutionService extends TypeOrmCrudService<InstitutionsTableEntity> {
    constructor(@InjectRepository(InstitutionsTableEntity) repo, private instRepo: InstitutionRepository) {
        super(repo);
    }

    async getInstitutionByInstitutionID(institutionID: number) {
        const result = await this.instRepo.getInstitutionByInstitutionID(institutionID);
        return result;
    }

    async getDeletedInstitutionByInstitutionID(institutionID: number) {
        const result = await this.instRepo.getDeletedInstitutionByInstitutionID(institutionID);
        return result;
    }

    async getInstitutionByUserName(username: string) {
        const result = await this.instRepo.getInstitutionByUserName(username);
        return result;
    }

    getInstitutionWithoutSyncedTeachers(validFrom: Date) {
        return this.instRepo.getInstitutionWithoutSyncedTeachers(validFrom);
    }

    getInstitutionWithoutSyncedStudents(validFrom: Date) {
        return this.instRepo.getInstitutionWithoutSyncedStudents(validFrom);
    }

    getUnsyncedInstitutions() {
        return this.instRepo.getUnsyncedInstitutions();
    }
}

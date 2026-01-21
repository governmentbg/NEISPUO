import { Injectable } from '@nestjs/common';
import { FixAccountantRolesDTO } from './dto/fix-accountant-roles.dto';
import { FixAccountantRolesRepository } from './fix-accountant-roles.repository';

@Injectable()
export class FixAccountantRolesService {
    constructor(private readonly fixAccountantRolesRepository: FixAccountantRolesRepository) {}

    async isTableNameFree(providedTableName: string) {
        await this.fixAccountantRolesRepository.isTableNameFree(providedTableName);
    }

    async createTempTable(schemaName: string, tempTableName: string) {
        await this.fixAccountantRolesRepository.createTempTable(schemaName, tempTableName);
    }

    async getAccountantsForUpdate(schemaName: string) {
        return this.fixAccountantRolesRepository.getAccountantsForUpdate(schemaName);
    }

    async bulkInsertAccountantsForUpdate(
        accountants: FixAccountantRolesDTO[],
        schemaName: string,
        tempTableName: string,
    ) {
        await this.fixAccountantRolesRepository.bulkInsertAccountantsForUpdate(accountants, schemaName, tempTableName);
    }
}

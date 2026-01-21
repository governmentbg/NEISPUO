import { Command, CommandRunner } from 'nest-commander';
import { FixAccountantRolesService } from './fix-accountant-roles.service';

@Command({
    name: 'fix-accountant-roles',
    arguments: '<tableName>',
    options: { isDefault: true },
})
export class FixAccountantRolesTaskRunner implements CommandRunner {
    constructor(private readonly fixAccountantRolesService: FixAccountantRolesService) {}

    // Pass the already existing schema name as firs and the unique table name as second command argument.
    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        const schemaName = inputs[0];
        const tempTableName = inputs[1];
        await this.fixAccountantRolesService.isTableNameFree(tempTableName);
        await this.fixAccountantRolesService.createTempTable(schemaName, tempTableName);
        const accountants = await this.fixAccountantRolesService.getAccountantsForUpdate(schemaName);
        await this.fixAccountantRolesService.bulkInsertAccountantsForUpdate(accountants, schemaName, tempTableName);
    }
}

import { Command, CommandRunner } from 'nest-commander';
import { InsertAzureCreateService } from './insert-azure-create.service';

@Command({
    name: 'insert-azure-create',
    arguments: '',
    options: { isDefault: true },
})
export class InsertAzureCreateTaskRunner implements CommandRunner {
    constructor(private readonly insertAzureCreateService: InsertAzureCreateService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        // this script is outdated and no longer nessesary.
        //i will comment this in order to throw and error and enforce reading this comment if command is ran.
        // await this.insertAzureCreateService.syncAllPersonsWithNoCreateWorkflow();
    }
}

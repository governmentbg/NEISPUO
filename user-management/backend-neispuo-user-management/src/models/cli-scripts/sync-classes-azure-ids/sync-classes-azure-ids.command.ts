import { Command, CommandRunner } from 'nest-commander';
import { SyncClassesAzureIDsService } from './sync-classes-azure-ids.service';

@Command({
    name: 'sync-classes-azure-ids',
    options: { isDefault: true },
})
export class SyncClassesAzureIDsTaskRunner implements CommandRunner {
    constructor(private readonly syncClassesAzureIDsService: SyncClassesAzureIDsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncClassesAzureIDsService.updateCurriculumsFromAzure();
    }
}

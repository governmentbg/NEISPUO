import { Command, CommandRunner } from 'nest-commander';
import { SyncExistingPersonsWithAzureIDService } from './sync-existing-persons-with-azureid.service';

@Command({
    name: 'sync-existing-persons-with-azureid',
    options: { isDefault: true },
})
export class SyncExistingPersonsWithAzureIDTaskRunner implements CommandRunner {
    constructor(private readonly syncExistingPersonsWithAzureIDService: SyncExistingPersonsWithAzureIDService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncExistingPersonsWithAzureIDService.syncSysUsers();
    }
}

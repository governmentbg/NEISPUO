import { Command, CommandRunner } from 'nest-commander';
import { SyncAzureIDsService } from './sync-azure-ids.service';

@Command({
    name: 'sync-azure-ids',
    arguments: '',
    options: { isDefault: true },
})
export class SyncAzureIDsTaskRunner implements CommandRunner {
    constructor(private readonly syncAzureIDsService: SyncAzureIDsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncAzureIDsService.syncAzureIDsInNEISPUO();
    }
}

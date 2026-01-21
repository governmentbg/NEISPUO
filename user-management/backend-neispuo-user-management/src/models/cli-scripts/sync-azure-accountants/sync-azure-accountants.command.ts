import { Command, CommandRunner } from 'nest-commander';
import { SyncAzureAccountantsService } from './sync-azure-accountants.service';

@Command({
    name: 'sync-azure-accountants',
    arguments: '',
    options: { isDefault: true },
})
export class SyncAzureAccountantsTaskRunner implements CommandRunner {
    constructor(private readonly syncAzureAccountantsService: SyncAzureAccountantsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncAzureAccountantsService.syncAccountantsWithAzure();
    }
}

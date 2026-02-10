import { Command, CommandRunner } from 'nest-commander';
import { SyncUsersFromAzureService } from './sync-users-from-azure.service';

@Command({
    name: 'sync-users-from-azure',
    arguments: '',
    options: { isDefault: true },
})
export class SyncOldUsersTaskRunner implements CommandRunner {
    constructor(private readonly syncUsersFromAzureService: SyncUsersFromAzureService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncUsersFromAzureService.syncUsersFromAzure();
    }
}

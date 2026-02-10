import { Command, CommandRunner } from 'nest-commander';
import { SyncAzureIDsForExternalUsersService } from './sync-azure-ids-for-external-users.service';

@Command({
    name: 'sync-azure-ids-for-external-users',
    arguments: '',
    options: { isDefault: true },
})
export class SyncAzureIDsForExternalUsersTaskRunner implements CommandRunner {
    constructor(private readonly syncAzureIDsForExternalUsersService: SyncAzureIDsForExternalUsersService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncAzureIDsForExternalUsersService.syncExternalUsersAzureIDsInNEISPUO();
    }
}

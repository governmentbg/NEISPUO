import { Command, CommandRunner } from 'nest-commander';
import { SyncAzureOrganizationsService } from './sync-azure-organizations.service';

@Command({
    name: 'sync-azure-organizations',
    arguments: '',
    options: { isDefault: true },
})
export class SyncAzureOrganizationsTaskRunner implements CommandRunner {
    constructor(private readonly syncAzureOrganizationsService: SyncAzureOrganizationsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncAzureOrganizationsService.syncExistingNEISPUOOrganizationsWithAzureOrganizations();
    }
}

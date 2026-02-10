import { Command, CommandRunner } from 'nest-commander';
import { SyncInstitutionsAzureIDsService } from './sync-institutions-azure-ids.service';

@Command({
    name: 'sync-institutions-azure-ids',
    options: { isDefault: true },
})
export class SyncInstitutionsAzureIDsTaskRunner implements CommandRunner {
    constructor(private readonly syncInstitutionsAzureIDsService: SyncInstitutionsAzureIDsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncInstitutionsAzureIDsService.syncInstitutionAzureIDs();
    }
}

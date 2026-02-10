import { Command, CommandRunner } from 'nest-commander';
import { SyncAzureEnrollmentsService } from './sync-azure-enrollments.service';

@Command({
    name: 'sync-azure-enrollments',
    arguments: '',
    options: { isDefault: true },
})
export class SyncAzureEnrollmentsTaskRunner implements CommandRunner {
    constructor(private readonly syncAzureEnrollmentsService: SyncAzureEnrollmentsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncAzureEnrollmentsService.syncEnrollments();
    }
}

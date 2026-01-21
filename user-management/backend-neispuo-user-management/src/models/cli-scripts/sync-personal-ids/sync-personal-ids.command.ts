import { Command, CommandRunner } from 'nest-commander';
import { SyncPersonalIDsService } from './sync-personal-ids.service';

@Command({
    name: 'sync-personal-ids',
    options: { isDefault: true },
})
export class SyncPersonalIDsTaskRunner implements CommandRunner {
    constructor(private readonly syncPersonalIDsService: SyncPersonalIDsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.syncPersonalIDsService.updatePersonalIDsInAzure();
    }
}

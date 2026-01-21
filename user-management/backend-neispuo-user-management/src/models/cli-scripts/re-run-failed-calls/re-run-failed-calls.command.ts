import { Command, CommandRunner } from 'nest-commander';
import { ReRunFailedCallsService } from './re-run-failed-calls.service';

@Command({
    name: 're-run-failed-calls',
    arguments: '',
    options: { isDefault: true },
})
export class ReRunFailedCallsTaskRunner implements CommandRunner {
    constructor(private readonly reRunFailedCallsService: ReRunFailedCallsService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        await this.reRunFailedCallsService.reRunAllFailedCalls();
    }
}

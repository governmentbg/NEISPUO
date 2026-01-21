import { Command, CommandRunner, InquirerService, Option } from 'nest-commander';
import { SyncCurriculumsWithoutAzureIDService } from './sync-curriculums-without-azureid.service';

@Command({
    name: 'sync-curriculums-without-azureid',
    options: { isDefault: true },
})

// to run script run command in npm node terminal npm run sync-curriculums-without-azureid -- --from 10 --to 50
//10 and 50 are examples of from to curriculumID params.
export class SyncCurriculumsWithoutAzureIDTaskRunner implements CommandRunner {
    constructor(
        private readonly scwaService: SyncCurriculumsWithoutAzureIDService,
        private readonly inquirerService: InquirerService,
    ) {}

    async run(inputs: string[], options: SyncCurriculumOptions): Promise<void> {
        console.table(options);
        // const resp = await this.scwaService.syncCurriculumsWithoutAzureIDs(options);
        const resp = await this.scwaService.syncEnrollmentsForCurriculumsWithoutAzureIDs(options);
        console.log(resp);
        return;
    }

    @Option({
        flags: '-from, --from <from>',
        description: 'The CurriculumID you wish to start from',
    })
    parseFrom(val: number) {
        return +val;
    }

    @Option({
        flags: '-to, --to <to>',
        description: 'The CurriculumID you wish to end to',
    })
    parseTo(val: number) {
        return +val;
    }
}

export interface SyncCurriculumOptions {
    from: number;
    to: number;
}

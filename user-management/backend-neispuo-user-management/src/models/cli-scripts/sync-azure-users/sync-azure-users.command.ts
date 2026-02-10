import console from 'console';
import moment from 'moment';
import { Command, CommandRunner, InquirerService, Option } from 'nest-commander';
import { SyncAzureUsersService } from './sync-azure-users.service';

export enum SyncUserPosition {
    STUDENT = 'student',
    TEACHER = 'teacher',
    ALL = 'all',
}

export interface SyncUserOptions {
    school: number;
    position: SyncUserPosition;
    date: string;
    sync: boolean;
}
@Command({
    name: 'sync-azure-enrollments',
    options: { isDefault: true },
})
export class SyncAzureUsersTaskRunner implements CommandRunner {
    constructor(
        private readonly syncAzureUsersService: SyncAzureUsersService,
        private readonly inquirerService: InquirerService,
    ) {}

    async run(inputs: string[], options: SyncUserOptions): Promise<void> {
        console.table(options);
        options = await this.inquirerService.ask('sync-users', options);
        await this.syncAzureUsersService.syncUsers(options);
    }

    @Option({
        flags: '-s, --school <school>',
        description: 'The institutionID if you wish to sync users for a specific school',
    })
    parseSchool(val: number) {
        return +val;
    }

    @Option({
        flags: '-p, --position <position>',
        description: 'The type of user (student/teacher/all) you wish to run the script for. ',
    })
    parsePosition(val: string) {
        if (!val) return SyncUserPosition.ALL;
        return val;
    }

    @Option({
        flags: '-d, --date <date>',
        description: 'A date from which to look for records.',
    })
    parseFromDate(val: string) {
        if (!moment(val, 'YYYY-MM-DD', true).isValid()) return '2012-12-12';
        return val;
    }

    @Option({
        flags: '-sync, --sync <sync>',
        description: 'A different shell to spawn than the default',
    })
    parseSync(val: string): boolean {
        return val.toLowerCase() === 'true';
    }
}

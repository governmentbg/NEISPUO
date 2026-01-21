import { Command, CommandRunner } from 'nest-commander';
import { DeactivateOldStudentUsernamesService } from './deactivate-old-student-usernames.service';

@Command({
    name: 'deactivate-old-student-usernames',
    arguments: '',
    options: { isDefault: true },
})
export class DeactivateOldStudentUsernamesTaskRunner implements CommandRunner {
    constructor(private readonly deactivateOldStudentUsernamesService: DeactivateOldStudentUsernamesService) {}

    async run(inputs: string[], options: Record<string, any>) {
        await this.deactivateOldStudentUsernamesService.run();
    }
}

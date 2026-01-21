import { Command, CommandRunner } from 'nest-commander';
import { FixShortStudentUsernameService } from './fix-short-student-username.service';

@Command({
    name: 'fix-short-student-username',
    arguments: '',
    options: { isDefault: true },
})
export class FixShortStudentUsernameTaskRunner implements CommandRunner {
    constructor(private readonly fixShortStudentUsernameService: FixShortStudentUsernameService) {}

    async run(inputs: string[], options: Record<string, any>) {
        const users = await this.fixShortStudentUsernameService.getAllStudents();
    }
}

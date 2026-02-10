import { Command, CommandRunner } from 'nest-commander';
import { GenerateStudentCodesService } from './generate-student-codes.service';

@Command({
    name: 'generate-student-codes',
    arguments: '',
    options: { isDefault: true },
})
export class GenerateStudentCodesTaskRunner implements CommandRunner {
    constructor(private readonly generateStudentCodesService: GenerateStudentCodesService) {}

    async run(inputs: string[], options: Record<string, any>): Promise<void> {
        ("this script shouldn't be used anymore. Exiting...");
        process.exit(0);
    }
}

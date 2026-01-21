import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { GenerateStudentCodesModule } from './generate-student-codes.module';

const bootstrap = async () => {
    await CommandFactory.run(GenerateStudentCodesModule, new Logger());
};

bootstrap();

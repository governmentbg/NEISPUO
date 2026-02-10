import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { FixShortStudentUsernameModule } from './fix-short-student-username.module';

const bootstrap = async () => {
    await CommandFactory.run(FixShortStudentUsernameModule, new Logger());
};

bootstrap();

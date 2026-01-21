import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { DeactivateOldStudentUsernamesModule } from './deactivate-old-student-usernames.module';

const bootstrap = async () => {
    await CommandFactory.run(DeactivateOldStudentUsernamesModule, new Logger());
};

bootstrap();

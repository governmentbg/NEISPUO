import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { ReRunFailedCallsModule } from './re-run-failed-calls.module';

const bootstrap = async () => {
    await CommandFactory.run(ReRunFailedCallsModule, new Logger());
};

bootstrap();

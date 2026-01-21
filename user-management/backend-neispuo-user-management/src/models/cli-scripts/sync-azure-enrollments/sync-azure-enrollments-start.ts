import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncAzureEnrollmentsModule } from './sync-azure-enrollments.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncAzureEnrollmentsModule, new Logger());
};

bootstrap();

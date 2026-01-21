import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncAzureAccountantsModule } from './sync-azure-accountants.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncAzureAccountantsModule, new Logger());
};

bootstrap();

import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncAzureIDsModule } from './sync-azure-ids.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncAzureIDsModule, new Logger());
};

bootstrap();

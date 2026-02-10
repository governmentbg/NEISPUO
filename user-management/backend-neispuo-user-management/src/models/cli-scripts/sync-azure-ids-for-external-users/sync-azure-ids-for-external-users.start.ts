import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncAzureIDsForExternalUsersModule } from './sync-azure-ids-for-external-users.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncAzureIDsForExternalUsersModule, new Logger());
};

bootstrap();

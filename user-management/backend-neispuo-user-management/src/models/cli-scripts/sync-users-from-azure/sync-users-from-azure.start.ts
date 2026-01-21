import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncOldUsersModule } from './sync-users-from-azure.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncOldUsersModule, new Logger());
};

bootstrap();

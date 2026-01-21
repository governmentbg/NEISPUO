import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncAzureOrganizationsModule } from './sync-azure-organizations.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncAzureOrganizationsModule, new Logger());
};

bootstrap();

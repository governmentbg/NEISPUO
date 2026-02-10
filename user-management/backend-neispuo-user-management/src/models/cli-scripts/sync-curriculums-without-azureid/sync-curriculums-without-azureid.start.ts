import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncCurriculumsWithoutAzureIDModule } from './sync-curriculums-without-azureid.module';

const bootstrap = async () => {
    await CommandFactory.run(SyncCurriculumsWithoutAzureIDModule, new Logger());
};

bootstrap();

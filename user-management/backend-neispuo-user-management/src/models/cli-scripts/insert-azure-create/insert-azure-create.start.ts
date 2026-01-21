import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { InsertAzureCreateModule } from './insert-azure-create.module';

const bootstrap = async () => {
    await CommandFactory.run(InsertAzureCreateModule, new Logger());
};

bootstrap();

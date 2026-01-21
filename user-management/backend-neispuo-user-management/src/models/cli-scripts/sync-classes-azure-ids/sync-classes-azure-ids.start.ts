import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncClassesAzureIDsModule } from './sync-classes-azure-ids.module';

//EXAMPLE OF A START SCRIPT npm run sync-classes-azure-ids

const bootstrap = async () => {
    await CommandFactory.run(SyncClassesAzureIDsModule, {
        errorHandler: (err) => {
            console.log(err.message);
            process.exit(0);
        },
        logger: new Logger(),
    });
};

bootstrap();

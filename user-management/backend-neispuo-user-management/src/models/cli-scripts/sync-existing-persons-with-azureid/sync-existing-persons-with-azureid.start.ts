import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { SyncExistingPersonsWithAzureIDModule } from './sync-existing-persons-with-azureid.module';

//EXAMPLE OF A START SCRIPT npm run sync-users -- -p 'alddddl' -sync true -s 123 -d '2012-12-11'

const bootstrap = async () => {
    await CommandFactory.run(SyncExistingPersonsWithAzureIDModule, {
        errorHandler: (err) => {
            console.log(err.message);
            process.exit(0);
        },
        logger: new Logger(),
    });
};

bootstrap();

import { Logger } from '@nestjs/common';
import { CommandFactory } from 'nest-commander';
import { FixAccountantRolesModule } from './fix-accountant-roles.module';

const bootstrap = async () => {
    const logger = new Logger('FixAccountantRoles');
    try {
        await CommandFactory.run(FixAccountantRolesModule, logger);
    } catch (error) {
        logger.error(error.message);
        process.exit(1);
    }
};

bootstrap();

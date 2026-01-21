import { Module } from '@nestjs/common';
import { DatabaseConfigModule } from 'src/config/database/database.configuration.module';
import { FixAccountantRolesTaskRunner } from './fix-accountant-roles.command';
import { FixAccountantRolesRepository } from './fix-accountant-roles.repository';
import { FixAccountantRolesService } from './fix-accountant-roles.service';

@Module({
    imports: [DatabaseConfigModule],
    providers: [FixAccountantRolesService, FixAccountantRolesRepository, FixAccountantRolesTaskRunner],
})
export class FixAccountantRolesModule {}

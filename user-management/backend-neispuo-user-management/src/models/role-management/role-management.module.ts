import { Module } from '@nestjs/common';
import { RoleManagementController } from './routing/role-management.controller';
import { RoleManagementRepository } from './role-management.repository';
import { RoleManagementService } from './routing/role-management.service';
import { SIEMLoggerModule } from '../siem-logger/siem-logger.module';

@Module({
    imports: [SIEMLoggerModule.forFeature()],
    controllers: [RoleManagementController],
    providers: [RoleManagementService, RoleManagementRepository],
    exports: [RoleManagementService],
})
export class RoleManagementModule {}

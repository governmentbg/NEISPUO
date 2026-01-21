import { Module } from '@nestjs/common';
import { ParentAccessRepository } from './parent-access.repository';
import { ParentAccessController } from './routing/parent-access.controller';
import { ParentAccessService } from './routing/parent-access.service';

@Module({
    controllers: [ParentAccessController],
    providers: [ParentAccessService, ParentAccessRepository],
    exports: [ParentAccessService],
})
export class ParentAccessModule {}

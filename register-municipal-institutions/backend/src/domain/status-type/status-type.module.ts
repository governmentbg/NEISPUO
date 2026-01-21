import { Module } from '@nestjs/common';
import { StatusTypeService } from '../status-type/routes/status-type/status-type.service';
import { StatusTypeController } from '../status-type/routes/status-type/status-type.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { StatusType } from './status-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([StatusType])],
    exports: [TypeOrmModule],

    controllers: [StatusTypeController],
    providers: [StatusTypeService]
})
export class StatusTypeModule {}

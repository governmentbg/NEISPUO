import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ProcedureTypeService } from '../procedure-type/routes/procedure-type/procedure-type.service';
import { ProcedureTypeController } from '../procedure-type/routes/procedure-type/procedure-type.controller';
import { ProcedureType } from './procedure-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([ProcedureType])],
    exports: [TypeOrmModule],

    controllers: [ProcedureTypeController],
    providers: [ProcedureTypeService],
})
export class ProcedureTypeModule {}

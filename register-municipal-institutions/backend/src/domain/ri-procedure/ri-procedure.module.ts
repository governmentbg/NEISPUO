import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RIProcedureService } from './routes/ri-procedure/ri-procedure.service';
import { RIProcedureController } from './routes/ri-procedure/ri-procedure.controller';
import { RIProcedure } from './ri-procedure.entity';

@Module({
    imports: [TypeOrmModule.forFeature([RIProcedure])],
    exports: [TypeOrmModule],

    controllers: [RIProcedureController],
    providers: [RIProcedureService],
})
export class RIProcedureModule {}

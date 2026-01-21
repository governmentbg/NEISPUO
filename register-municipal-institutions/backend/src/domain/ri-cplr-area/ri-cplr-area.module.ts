import { Module } from '@nestjs/common';
import { RICPLRAreaService } from './routes/ri-cplr-area/ri-cplr-area.service';
import { RICPLRAreaController } from './routes/ri-cplr-area/ri-cplr-area.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RICPLRArea } from './ri-cplr-area.entity';
import { InstitutionModule } from '@domain/institution/institution.module';
import { RIProcedureModule } from '@domain/ri-procedure/ri-procedure.module';
import { TownModule } from '@domain/town/town.module';

@Module({
    imports: [
        TypeOrmModule.forFeature([RICPLRArea]),
        InstitutionModule,
        RIProcedureModule,
        TownModule
    ],
    exports: [TypeOrmModule],

    controllers: [RICPLRAreaController],
    providers: [RICPLRAreaService]
})
export class RICPLRAreaModule {}

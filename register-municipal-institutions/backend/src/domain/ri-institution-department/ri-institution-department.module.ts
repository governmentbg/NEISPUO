import { Module } from '@nestjs/common';
import { RIInstitutionDepartmentService } from './routes/ri-institution-department/ri-institution-department.service';
import { RIInstitutionDepartmentController } from './routes/ri-institution-department/ri-institution-department.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RIInstitutionDepartment } from './ri-institution-department.entity';
import { InstitutionModule } from '@domain/institution/institution.module';
import { RIProcedureModule } from '@domain/ri-procedure/ri-procedure.module';
import { TownModule } from '@domain/town/town.module';

@Module({
    imports: [
        TypeOrmModule.forFeature([RIInstitutionDepartment]),
        InstitutionModule,
        RIProcedureModule,
        TownModule
    ],
    exports: [TypeOrmModule],

    controllers: [RIInstitutionDepartmentController],
    providers: [RIInstitutionDepartmentService]
})
export class RIInstitutionDepartmentModule {}

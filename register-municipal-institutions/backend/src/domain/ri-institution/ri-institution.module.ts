import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { InstitutionService } from '@domain/institution/routes/institution/institution.service';
import { InstitutionModule } from '@domain/institution/institution.module';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { RIProcedureModule } from '@domain/ri-procedure/ri-procedure.module';
import { TownModule } from '@domain/town/town.module';
import { TownService } from '@domain/town/routes/town/town.service';
import { UserManagementAPIRequestService } from '@domain/user-management-api-request/user-management-api-request.service';
import { RIInstitution } from './ri-institution.entity';
import { RIInstitutionController } from './routes/ri-institution/ri-institution.controller';
import { RIInstitutionService } from './routes/ri-institution/ri-institution.service';
import { RIInstitutionPublicController } from './routes/ri-institution-public/ri-institution-public.controller';
import { RIInstitutionPublicService } from './routes/ri-institution-public/ri-institution-public.service';
import { RIInstitutionLatestActive } from './ri-institution-latest-active.entity';
import { RIInstitutionLatestActiveController } from './routes/ri-institution-latest-active/ri-institution-latest-active.controller';
import { RIInstitutionLatestActiveService } from './routes/ri-institution-latest-active/ri-institution-latest-active.service';
import { RIInstitutionClosedController } from './routes/ri-institution-closed/ri-institution-closed.controller';
import { RIInstitutionClosedService } from './routes/ri-institution-closed/ri-institution-closed.service';
import { AuditEntity } from './audit/audit.entity';
import { AuditService } from './audit/audit.service';
import { RIInstitutionDetachController } from './routes/ri-institution/detach/ri-institution-detach.controller';
import { RIInstitutionDivideController } from './routes/ri-institution/divide/ri-institution-divide.controller';
import { RIInstitutionMergeController } from './routes/ri-institution/merge/ri-institution-merge.controller';
import { RIInstitutionJoinController } from './routes/ri-institution/join/ri-institution-join.controller';
import { RIInstitutionDetachService } from './routes/ri-institution/detach/ri-institution-detach.service';
import { RIInstitutionDivideService } from './routes/ri-institution/divide/ri-institution-divide.service';
import { RIInstitutionJoinService } from './routes/ri-institution/join/ri-institution-join.service';
import { RIInstitutionMergeService } from './routes/ri-institution/merge/ri-institution-merge.service';

@Module({
    imports: [
        TypeOrmModule.forFeature([RIInstitution, RIInstitutionLatestActive, AuditEntity]),
        InstitutionModule,
        RIProcedureModule,
        TownModule,
    ],
    exports: [TypeOrmModule],

    controllers: [
        RIInstitutionController,
        RIInstitutionPublicController,
        RIInstitutionLatestActiveController,
        RIInstitutionClosedController,
        RIInstitutionDetachController,
        RIInstitutionDivideController,
        RIInstitutionMergeController,
        RIInstitutionJoinController,
    ],
    providers: [
        RIInstitutionService,
        AuditService,
        InstitutionService,
        RIProcedureService,
        TownService,
        RIInstitutionPublicService,
        RIInstitutionLatestActiveService,
        RIInstitutionClosedService,
        RIInstitutionDetachService,
        RIInstitutionDivideService,
        RIInstitutionJoinService,
        RIInstitutionMergeService,
        UserManagementAPIRequestService,
    ],
})
export class RIInstitutionModule {}

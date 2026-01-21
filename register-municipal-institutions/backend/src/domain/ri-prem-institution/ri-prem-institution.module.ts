import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RIPremInstitutionService } from './routes/ri-prem-institution/ri-prem-institution.service';
import { RIPremInstitutionController } from './routes/ri-prem-institution/ri-prem-institution.controller';
import { RIPremInstitution } from './ri-prem-institution.entity';

@Module({
    imports: [TypeOrmModule.forFeature([RIPremInstitution])],
    exports: [TypeOrmModule],

    controllers: [RIPremInstitutionController],
    providers: [RIPremInstitutionService],
})
export class RIPremInstitutionModule {}

import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ExternalUserViewEntity } from 'src/common/entities/external-user-view.entity';
import { BudgetingInstitutionsRepository } from './budgeting-institution.repository';
import { BudgetingInstitutionService } from './routing/budgeting-institution.service';
import { BudgetingInstitutionCrudController } from './routing/budgeting-institution-crud.controller';
import { BudgetingInstitutionCrudService } from './routing/budgeting-institution-crud.service';

@Module({
    imports: [TypeOrmModule.forFeature([BudgetingInstitutionsRepository, ExternalUserViewEntity])],
    providers: [BudgetingInstitutionService, BudgetingInstitutionCrudService],
    exports: [BudgetingInstitutionService, BudgetingInstitutionCrudService],
    controllers: [BudgetingInstitutionCrudController],
})
export class BudgetingInstitutionModule {}

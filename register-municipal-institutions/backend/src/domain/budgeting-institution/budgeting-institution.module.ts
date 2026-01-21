import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { BudgetingInstitutionService } from './routes/budgeting-institution/budgeting-institution.service';
import { BudgetingInstitutionController } from './routes/budgeting-institution/budgeting-institution.controller';
import { BudgetingInstitution } from './budgeting-institution.entity';

@Module({
    imports: [TypeOrmModule.forFeature([BudgetingInstitution])],
    exports: [TypeOrmModule],

    controllers: [BudgetingInstitutionController],
    providers: [BudgetingInstitutionService],
})
export class BudgetingInstitutionModule {}

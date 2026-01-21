import { BaseSchoolType } from '@domain/base-school-type/base-school-type.entity';
import { BudgetingInstitution } from '@domain/budgeting-institution/budgeting-institution.entity';
import { DetailedSchoolType } from '@domain/detailed-school-type/detailed-school-type.entity';
import { FinancialSchoolType } from '@domain/financial-school-type/financial-school-type.entity';
import { Town } from '@domain/town/town.entity';
import { IsNotEmpty } from 'class-validator';
import { Institution } from '../../institution.entity';

export class InstitutionDto extends Institution {
    @IsNotEmpty()
    Abbreviation: string;

    @IsNotEmpty()
    Name: string;

    Bulstat: string;

    @IsNotEmpty()
    BaseSchoolType: BaseSchoolType;

    @IsNotEmpty()
    FinancialSchoolType: FinancialSchoolType;

    @IsNotEmpty()
    DetailedSchoolType: DetailedSchoolType;

    @IsNotEmpty()
    BudgetingInstitution: BudgetingInstitution;

    Town: Town;
}

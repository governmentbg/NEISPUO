import { BaseSchoolType } from './base-school-type';
import { BudgetingInstitution } from './budgeting-institution';
import { Country } from './country';
import { DetailedSchoolType } from './detailed-school-type';
import { FinancialSchoolType } from './financial-school-type';
import { LocalArea } from './local-area';
import { CPLRAreaType } from './cplr-area-type';
import { RIProcedure } from './ri-procedure';
import { Town } from './town';

interface BaseInstitution {
  Name: string;
  Abbreviation: string;
  Bulstat: string;
  Country?: Country;
  LocalArea?: LocalArea;
  FinancialSchoolType?: FinancialSchoolType;
  DetailedSchoolType?: DetailedSchoolType;
  CPLRAreaType?: CPLRAreaType;
  BudgetingInstitution?: BudgetingInstitution;
  Town?: Town;
  BaseSchoolType?: BaseSchoolType;
}

// interface Institution extends BaseInstitution {
//   InstitutionID: number;
// }

export interface RIInstitution extends BaseInstitution {
  RIFlexFieldValues: any; // TODO: Replace any with model
  RIInstitutionID: number;
  InstitutionID?: number;
  TRAddress?: string;
  TRPostCode?: string;
  HeadFirstName?: string;
  HeadMiddleName?: string;
  HeadLastName?: string;
  RIProcedure?: RIProcedure;
  _newRIProcedure?: RIProcedure;
}

import { ProcedureType } from './procedure-type';
import { RICPLRArea } from './ri-cplr-area';
import { RIDocument } from './ri-document';
import { RIPremInstitution } from './ri-prem-institution';
import { TransformType } from './transform-type';

export interface RIProcedure {
  RIProcedureID: number;
  RICPLRArea?: RICPLRArea;
  YearDue?: number;
  ProcedureDate?: Date | string;
  TransformDetails?: string;
  RIDocument: RIDocument;
  ProcedureType?: ProcedureType;
  TransformType?: TransformType;
  RIPremInstitution?: RIPremInstitution;
}

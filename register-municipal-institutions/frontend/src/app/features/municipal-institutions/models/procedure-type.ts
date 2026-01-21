import { CPLRAreaType } from './cplr-area-type';
import { INomenclature } from './nomenclature';

export interface ProcedureType extends INomenclature {
  ProcedureTypeID: number;
  CPLRAreaTypes: CPLRAreaType[];
}

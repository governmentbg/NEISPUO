import { BaseSchoolType } from './base-school-type';
import { INomenclature } from './nomenclature';

export interface DetailedSchoolType extends INomenclature {
  DetailedSchoolTypeID: number;
  BaseSchoolType: BaseSchoolType;
}

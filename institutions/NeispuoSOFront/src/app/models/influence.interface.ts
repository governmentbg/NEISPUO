import { InfluenceType } from "../enums/influenceType.enum";

export interface Influence {
  fieldName?: string;
  sectionName?: string;
  subsectionName?: string;
  createNewTableName?: string;
  url?: string; // type === options/setValue
  condition?: any[]; // type === hide/disable/render/require
  fields?: string[]; // type === require
  message?: string; // type === require
  defaultValue?: string | number; // type === defaultValue
  type: InfluenceType;
  notNull?: boolean; // instead of condition for require
  value?: string | number; // type === setValueOppositeCondition
}

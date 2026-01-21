import { InfluenceType } from "../enums/influenceType.enum";

export interface Influence {
  fieldName?: string;
  sectionName?: string;
  subsectionName?: string;
  url?: string; // type === options
  condition?: string[]; // type === hide/disable
  value?: string | number; // type === setValue
  defaultValue?: string | number; // type === defaultValue
  type: InfluenceType;
}

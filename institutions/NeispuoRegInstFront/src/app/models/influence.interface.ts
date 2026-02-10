import { InfluenceType } from "../enums/influenceType.enum";

export interface Influence {
  fieldName?: string;
  sectionName?: string;
  subsectionName?: string;
  url?: string; // type === options
  condition?: any[]; // type === hide/disable/render
  fields?: string[]; // type === require
  message?: string; // type === require
  defaultValue?: string | number; // type === defaultValue
  value?: string | number; // type === setValue
  type: InfluenceType;
}

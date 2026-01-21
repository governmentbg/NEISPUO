import { Option } from "./option.interface";
import { Influence } from "./influence.interface";
import { Validator } from "./validator.interface";
import { FieldType } from "../enums/fieldType.enum";
import { InputType } from "../enums/inputType.enum";

export interface FieldConfig {
  type: FieldType;
  label: string;
  name: string;
  order: number;
  inputType?: InputType;
  optionsPath?: string;
  flexible?: boolean;
  influence?: Influence[];
  disabled?: boolean;
  rendered?: boolean;
  validations?: Validator[];
  width?: number; // for inputs and textareas
  defaultValue?: string | number; // for some selects
  info?: string;
  code?: string | number; // for table fields that are selects
  bold?: boolean; // for label fields
  size?: number; // for label fields in px
  color?: string; // for label fields
  procName?: string; // for button
  procParams?: Object; // for button
  canSign?: boolean; // for button
  searchByEik?: boolean; // for button with canSign=true
  requiredFields?: string[];
  generateCertificate?: boolean;
  regixData?: RegixData; // for button

  value?: any;
  dbValue?: any; // for defaultValue influenced fields
  options?: Option[];
  hidden?: boolean;
  requiredBy?: { fieldName: string; condition: (string | number)[]; notNull: boolean }[];
  influencedBy?: { fieldName: string; condition: (string | number)[]; url: string }[];
  hiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean; notNull: boolean }[];
  disabledBy?: { fieldName: string; condition: (string | number)[]; notNull: boolean }[];
  dependingTables?: string[];

  idntfr?: number;
  pseudoLabel?: string;
}

export interface RegixData {
  operation: string;
  xmlns: string;
  requestName: string;
  responseName: string;
  requestMap: { neispuoField: string; regixField: string }[];
  responseMap: {
    neispuoField: string;
    regixField?: string;
    regixFields?: { regixField: string; prefix: string }[];
    optionsPath?: string;
  }[];
}

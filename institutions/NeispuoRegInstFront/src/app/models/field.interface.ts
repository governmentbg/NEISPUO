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
  height?: number; // for textareas
  defaultValue?: string | number; // for some selects
  info?: string;
  code?: string | number; // for table fields that are selects

  value?: any;
  options?: Option[];
  hidden?: boolean;
  requiredBy?: string[];
  hiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean }[];
  disabledBy?: { fieldName: string; condition: (string | number)[] }[];

  idntfr?: number;
  pseudoLabel?: string;
}

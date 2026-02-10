import { FieldConfig } from "./field.interface";

export interface Subsection {
  fields?: FieldConfig[];
  canDuplicate?: boolean; // if not multiple property should not be present
  firstRecordDisabled?: boolean; // for duplicate subsections
  order?: number;
  label?: string;
  name?: string; // when subsection can be duplicated or is influenced by a field
  subsections?: Subsection[];
  atLeastOneRequired?: boolean; // when subsection can be duplicated
  rendered?: boolean;
  help?: string;
  accordion?: { state: "closed" | "opened"; dataName: string };
  info?: string;
  checkDelete?: { procedure: string; subsectionField: string; multiselectField: string }; // if canDuplicate = true
  bold?: boolean; // when label only
  size?: number; // when label only
  disabled?: boolean;

  id?: string; // when subsection can be duplicated
  position?: number; //when subsection can be duplicated
  subsectionRecordsCount?: number; //when subsection can be duplicated
  hidden?: boolean; //if influence hides it
  hiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean; notNull: boolean }[];
  disabledBy?: { fieldName: string; condition: (string | number)[]; notNull: boolean }[];
  dataLoaded?: boolean;
  labelOnly?: boolean;

  idntfr?: number;
  pseudoLabel?: string;
}

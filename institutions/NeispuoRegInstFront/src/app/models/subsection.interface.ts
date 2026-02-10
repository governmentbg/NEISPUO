import { FieldConfig } from "./field.interface";

export interface Subsection {
  fields?: FieldConfig[];
  canDuplicate?: boolean; // if not multiple property should not be present
  order?: number;
  label?: string;
  name?: string; // when subsection can be duplicated or is influenced by a field
  subsections?: Subsection[];
  atLeastOneRequired?: boolean; // when subsection can be duplicated
  rendered?: boolean;
  canHaveCertificate?: boolean;
  canHaveFiles?: boolean;

  id?: string; // when subsection can be duplicated
  position?: number; //when subsection can be duplicated
  subsectionRecordsCount?: number; //when subsection can be duplicated
  hidden?: boolean; //if influence hides it
  certificateLabel?: string;
  certificateId?: number;
  copyCertificateLabel?: string;
  copyCertificateId?: number;
  fileLabel?: string;
  fileId?: number;
  hiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean }[];
  disabledBy?: { fieldName: string; condition: (string | number)[] }[];

  idntfr?: number;
  pseudoLabel?: string;
}

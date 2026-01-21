import { FieldConfig } from "./field.interface";

export interface Subsection {
  fields?: FieldConfig[];
  canDuplicate?: boolean;
  order: number;
  label?: string;
  name?: string; // when subsection can be duplicated or is influenced by a field
  id?: string; // when subsection can be duplicated
  subsections?: Subsection[];
  atLeastOneRequired?: boolean; // when subsection can be duplicated
  rendered?: boolean;
  canHaveCertificate?: boolean;
  canHaveFiles?: boolean;

  position?: number; //when subsection can be duplicated
  subsectionRecordsCount?: number; //when subsection can be duplicated
  hidden?: boolean; //if influence hides it
  certificateLabel?: string;
  certificateId?: number;
  copyCertificateLabel?: string;
  copyCertificateId?: number;
  fileLabel?: string;
  fileId?: number;
}

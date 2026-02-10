import { Section } from "./section.interface";

export interface Form {
  sections: Section[];
  label?: string;
  procedureName?: string;
  dataName?: string;
  statusFieldName?: string;

  idntfr?: number;
  isDraft?: boolean;
  canBeAnnulled?: boolean;
  canBeRenewed?: boolean;
  hideCertificateBtn?: boolean;
}

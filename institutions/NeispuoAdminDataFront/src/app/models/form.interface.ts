import { Section } from "./section.interface";

export interface Form {
  sections: Section[];
  label?: string;
  procedureName?: string;
  dataName?: string;
  changeData?: boolean;
  forceOperation?: boolean; // for update

  idntfr?: number;
}

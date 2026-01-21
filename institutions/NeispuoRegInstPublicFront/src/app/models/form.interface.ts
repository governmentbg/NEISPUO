import { Section } from "./section.interface";

export interface Form {
  sections: Section[];
  submitUrl?: string;
}

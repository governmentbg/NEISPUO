import { Subsection } from "./subsection.interface";
import { Table } from "./table.interface";

export interface Section {
  label?: string;
  subsections?: Subsection[];
  order?: number;
  name?: string; // when influenced by a field
  table?: Table;
  rendered?: boolean;

  hidden?: boolean; // if influence hides it
}

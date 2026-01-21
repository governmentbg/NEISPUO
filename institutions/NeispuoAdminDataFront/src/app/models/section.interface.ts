import { Subsection } from "./subsection.interface";
import { Table } from "./table.interface";

export interface Section {
  label?: string;
  subsections?: Subsection[];
  order: number;
  name?: string; // when influenced by a field
  table?: Table;
  rendered?: boolean;
  help?: string;
  accordion?: { state: "closed" | "opened"; dataName: string };
  info?: string;
  bold?: boolean; // when label only
  size?: number; // when label only

  hidden?: boolean; // if influence hides it
  disabled?: boolean;
  hiddenBy?: { fieldName: string; condition: (string | number)[]; hide: boolean; notNull: boolean }[];
  disabledBy?: { fieldName: string; condition: (string | number)[]; notNull: boolean }[];
  dataLoaded?: boolean;
  labelOnly?: boolean;
  loading?: boolean; // on accordion open

  idntfr?: number;
  pseudoLabel?: string;
}

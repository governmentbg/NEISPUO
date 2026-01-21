import { Form } from "@angular/forms";
import { FocusedElementType } from "../enums/focusedElementType";
import { FieldConfig } from "./field.interface";
import { Section } from "./section.interface";
import { Subsection } from "./subsection.interface";
import { Table } from "./table.interface";

export interface FocusedElement {
  type: FocusedElementType;
  element: FieldConfig | Section | Subsection | Form | Table;
  index: number;
}

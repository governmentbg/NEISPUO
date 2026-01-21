import { BreakpointComponent } from "../pages/dynamic-components/breakpoint/breakpoint.component";
import { ButtonComponent } from "../pages/dynamic-components/button/button.component";
import { CheckboxComponent } from "../pages/dynamic-components/checkbox/checkbox.component";
import { DateComponent } from "../pages/dynamic-components/date/date.component";
import { InputComponent } from "../pages/dynamic-components/input/input.component";
import { RadiobuttonComponent } from "../pages/dynamic-components/radiobutton/radiobutton.component";
import { SearchmultiselectComponent } from "../pages/dynamic-components/searchmultiselect/searchmultiselect.component";
import { SearchSelectComponent } from "../pages/dynamic-components/searchselect/searchselect.component";
import { SelectComponent } from "../pages/dynamic-components/select/select.component";
import { TextareaComponent } from "../pages/dynamic-components/textarea/textarea.component";

export const componentMapper = {
  input: InputComponent,
  button: ButtonComponent,
  select: SelectComponent,
  date: DateComponent,
  radiobutton: RadiobuttonComponent,
  checkbox: CheckboxComponent,
  textarea: TextareaComponent,
  searchselect: SearchSelectComponent,
  searchmultiselect: SearchmultiselectComponent,
  breakpoint: BreakpointComponent
};

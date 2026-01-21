import { CheckboxAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/checkbox-admin/checkbox-admin.component";
import { DateAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/date-admin/date-admin.component";
import { InputAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/input-admin/input-admin.component";
import { SearchselectAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/searchselect-admin/searchselect-admin.component";
import { SelectAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/select-admin/select-admin.component";
import { TextareaAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/textarea-admin/textarea-admin.component";
import { BreakpointComponent } from "../pages/dynamic-components/breakpoint/breakpoint.component";
import { CheckboxComponent } from "../pages/dynamic-components/checkbox/checkbox.component";
import { DateComponent } from "../pages/dynamic-components/date/date.component";
import { InfoComponent } from "../pages/dynamic-components/info/info.component";
import { InputComponent } from "../pages/dynamic-components/input/input.component";
import { SearchSelectComponent } from "../pages/dynamic-components/searchselect/searchselect.component";
import { SelectComponent } from "../pages/dynamic-components/select/select.component";
import { TextareaComponent } from "../pages/dynamic-components/textarea/textarea.component";

export const componentMapper = {
  input: InputComponent,
  select: SelectComponent,
  date: DateComponent,
  checkbox: CheckboxComponent,
  textarea: TextareaComponent,
  searchselect: SearchSelectComponent,
  breakpoint: BreakpointComponent,
  info: InfoComponent
};

export const componentMapperAdmin = {
  input: InputAdminComponent,
  select: SelectAdminComponent,
  date: DateAdminComponent,
  checkbox: CheckboxAdminComponent,
  textarea: TextareaAdminComponent,
  searchselect: SearchselectAdminComponent,
  breakpoint: BreakpointComponent,
  info: InfoComponent
};

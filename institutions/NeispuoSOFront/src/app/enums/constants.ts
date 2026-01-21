import { CheckboxAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/checkbox-admin/checkbox-admin.component";
import { DateAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/date-admin/date-admin.component";
import { FileAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/file-admin/file-admin.component";
import { InfoAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/info-admin/info-admin.component";
import { InputAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/input-admin/input-admin.component";
import { LabelAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/label-admin/label-admin.component";
import { MultiselectAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/multiselect-admin/multiselect-admin.component";
import { SearchselectAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/searchselect-admin/searchselect-admin.component";
import { SelectAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/select-admin/select-admin.component";
import { TextareaAdminComponent } from "../pages/admin/center-bar/create-edit-form-admin/dynamic-components/textarea-admin/textarea-admin.component";
import { BreakpointComponent } from "../pages/dynamic-components/breakpoint/breakpoint.component";
import { ButtonComponent } from "../pages/dynamic-components/button/button.component";
import { CheckboxComponent } from "../pages/dynamic-components/checkbox/checkbox.component";
import { DateTimeComponent } from "../pages/dynamic-components/date-time/date-time.component";
import { DateComponent } from "../pages/dynamic-components/date/date.component";
import { FileComponent } from "../pages/dynamic-components/file/file.component";
import { InfoComponent } from "../pages/dynamic-components/info/info.component";
import { InputComponent } from "../pages/dynamic-components/input/input.component";
import { LabelComponent } from "../pages/dynamic-components/label/label.component";
import { MultiselectComponent } from "../pages/dynamic-components/multiselect/multiselect.component";
import { RegixButtonComponent } from "../pages/dynamic-components/regix-button/regix-button.component";
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
  multiselect: MultiselectComponent,
  label: LabelComponent,
  info: InfoComponent,
  breakpoint: BreakpointComponent,
  file: FileComponent,
  button: ButtonComponent,
  datetime: DateTimeComponent,
  regixbutton: RegixButtonComponent
};

export const componentMapperAdmin = {
  input: InputAdminComponent,
  select: SelectAdminComponent,
  date: DateAdminComponent,
  checkbox: CheckboxAdminComponent,
  textarea: TextareaAdminComponent,
  searchselect: SearchselectAdminComponent,
  multiselect: MultiselectAdminComponent,
  label: LabelAdminComponent,
  info: InfoAdminComponent,
  breakpoint: BreakpointComponent,
  file: FileAdminComponent,
  button: ButtonComponent,
  datetime: DateTimeComponent,
  regixbutton: RegixButtonComponent
};

export const monRuoMenu = {
  school: "Училище",
  kindergarden: "Детска градина",
  cplr: "ЦПЛР/СОЗ",
  csop: "ЦСОП",
  infotable: "Информационно табло",
  settings: "Настройки"
};

export const headmasterMenu = {
  main: "Основно меню",
  list: "Подаване на Списък-образец",
  // data: "Преглед на подадени данни",
  physical: "Физическа среда",
  history: "История"
};

export const instType = ["school", "kindergarden", "cplr", "csop", "csop"];

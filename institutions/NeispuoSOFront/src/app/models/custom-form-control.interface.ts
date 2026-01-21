import { FormControl, FormGroup } from "@angular/forms";
import { FieldType } from '../enums/fieldType.enum';

export interface CustomFormControl extends FormControl {
  hidden: boolean;
  code: string | number;
  type: FieldType;
}

export interface CustomFormGroup extends FormGroup {
  opened: boolean;
}

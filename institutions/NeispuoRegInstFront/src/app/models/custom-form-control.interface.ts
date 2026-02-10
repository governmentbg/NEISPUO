import { FormControl } from "@angular/forms";
import { FieldType } from '../enums/fieldType.enum';

export interface CustomFormControl extends FormControl {
  hidden: boolean;
  code: string | number;
  type: FieldType;
}

import { Validators } from "@angular/forms";
import { ValidatorType } from "../enums/validatorType.enum";

export interface Validator {
  name: ValidatorType;
  validation: any;
  message: string;
  
  validator?: Validators;
}

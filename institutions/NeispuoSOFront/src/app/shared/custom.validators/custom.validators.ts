import { AbstractControl, FormControl, ValidationErrors } from "@angular/forms";
import { CustomFormControl } from "../../models/custom-form-control.interface";

export class CustomValidators {
  public static mustMatch(controlName: string) {
    return (control: FormControl): ValidationErrors => {
      if (!control.parent) return null;

      const matchingControl = control.parent.controls[controlName];

      if (!control || !matchingControl) {
        return null;
      }

      if (control.value !== matchingControl.value) {
        return {
          mustMatch: {
            valid: false
          }
        };
      } else {
        control.setErrors(null);
        return null;
      }
    };
  }

  public static validateDate(control: AbstractControl): object | null {
    if (!control || !control.value) {
      return null;
    }
    const isDateValid = control.value.getTime() === control.value.getTime();

    if (!isDateValid) return { validateDate: { valid: false } };
    else {
      control.setErrors(null);
      return null;
    }
  }

  public static requiredControl(control: AbstractControl): object | null {
    if (!control) {
      return null;
    }

    const trimmedValue = typeof control.value === "string" ? control.value.trim() : control.value;
    
    if (!trimmedValue && control.value !== 0 && !(control as CustomFormControl).hidden) {
      return { requiredControl: true };
    } else {
      return null;
    }
  }
}

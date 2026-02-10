import { Directive } from "@angular/core";
import { Validator, ValidatorFn, FormControl, NG_VALIDATORS } from "@angular/forms";

@Directive({
  selector: "[notLnchValidator]",
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: NotLnchValidator,
      multi: true
    }
  ]
})
export class NotLnchValidator implements Validator {
  validator: ValidatorFn;

  constructor() {
    this.validator = this.idnValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  idnValidator(): ValidatorFn {
    return (c: FormControl) => {
      let isValidLnch = new RegExp("^s*([0-9]{10}|.{0})s*$", "i").test(c.value);
      if (isValidLnch && c.value.length === 10) {
        const lnchWeights = [21, 19, 17, 13, 11, 9, 7, 3, 1];

        const checkSum = +c.value.substring(9, 10);
        let lnchSum = 0;
        for (let i = 0; i < 9; i++) {
          lnchSum += c.value.substring(i, i + 1) * lnchWeights[i];
        }

        const validChecksum = lnchSum % 10;
        if (checkSum !== validChecksum) {
          isValidLnch = false;
        }
      }

      if (!isValidLnch) {
        return null;
      } else {
        return {
          validLnch: {
            valid: false
          }
        };
      }
    };
  }
}

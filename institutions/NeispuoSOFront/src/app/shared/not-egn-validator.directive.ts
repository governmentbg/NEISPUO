import { Directive } from "@angular/core";
import { Validator, ValidatorFn, FormControl, NG_VALIDATORS } from "@angular/forms";

@Directive({
  selector: "[notEgnValidator]",
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: NotEgnValidator,
      multi: true
    }
  ]
})
export class NotEgnValidator implements Validator {
  validator: ValidatorFn;

  constructor() {
    this.validator = this.idnValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  idnValidator(): ValidatorFn {
    return (c: FormControl) => {
      let isValidEgn = new RegExp("^s*([0-9]{10}|.{0})s*$", "i").test(c.value);
      if (isValidEgn && c.value.length === 10) {
        const engWeights = [2, 4, 8, 5, 10, 9, 7, 3, 6];
        const year = +c.value.substring(0, 2);
        const month = +c.value.substring(2, 4) - 1;
        const day = +c.value.substring(4, 6);

        if (!this.isValidEgnDate(year, month, day)) {
          isValidEgn = false;
        } else {
          const checkSum = +c.value.substring(9, 10);
          let egnSum = 0;
          for (let i = 0; i < 9; i++) {
            egnSum += c.value.substring(i, i + 1) * engWeights[i];
          }

          let validChecksum = egnSum % 11;
          if (validChecksum === 10) {
            validChecksum = 0;
          }
          if (checkSum !== validChecksum) {
            isValidEgn = false;
          }
        }
      }

      if (!isValidEgn) {
        return null;
      } else {
        return {
          validEgn: {
            valid: false
          }
        };
      }
    };
  }

  private isValidEgnDate(year: number, month: number, day: number) {
    if (month >= 40) {
      if (!this.isValidDate(year + 2000, month - 40, day)) {
        return false;
      }
    } else if (month >= 20) {
      if (!this.isValidDate(year + 1800, month - 20, day)) {
        return false;
      }
    } else {
      if (!this.isValidDate(year + 1900, month, day)) {
        return false;
      }
    }

    return true;
  }

  private isValidDate(year: number, month: number, day: number) {
    const date = new Date(year, month, day);

    return date.getFullYear() === year && date.getMonth() === month && date.getDate() === day;
  }
}

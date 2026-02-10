import { Pipe, PipeTransform } from "@angular/core";
import { FormType } from "../enums/formType.enum";

@Pipe({
  name: "formType"
})
export class FormTypeDisplayPipe implements PipeTransform {
  transform(value: string): string {
    switch (value) {
      case FormType.Cplr:
        return "Център за подкрепа за личностно развитие";
      case FormType.Csop:
        return "Център за специална образователна подкрепа ";
      case FormType.Kindergarden:
        return "Детска градина";
      case FormType.School:
        return "Училище";
      default:
        return value;
    }
  }
}

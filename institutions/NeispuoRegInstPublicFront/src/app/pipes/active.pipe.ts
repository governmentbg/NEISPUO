import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "active"
})
export class ActiveDisplayPipe implements PipeTransform {
  transform(value: boolean): string {
    return value ? "Закрити" : "Действащи";
  }
}

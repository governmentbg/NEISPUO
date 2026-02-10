import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "direction"
})
export class DirectionDisplayPipe implements PipeTransform {
  transform(value: string): string {
    return value === "asc" ? "↑" : "↓";
  }
}

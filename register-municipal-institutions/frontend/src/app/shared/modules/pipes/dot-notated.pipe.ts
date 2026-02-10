import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dotNotated',
  pure: false,
})
export class DotNotatedPipe implements PipeTransform {
  transform([value, dotNotatedProperty]: [any, string], ...args: unknown[]): any {
    const path = dotNotatedProperty.split('.');

    for (const segment of path) {
      value = value[segment];
      if (!value) {
        break;
      }
    }

    return value;
  }
}

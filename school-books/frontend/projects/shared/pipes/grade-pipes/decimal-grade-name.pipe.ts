import { Pipe, PipeTransform } from '@angular/core';
import { formatDecimalGradeName } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbDecimalGradeName'
})
export class DecimalGradeNamePipe implements PipeTransform {
  transform(value?: number | null): string {
    if (value == null) return '';

    return formatDecimalGradeName(value);
  }
}

import { Pipe, PipeTransform } from '@angular/core';
import { formatDecimalGradeNumber } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbDecimalGradeNumber'
})
export class DecimalGradeNumberPipe implements PipeTransform {
  transform(value?: number | null): string {
    if (value == null) return '';

    return formatDecimalGradeNumber(value);
  }
}

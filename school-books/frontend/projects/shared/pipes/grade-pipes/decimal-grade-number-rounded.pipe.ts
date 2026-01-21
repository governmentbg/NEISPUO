import { Pipe, PipeTransform } from '@angular/core';
import { formatDecimalGradeNumberRounded } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbDecimalGradeNumberRounded'
})
export class DecimalGradeNumberRoundedPipe implements PipeTransform {
  transform(value?: number | null): string {
    if (value == null) return '';

    return formatDecimalGradeNumberRounded(value);
  }
}

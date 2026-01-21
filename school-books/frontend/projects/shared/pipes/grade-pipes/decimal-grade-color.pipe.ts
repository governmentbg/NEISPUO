import { Pipe, PipeTransform } from '@angular/core';
import { formatDecimalGradeColor } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbDecimalGradeColor'
})
export class DecimalGradeColorPipe implements PipeTransform {
  transform(value?: number | null): string {
    if (value == null) return '';

    return formatDecimalGradeColor(value);
  }
}

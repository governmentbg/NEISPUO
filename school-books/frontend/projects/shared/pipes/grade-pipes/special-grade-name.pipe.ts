import { Pipe, PipeTransform } from '@angular/core';
import { SpecialNeedsGrade } from 'projects/sb-api-client/src/model/specialNeedsGrade';
import { formatSpecialGradeName } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbSpecialGradeName'
})
export class SpecialGradeNamePipe implements PipeTransform {
  transform(value?: SpecialNeedsGrade | null): string {
    if (value == null) return '';

    return formatSpecialGradeName(value);
  }
}

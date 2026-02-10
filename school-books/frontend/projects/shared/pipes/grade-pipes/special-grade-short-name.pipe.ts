import { Pipe, PipeTransform } from '@angular/core';
import { SpecialNeedsGrade } from 'projects/sb-api-client/src/model/specialNeedsGrade';
import { formatSpecialGradeShortName } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbSpecialGradeShortName'
})
export class SpecialGradeShortNamePipe implements PipeTransform {
  transform(value?: SpecialNeedsGrade | null): string {
    if (value == null) return '';

    return formatSpecialGradeShortName(value);
  }
}

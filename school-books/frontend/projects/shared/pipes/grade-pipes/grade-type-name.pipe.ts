import { Pipe, PipeTransform } from '@angular/core';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { formatGradeTypeName } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbGradeTypeName'
})
export class GradeTypeNamePipe implements PipeTransform {
  transform(value?: GradeType | null): string {
    if (value == null) return '';

    return formatGradeTypeName(value);
  }
}

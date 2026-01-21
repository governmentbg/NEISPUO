import { Pipe, PipeTransform } from '@angular/core';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { formatQualitativeGradeName } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbQualitativeGradeName'
})
export class QualitativeGradeNamePipe implements PipeTransform {
  transform(value?: QualitativeGrade | null): string {
    if (value == null) return '';

    return formatQualitativeGradeName(value);
  }
}

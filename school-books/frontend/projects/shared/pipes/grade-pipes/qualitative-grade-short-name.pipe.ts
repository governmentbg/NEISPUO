import { Pipe, PipeTransform } from '@angular/core';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { formatQualitativeGradeShortName } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbQualitativeGradeShortName'
})
export class QualitativeGradeShortNamePipe implements PipeTransform {
  transform(value?: QualitativeGrade | null): string {
    if (value == null) return '';

    return formatQualitativeGradeShortName(value);
  }
}

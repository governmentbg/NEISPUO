import { Pipe, PipeTransform } from '@angular/core';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { formatQualitativeGradeColor } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbQualitativeGradeColor'
})
export class QualitativeGradeColorPipe implements PipeTransform {
  transform(value?: QualitativeGrade | null): string {
    if (value == null) return '';

    return formatQualitativeGradeColor(value);
  }
}

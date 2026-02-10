import { Pipe, PipeTransform } from '@angular/core';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { formatQualitativeGradeEmoticon } from 'projects/shared/utils/book';

@Pipe({
  name: 'sbQualitativeGradeEmoticon'
})
export class QualitativeGradeEmoticonPipe implements PipeTransform {
  transform(value?: QualitativeGrade | null): string {
    if (value == null) return '';

    return formatQualitativeGradeEmoticon(value);
  }
}

import { Pipe, PipeTransform } from '@angular/core';
import { formatDate } from 'projects/shared/utils/date';

@Pipe({
  name: 'sbDate'
})
export class DatePipe implements PipeTransform {
  transform(value?: Date | null): string {
    if (value == null) return '';

    return formatDate(value);
  }
}

import { Pipe, PipeTransform } from '@angular/core';
import { formatDateTime } from 'projects/shared/utils/date';

@Pipe({
  name: 'sbDateTime'
})
export class DateTimePipe implements PipeTransform {
  transform(value?: Date | null): string {
    if (value == null) return '';

    return formatDateTime(value);
  }
}

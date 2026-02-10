import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'topicTitlesList'
})
export class TooltipListPipe implements PipeTransform {
  transform(lines?: string[] | null): string {
    return lines ? (lines.length > 1 ? lines.map((l) => '\u2022 ' + l).join('\n') : lines[0]) : '';
  }
}

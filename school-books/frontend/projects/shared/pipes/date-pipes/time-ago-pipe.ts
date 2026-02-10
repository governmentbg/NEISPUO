import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo'
})
export class TimeAgoPipe implements PipeTransform {
  transform(value: Date | string | number): string {
    if (!value) return '';

    const date = new Date(value);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (seconds < 60) {
      return 'преди секунди';
    }

    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) {
      return `преди ${minutes} минути`;
    }

    const hours = Math.floor(minutes / 60);
    if (hours < 24) {
      return `преди ${hours} часа`;
    }

    const days = Math.floor(hours / 24);
    if (days < 30) {
      return `преди ${days} дни`;
    }

    const months = Math.floor(days / 30);
    if (months < 12) {
      return `преди ${months} месеца`;
    }

    const years = Math.floor(days / 365);
    return `преди ${years} години`;
  }
}

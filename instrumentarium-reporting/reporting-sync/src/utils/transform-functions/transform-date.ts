import { DateTime } from 'luxon';

export function transformDate(value: string): any {
  if (!value || isNaN(Date.parse(value))) {
    return value;
  }

  const date = new Date(value);
  if (!isNaN(date.getTime())) {
    const dt = DateTime.fromJSDate(date);
    return dt.toFormat('yyyy-MM-dd HH:mm:ss.SSS');
  } else {
    throw new Error(`Invalid date value: ${value}`);
  }
}
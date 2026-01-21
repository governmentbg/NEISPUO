import { format, isValid, parse } from 'date-fns';
import { bg } from 'date-fns/locale';

export const DATE_PARSE_FORMATS = [
  'dd.MM',
  'dd_MM',
  'dd-MM',
  'dd/MM',
  'dd\\MM',
  'dd.MM.yyyy',
  'dd_MM_yyyy',
  'dd-MM-yyyy',
  'dd/MM/yyyy',
  'dd\\MM\\yyyy'
];

export const DATE_FORMAT = 'dd.MM.yyyy';

export const DATE_TIME_FORMAT = 'dd.MM.yyyy HH:mm';

export const TIME_FORMAT = 'HH:mm';

export const MONTH_FORMAT = 'yyyy-MM';

export function parseMultiple(
  dateString: string,
  formatString: string | Array<string>,
  referenceDate: Date | number,
  options?: {
    locale?: Locale;
    weekStartsOn?: 0 | 1 | 2 | 3 | 4 | 5 | 6;
    firstWeekContainsDate?: 1 | 2 | 3 | 4 | 5 | 6 | 7;
    useAdditionalWeekYearTokens?: boolean;
    useAdditionalDayOfYearTokens?: boolean;
  }
): Date {
  let result = new Date(NaN);

  if (Array.isArray(formatString)) {
    for (let i = 0; i < formatString.length; i++) {
      result = parse(dateString, formatString[i], referenceDate, options);
      if (isValid(result)) {
        break;
      }
    }
  } else {
    result = parse(dateString, formatString, referenceDate, options);
  }

  return result;
}

export function parseDate(value: string, formatString: string | Array<string>): Date {
  return parseMultiple(value, formatString ?? DATE_PARSE_FORMATS, new Date(), { locale: bg });
}

export function formatNullableDate(date: Date | null | undefined, formatString?: string): string {
  if (date) {
    return formatDate(date, formatString);
  }
  return '';
}

export function formatDate(date: Date, formatString?: string): string {
  return format(date, formatString ?? DATE_FORMAT, { locale: bg });
}

export function formatDateTime(date: Date, formatString?: string): string {
  return format(date, formatString ?? DATE_TIME_FORMAT, { locale: bg });
}

export function parseMonth(value: string): Date {
  return parseMultiple(value, MONTH_FORMAT, new Date(), { locale: bg });
}

export function formatMonth(date: Date): string {
  return format(date, MONTH_FORMAT, { locale: bg });
}

export const hourRegex = /^([0-9]|0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])$/;

export function getTimeFormatHint(): string {
  return format(new Date(), TIME_FORMAT, { locale: bg });
}

export function isExpired(dueDate: Date): boolean {
  const timeLeft = dueDate.getTime() - Date.now();
  return timeLeft <= 0;
}

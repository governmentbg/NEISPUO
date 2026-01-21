import { Injectable } from '@angular/core';
import { DateAdapter } from '@angular/material/core';
import { addDays, addMonths, addYears, format, getDaysInMonth, isValid } from 'date-fns';
import { bg } from 'date-fns/locale';
import { DATE_FORMAT, DATE_PARSE_FORMATS, formatDate, parseDate } from 'projects/shared/utils/date';

export const SB_DATE_FORMATS = {
  parse: {
    dateInput: DATE_PARSE_FORMATS
  },
  display: {
    dateInput: DATE_FORMAT,
    monthYearLabel: 'MMM yyyy', // дек 2020
    dateA11yLabel: 'do MMMM yyyy', // 31-ви декември 2020
    monthYearA11yLabel: 'MMMM yyyy' // декември 2020
  }
};

/**
 * Matches strings that have the form of a valid RFC 3339 string
 * (https://tools.ietf.org/html/rfc3339). Note that the string may not actually be a valid date
 * because the regex will match strings an with out of bounds month, date, etc.
 */
const ISO_8601_REGEX = /^\d{4}-\d{2}-\d{2}(?:T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|(?:(?:\+|-)\d{2}:\d{2}))?)?$/;

@Injectable()
export class SbDateAdapter extends DateAdapter<Date> {
  getYear(date: Date): number {
    return date.getFullYear();
  }

  getMonth(date: Date): number {
    return date.getMonth();
  }

  getDate(date: Date): number {
    return date.getDate();
  }

  getDayOfWeek(date: Date): number {
    return date.getDay();
  }

  getMonthNames(style: 'long' | 'short' | 'narrow'): string[] {
    let formatString: string;

    switch (style) {
      case 'long':
        formatString = 'LLLL';
        break;
      case 'short':
        formatString = 'LLL';
        break;
      case 'narrow':
        formatString = 'LLLLL';
        break;
      default:
        throw new Error();
    }

    return [...Array(12).keys()].map((i) => format(new Date(2017, i, 1), formatString, { locale: bg }));
  }

  getDateNames(): string[] {
    return [...Array(31).keys()].map((i) => format(new Date(2017, 0, i + 1), 'd', { locale: bg }));
  }

  getDayOfWeekNames(style: 'long' | 'short' | 'narrow'): string[] {
    let formatString: string;

    switch (style) {
      case 'long':
        formatString = 'EEEE';
        break;
      case 'short':
        formatString = 'EEE';
        break;
      case 'narrow':
        formatString = 'EEEEE';
        break;
      default:
        throw new Error();
    }

    // This works because 01.01.2017 falls on a Sunday
    return [...Array(7).keys()].map((i) => format(new Date(2017, 0, i + 1), formatString, { locale: bg }));
  }

  getYearName(date: Date): string {
    return format(date, 'yyyy', { locale: bg });
  }

  getFirstDayOfWeek(): number {
    return 1; // Monday
  }

  getNumDaysInMonth(date: Date): number {
    return getDaysInMonth(date);
  }

  clone(date: Date): Date {
    return new Date(date.getTime());
  }

  createDate(year: number, month: number, date: number): Date {
    // Check for invalid month and date (except upper bound on date which we have to check after
    // creating the Date).
    if (month < 0 || month > 11) {
      throw Error(`Invalid month index "${month}". Month index has to be between 0 and 11.`);
    }

    if (date < 1) {
      throw Error(`Invalid date "${date}". Date has to be greater than 0.`);
    }

    const result = this._createDateWithOverflow(year, month, date);

    // Check that the date wasn't above the upper bound for the month, causing the month to overflow
    if (result.getMonth() !== month) {
      throw Error(`Invalid date "${date}" for month with index "${month}".`);
    }

    return result;
  }

  today(): Date {
    return new Date();
  }

  parse(value: any, parseFormat: string | Array<string>): Date | null {
    if (value == null) {
      return null;
    }

    if (typeof value === 'number') {
      return new Date(value);
    } else if (typeof value === 'string') {
      return parseDate(value, parseFormat);
    } else {
      return this.invalid();
    }
  }

  format(date: Date, displayFormat: string): string {
    if (!this.isValid(date)) {
      throw Error('Cannot format invalid date.');
    }

    return formatDate(date, displayFormat);
  }

  addCalendarYears(date: Date, years: number): Date {
    return addYears(date, years);
  }

  addCalendarMonths(date: Date, months: number): Date {
    return addMonths(date, months);
  }

  addCalendarDays(date: Date, days: number): Date {
    return addDays(date, days);
  }

  toIso8601(date: Date): string {
    return [date.getUTCFullYear(), this._2digit(date.getUTCMonth() + 1), this._2digit(date.getUTCDate())].join('-');
  }

  /**
   * Returns the given value if given a valid Date or null. Deserializes valid ISO 8601 strings
   * (https://www.ietf.org/rfc/rfc3339.txt) into valid Dates and empty string into null. Returns an
   * invalid date for all other values.
   */
  deserialize(value: any): Date | null {
    if (typeof value === 'string') {
      if (!value) {
        return null;
      }
      // The `Date` constructor accepts formats other than ISO 8601, so we need to make sure the
      // string is the right format first.
      if (ISO_8601_REGEX.test(value)) {
        const date = new Date(value);
        if (this.isValid(date)) {
          return date;
        }
      }
    }
    return super.deserialize(value);
  }

  isDateInstance(obj: any) {
    return obj instanceof Date;
  }

  isValid(date: Date) {
    return isValid(date);
  }

  invalid(): Date {
    return new Date(NaN);
  }

  /** Creates a date but allows the month and date to overflow. */
  private _createDateWithOverflow(year: number, month: number, date: number) {
    // Passing the year to the constructor causes year numbers <100 to be converted to 19xx.
    // To work around this we use `setFullYear` and `setHours` instead.
    const d = new Date();
    d.setFullYear(year, month, date);
    d.setHours(0, 0, 0, 0);
    return d;
  }

  /**
   * Pads a number to make it two digits.
   * @param n The number to pad.
   * @returns The padded number.
   */
  private _2digit(n: number) {
    return ('00' + n).slice(-2);
  }
}

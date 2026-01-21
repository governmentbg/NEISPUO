import { SchoolYearDateInterval } from './interfaces/school-year-date-interval.interface';

export class SchoolYearService {
    /**
     * Returns the end date of the previous school year (August 31st).
     * If called before August 31st, returns August 31st of the previous year.
     * If called on or after August 31st, returns August 31st of the current year.
     */
    static getPreviousSchoolYearEndDate(): Date {
        const now = new Date();
        const currentYear = now.getFullYear();
        const currentMonth = now.getMonth(); // 0-11, where 7 is August

        // If we're before August 31st, return last year's end date
        // If we're in August or later, return this year's end date
        const targetYear = currentMonth < 7 ? currentYear - 1 : currentYear;

        return this.createEndDate(targetYear);
    }

    /**
     * Maps a school year to a date interval
     * @param schoolYear - The school year (e.g., 2022 or 2022/2023)
     * @returns Date interval for the school year
     */
    static mapSchoolYearToDateInterval(schoolYear: string | number): SchoolYearDateInterval {
        const year = this.extractYear(schoolYear);
        const startDate = new Date(year, 8, 1, 0, 0, 0, 0);
        const endDate = this.createEndDate(year + 1);

        return {
            startDate,
            endDate,
        };
    }

    /**
     * Validates if a school year format is valid
     * @param schoolYear - The school year to validate
     * @returns true if valid, false otherwise
     */
    static isValidSchoolYear(schoolYear: string | number): boolean {
        try {
            const year = this.extractYear(schoolYear);
            return !isNaN(year);
        } catch {
            return false;
        }
    }

    private static extractYear(schoolYear: string | number): number {
        if (typeof schoolYear === 'string') {
            const yearParts = schoolYear.split('/');
            if (yearParts.length > 2) {
                throw new Error('Invalid school year format');
            }
            return parseInt(yearParts[0], 10);
        }
        return schoolYear;
    }

    private static createEndDate(year: number): Date {
        return new Date(year, 7, 31, 23, 59, 59, 999);
    }
}

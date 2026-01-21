

export function getPreviousSchoolYearEndDate(): string[] {
  const now = new Date();
  const currentYear = now.getFullYear();
  const currentMonth = now.getMonth(); // 0-11, where 7 is August

  // If we're before August 31st, return last year's end date
  // If we're in August or later, return this year's end date
  const targetYear = currentMonth < 7 ? currentYear - 1 : currentYear;

  return [createEndDate(targetYear).toISOString()];
}

function createEndDate(year: number): Date {
  return new Date(year, 7, 31, 23, 59, 59, 999);
}
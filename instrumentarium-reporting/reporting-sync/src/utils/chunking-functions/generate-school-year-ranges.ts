import { START_YEAR } from "../../constants/start-year";

export function generateSchoolYearRanges(): string[] {
  const currentDate = new Date();
  const currentYear = currentDate.getFullYear();
  const currentMonth = currentDate.getMonth() + 1;
  const currentDay = currentDate.getDate();
  
  let currentSchoolYear: number;
  if (currentMonth > 9 || (currentMonth === 9 && currentDay >= 15)) {
    currentSchoolYear = currentYear;
  } else {
    currentSchoolYear = currentYear - 1;
  }
  
  const years: string[] = [];
  for (let year = START_YEAR; year <= currentSchoolYear; year++) {
    years.push(year.toString());
  }
  
  return years;
}

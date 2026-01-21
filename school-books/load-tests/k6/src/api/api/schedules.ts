import http from 'k6/http';
import { checkIsSuccessfulJson, json } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export function GetScheduleLessons(
  schoolYear: number,
  instId: number,
  classBookId: number,
  curriculumId: number,
  date: string
) {
  const url =
    BASE_URL +
    `/api/schedulelessonnoms/${schoolYear}/${instId}/${classBookId}/getnoms?curriculumId=${curriculumId}&date=${date}`;
  const res = http.get(url, REQUEST_OPTIONS);
  return json(res) as {
    scheduleLessonId: number;
  }[];
}

export function GetClassBookScheduleForWeek(
  schoolYear: number,
  instId: number,
  classBookId: number,
  year: number,
  weekNumber: number
) {
  const url =
    BASE_URL +
    `/api/schedules/${schoolYear}/${instId}/${classBookId}/getclassbookscheduleforweek/${year}/${weekNumber}`;
  const res = http.get(url, REQUEST_OPTIONS);
  if (!checkIsSuccessfulJson(res)) {
    return null;
  }

  return json(res) as {
    hours: {
      scheduleLessonId: number;
      date: string;
      day: number;
      hourNumber: number;
      curriculumId: number;
      curriculumGroupName: string | null;
      subjectName: string;
      subjectNameShort: string;
      subjectTypeName: string;
      curriculumTeachers: {
        teacherPersonId: number;
        teacherFirstName: string;
        teacherLastName: string;
      }[];
    }[];
    offDays: number[];
  };
}

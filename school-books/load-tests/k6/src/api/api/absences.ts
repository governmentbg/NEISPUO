import http from 'k6/http';
import { Counter } from 'k6/metrics';
import { checkIsSuccessful, checkIsSuccessfulJson, json } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

const absencesViewed = new Counter('absences_viewed');

export type AbsenceType = 'Late' | 'Unexcused' | 'Excused';

export interface CreateAbsenceCommand {
  absences: Array<CreateAbsenceCommandAbsence>;
}

export interface CreateAbsenceCommandAbsence {
  classId: number;
  personId: number;
  curriculumId: number;
  type: AbsenceType;
  date: string;
  scheduleLessonId: number;
  teacherAbsenceId?: number | null;
}

export function GetAllForClassBook(schoolYear: number, instId: number, classBookId: number) {
  const url = BASE_URL + `/api/absences/${schoolYear}/${instId}/${classBookId}/getallforclassbook`;
  const res = http.get(url, REQUEST_OPTIONS);
  checkIsSuccessfulJson(res);
}

export function GetAllForStudentAndType(
  schoolYear: number,
  instId: number,
  classBookId: number,
  personId: number,
  type: AbsenceType
) {
  const url =
    BASE_URL +
    `/api/absences/${schoolYear}/${instId}/${classBookId}/getallforstudentandtype?personId=${personId}&type=${type}`;
  const res = http.get(url, REQUEST_OPTIONS);
  if (checkIsSuccessfulJson(res)) {
    absencesViewed.add(1);
  }
}

export function GetAllForWeek(
  schoolYear: number,
  instId: number,
  classBookId: number,
  year: number,
  weekNumber: number
) {
  const url =
    BASE_URL +
    `/api/absences/${schoolYear}/${instId}/${classBookId}/getallforweek?year=${year}&weekNumber=${weekNumber}`;
  const res = http.get(url, REQUEST_OPTIONS);
  if (!checkIsSuccessfulJson(res)) {
    return null;
  }

  return json(res) as {
    absenceId: number;
    personId: number;
    type: AbsenceType;
    date: string;
    scheduleLessonId: number;
  }[];
}

export function CreateAbsence(schoolYear: number, instId: number, classBookId: number, body: CreateAbsenceCommand) {
  const url = BASE_URL + `/api/absences/${schoolYear}/${instId}/${classBookId}/createabsence`;
  const res = http.post(url, JSON.stringify(body), REQUEST_OPTIONS);
  checkIsSuccessful(res);
}

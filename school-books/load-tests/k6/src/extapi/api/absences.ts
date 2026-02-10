import http from 'k6/http';
import { checkIsSuccessful } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export enum AbsenceType {
  Late = 1,
  Unexcused = 2,
  Excused = 3
}
export enum AbsenceReason {
  Health = 1,
  Family = 2,
  Other = 3
}
export interface AbsenceDO {
  absenceId?: number;
  classId: number;
  personId: number;
  curriculumId: number;
  date: string;
  type: AbsenceType;
  excusedReason?: AbsenceReason;
  excusedReasonComment?: string;
  scheduleLessonId: number;
}

export function CreateAbsence(schoolYear: number, institutionId: number, classBookId: number, body: AbsenceDO) {
  const url = BASE_URL + `/${schoolYear}/${institutionId}/classBooks/${classBookId}/absences`;
  const res = http.post(url, JSON.stringify(body), REQUEST_OPTIONS);
  checkIsSuccessful(res);
}

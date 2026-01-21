import http from 'k6/http';
import { checkIsSuccessful } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export enum GradeType {
  General = 1,
  ControlExam = 2,
  ClassExam = 3,
  Test = 4,
  Homework = 5,
  EntryLevel = 11,
  ExitLevel = 12,
  Term = 21,
  Final = 22,
  OtherSchool = 99
}
export enum SpecialNeedsGrade {
  HasDificulty = 1,
  DoingOk = 2,
  MeetsExpectations = 3
}
export enum QualitativeGrade {
  Poor = 2,
  Fair = 3,
  Good = 4,
  VeryGood = 5,
  Excellent = 6
}
export enum GradeCategory {
  Decimal = 1,
  SpecialNeeds = 2,
  Qualitative = 3
}
export interface GradeDO {
  gradeId?: number;
  classId: number;
  personId: number;
  curriculumId: number;
  date: string;
  category: GradeCategory;
  type?: GradeType;
  decimalGrade?: number;
  qualitativeGrade?: QualitativeGrade;
  specialGrade?: SpecialNeedsGrade;
  comment?: string;
  scheduleLessonId: number;
}

export function CreateGrade(schoolYear: number, institutionId: number, classBookId: number, body: GradeDO) {
  const url = BASE_URL + `/${schoolYear}/${institutionId}/classBooks/${classBookId}/grades`;
  const res = http.post(url, JSON.stringify(body), REQUEST_OPTIONS);
  checkIsSuccessful(res);
}

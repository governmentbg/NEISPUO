import http from 'k6/http';
import { checkIsSuccessful, checkIsSuccessfulJson, json } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export type GradeType =
  | 'General'
  | 'ControlExam'
  | 'ClassExam'
  | 'Test'
  | 'Homework'
  | 'EntryLevel'
  | 'ExitLevel'
  | 'Term'
  | 'Final'
  | 'OtherSchool';
export type SpecialNeedsGrade = 'HasDificulty' | 'DoingOk' | 'MeetsExpectations';
export type QualitativeGrade = 'Poor' | 'Fair' | 'Good' | 'VeryGood' | 'Excellent';
export interface CreateGradeCommandStudent {
  classId: number;
  personId: number;
  decimalGrade?: number | null;
  qualitativeGrade?: QualitativeGrade | null;
  specialGrade?: SpecialNeedsGrade | null;
  comment?: string | null;
}
export interface CreateGradeCommand {
  type: GradeType;
  date: string;
  scheduleLessonId?: number | null;
  teacherAbsenceId?: number | null;
  students: Array<CreateGradeCommandStudent>;
}

export function GetCurriculums(schoolYear: number, instId: number, classBookId: number) {
  const url = BASE_URL + `/api/grades/${schoolYear}/${instId}/${classBookId}/getcurriculums`;
  const res = http.get(url, REQUEST_OPTIONS);
  return json(res) as {
    curriculumId: number;
  }[];
}

export function GetCurriculumStudents(schoolYear: number, instId: number, classBookId: number, curriculumId: number) {
  const url =
    BASE_URL + `/api/grades/${schoolYear}/${instId}/${classBookId}/getcurriculumstudents?curriculumId=${curriculumId}`;
  const res = http.get(url, REQUEST_OPTIONS);
  return json(res) as {
    classId: number;
    personId: number;
    hasSpecialNeeds: boolean;
    isTransferred: boolean;
  }[];
}

export function GetCurriculumGrades(schoolYear: number, instId: number, classBookId: number, curriculumId: number) {
  const url =
    BASE_URL + `/api/grades/${schoolYear}/${instId}/${classBookId}/getcurriculumgrades?curriculumId=${curriculumId}`;
  const res = http.get(url, REQUEST_OPTIONS);
  return json(res) as {
    gradeId: number;
  }[];
}

export function Get(schoolYear: number, instId: number, classBookId: number, gradeId: number) {
  const url = BASE_URL + `/api/grades/${schoolYear}/${instId}/${classBookId}/get/${gradeId}`;
  const res = http.get(url, REQUEST_OPTIONS);
  checkIsSuccessfulJson(res);
}

export function CreateGrade(
  schoolYear: number,
  instId: number,
  classBookId: number,
  curriculumId: number,
  body: CreateGradeCommand
) {
  const url = BASE_URL + `/api/grades/${schoolYear}/${instId}/${classBookId}/creategrade/${curriculumId}`;
  const res = http.post(url, JSON.stringify(body), REQUEST_OPTIONS);
  checkIsSuccessful(res);
}

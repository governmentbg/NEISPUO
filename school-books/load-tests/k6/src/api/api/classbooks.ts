import { check } from 'k6';
import http from 'k6/http';
import { json } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export type ClassBookType =
  | 'Book_PG'
  | 'Book_I_III'
  | 'Book_IV'
  | 'Book_V_XII'
  | 'Book_CDO'
  | 'Book_DPLR'
  | 'Book_CSOP';

export function GetAll(schoolYear: number, instId: number) {
  const url = BASE_URL + `/api/teacher/${schoolYear}/${instId}/getallclassbooks`;
  const res = http.get(url, REQUEST_OPTIONS);
  const classBooks = json(res) as {
    classBookId: number;
  }[];
  check(res, {
    'has classbooks': () => classBooks?.length > 0
  });

  return classBooks;
}

export function Get(schoolYear: number, instId: number, classBookId: number) {
  const url = BASE_URL + `/api/classbooks/${schoolYear}/${instId}/${classBookId}/get`;
  const res = http.get(url, REQUEST_OPTIONS);
  return json(res) as {
    bookType: ClassBookType;
  };
}

export function GetStudents(schoolYear: number, instId: number, classBookId: number) {
  const url = BASE_URL + `/api/classbooks/${schoolYear}/${instId}/${classBookId}/getstudents`;
  const res = http.get(url, REQUEST_OPTIONS);
  return json(res) as {
    classId: number;
    personId: number;
    isTransferred: boolean;
  }[];
}

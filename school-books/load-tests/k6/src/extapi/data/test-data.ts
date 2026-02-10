import { SharedArray } from 'k6/data';
import { shuffle } from 'src/utils';

export enum ClassBookType {
  Book_PG = 1,
  Book_I_III = 2,
  Book_IV = 3,
  Book_V_XII = 4,
  Book_CDO = 5,
  Book_DPLR = 6,
  Book_CSOP = 7
}

export type TestData = Array<{
  schoolYear: number;
  instId: number;
  classBookId: number;
  bookType: ClassBookType;
  students: Array<{ personId: number; classId: number }>;
  scheduleLessons: Array<{ scheduleLessonId: number; curriculumId: number; date: string }>;
}>;

export type ArrayElementType<TResult> = TResult extends Array<infer T> ? T : never;
export type TestDataClassBook = ArrayElementType<TestData>;

export const bookTypeAllowsDecimalGrades = (bookType: ClassBookType) =>
  bookType == ClassBookType.Book_IV || bookType == ClassBookType.Book_V_XII || bookType == ClassBookType.Book_CSOP;

export const bookTypeAllowsQualitativeGrades = (bookType: ClassBookType) =>
  bookType == ClassBookType.Book_I_III || bookType == ClassBookType.Book_CSOP;

export const bookTypeAllowsSpecialGrades = (bookType: ClassBookType) =>
  bookType == ClassBookType.Book_I_III ||
  bookType == ClassBookType.Book_IV ||
  bookType == ClassBookType.Book_V_XII ||
  bookType == ClassBookType.Book_CSOP;

const testData: TestData = new SharedArray('some data name', function () {
  const data = JSON.parse(open('./test-data.json')) as TestData;

  // randomize the scheduleLessons arrays
  for (const cb of data) {
    shuffle(cb.scheduleLessons);
  }

  return data as [];
});

export default testData;

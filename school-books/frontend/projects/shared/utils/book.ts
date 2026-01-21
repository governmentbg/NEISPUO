import { addDays, addMonths, setISODay, setISOWeek, startOfDay } from 'date-fns';
import {
  ClassBooksService,
  ClassBooks_Get,
  ClassBooks_GetRemovedStudents
} from 'projects/sb-api-client/src/api/classBooks.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { SchoolTerm } from 'projects/sb-api-client/src/model/schoolTerm';
import { SpecialNeedsGrade } from 'projects/sb-api-client/src/model/specialNeedsGrade';
import { forkJoin, Observable, of } from 'rxjs';
import { map, switchMap, take } from 'rxjs/operators';
import { average } from './array';
import { BookTabRoutesConfig, mapBookTypeToTabs } from './book-tabs';
import { throwError } from './various';

export const UNDO_INTERVAL_IN_MINUTES = 60;

export const isGroupClassBookType = (classBookType: ClassBookType) =>
  classBookType === ClassBookType.Book_CDO || classBookType === ClassBookType.Book_DPLR;

export const classBookTypeAllowsGrades = (classBookType: ClassBookType) =>
  classBookType === ClassBookType.Book_I_III ||
  classBookType === ClassBookType.Book_IV ||
  classBookType === ClassBookType.Book_V_XII ||
  classBookType === ClassBookType.Book_CSOP;

export const roundDecimalGrade = (decimalGrade: number) => (decimalGrade < 3 ? 2 : Math.round(decimalGrade));

export const formatDecimalGradeNumberRounded = (decimalGrade: number) =>
  decimalGrade < 3 ? '2' : decimalGrade.toFixed(0);

export const formatDecimalGradeNumber = (decimalGrade: number) => decimalGrade.toFixed(2);

export const formatDecimalGradeName = (decimalGrade: number) =>
  decimalGrade < 3
    ? 'Слаб'
    : decimalGrade < 3.5
    ? 'Среден'
    : decimalGrade < 4.5
    ? 'Добър'
    : decimalGrade < 5.5
    ? 'Мн.Добър'
    : decimalGrade >= 5.5
    ? 'Отличен'
    : throwError('Invalid decimalGrade');

export const formatDecimalGradeColor = (decimalGrade: number) =>
  decimalGrade < 3
    ? 'poor'
    : decimalGrade < 3.5
    ? 'fair'
    : decimalGrade < 4.5
    ? 'good'
    : decimalGrade < 5.5
    ? 'very-good'
    : decimalGrade >= 5.5
    ? 'excellent'
    : throwError('Invalid decimalGrade');

export const formatGradeTypeName = (gradeType: GradeType) =>
  gradeType === GradeType.General
    ? 'Текуща оценка'
    : gradeType === GradeType.ControlExam
    ? 'Контролна работа'
    : gradeType === GradeType.ClassExam
    ? 'Класна работа'
    : gradeType === GradeType.Test
    ? 'Тест'
    : gradeType === GradeType.Homework
    ? 'Домашна работа'
    : gradeType === GradeType.Project
    ? 'Проект'
    : gradeType === GradeType.EntryLevel
    ? 'Входно ниво'
    : gradeType === GradeType.ExitLevel
    ? 'Изходно ниво'
    : gradeType === GradeType.Term
    ? 'Срочна'
    : gradeType === GradeType.Final
    ? 'Годишна'
    : gradeType === GradeType.OtherClass
    ? 'От друг клас'
    : gradeType === GradeType.OtherSchool
    ? 'От друго училище'
    : gradeType === GradeType.OtherClassTerm
    ? 'Срочна от друг клас'
    : gradeType === GradeType.OtherSchoolTerm
    ? 'Срочна от друго училище'
    : throwError('Invalid GradeType');

export const formatQualitativeGradeName = (qualitativeGrade: QualitativeGrade) =>
  qualitativeGrade === QualitativeGrade.Poor
    ? 'Незадоволителен'
    : qualitativeGrade === QualitativeGrade.Fair
    ? 'Среден'
    : qualitativeGrade === QualitativeGrade.Good
    ? 'Добър'
    : qualitativeGrade === QualitativeGrade.VeryGood
    ? 'Мн.Добър'
    : qualitativeGrade === QualitativeGrade.Excellent
    ? 'Отличен'
    : throwError('Invalid QualitativeGrade');

export const formatQualitativeGradeShortName = (qualitativeGrade: QualitativeGrade) =>
  qualitativeGrade === QualitativeGrade.Poor
    ? 'Нез.'
    : qualitativeGrade === QualitativeGrade.Fair
    ? 'Ср.'
    : qualitativeGrade === QualitativeGrade.Good
    ? 'Доб.'
    : qualitativeGrade === QualitativeGrade.VeryGood
    ? 'Мн.Д'
    : qualitativeGrade === QualitativeGrade.Excellent
    ? 'Отл.'
    : throwError('Invalid QualitativeGrade');

export const formatQualitativeGradeColor = (qualitativeGrade: QualitativeGrade) =>
  qualitativeGrade === QualitativeGrade.Poor
    ? 'poor'
    : qualitativeGrade === QualitativeGrade.Fair
    ? 'fair'
    : qualitativeGrade === QualitativeGrade.Good
    ? 'good'
    : qualitativeGrade === QualitativeGrade.VeryGood
    ? 'very-good'
    : qualitativeGrade === QualitativeGrade.Excellent
    ? 'excellent'
    : throwError('Invalid QualitativeGrade');

export const formatQualitativeGradeEmoticon = (qualitativeGrade: QualitativeGrade) =>
  qualitativeGrade === QualitativeGrade.Poor
    ? 'anguished'
    : qualitativeGrade === QualitativeGrade.Fair
    ? 'hushed'
    : qualitativeGrade === QualitativeGrade.Good
    ? 'neutral_face'
    : qualitativeGrade === QualitativeGrade.VeryGood
    ? 'blush'
    : qualitativeGrade === QualitativeGrade.Excellent
    ? 'smiley'
    : throwError('Invalid QualitativeGrade');

export const qualitativeGradeToNumber = (qualitativeGrade: QualitativeGrade) =>
  qualitativeGrade === QualitativeGrade.Poor
    ? 2
    : qualitativeGrade === QualitativeGrade.Fair
    ? 3
    : qualitativeGrade === QualitativeGrade.Good
    ? 4
    : qualitativeGrade === QualitativeGrade.VeryGood
    ? 5
    : qualitativeGrade === QualitativeGrade.Excellent
    ? 6
    : throwError('Invalid QualitativeGrade');

export const numberToQualitativeGrade = (qualitativeGrade: number) =>
  qualitativeGrade === 2
    ? QualitativeGrade.Poor
    : qualitativeGrade === 3
    ? QualitativeGrade.Fair
    : qualitativeGrade === 4
    ? QualitativeGrade.Good
    : qualitativeGrade === 5
    ? QualitativeGrade.VeryGood
    : qualitativeGrade === 6
    ? QualitativeGrade.Excellent
    : throwError('Invalid QualitativeGrade');

export const formatSpecialGradeName = (specialGrade: SpecialNeedsGrade) =>
  specialGrade === SpecialNeedsGrade.HasDificulty
    ? 'Среща затруднения'
    : specialGrade === SpecialNeedsGrade.DoingOk
    ? 'Справя се'
    : specialGrade === SpecialNeedsGrade.MeetsExpectations
    ? 'Постига изискванията'
    : throwError('Invalid SpecialNeedsGrade');

export const formatSpecialGradeShortName = (specialGrade: SpecialNeedsGrade) =>
  specialGrade === SpecialNeedsGrade.HasDificulty
    ? 'СЗ'
    : specialGrade === SpecialNeedsGrade.DoingOk
    ? 'СС'
    : specialGrade === SpecialNeedsGrade.MeetsExpectations
    ? 'ПИ'
    : throwError('Invalid SpecialNeedsGrade');

export const calculateAverageDecimalGrade = (decimalGrades: (number | null | undefined)[]) =>
  average(decimalGrades, (g) => (g != null ? roundDecimalGrade(g) : null));

export const calculateAverageQualitativeGrade = (qualitativeGrades: (QualitativeGrade | null | undefined)[]) => {
  const averageGrade = average(qualitativeGrades, (g) => (g != null ? qualitativeGradeToNumber(g) : null));

  return averageGrade != null ? numberToQualitativeGrade(Math.round(averageGrade)) : null;
};

// Note! This function have identical version in the backend in
// backend/SB.Domain/Grades/Entities/Grade.cs
export const gradeTypeRequiresScheduleLesson = (gradeType: GradeType | null) =>
  gradeType !== GradeType.Term &&
  gradeType !== GradeType.Final &&
  gradeType !== GradeType.OtherClass &&
  gradeType !== GradeType.OtherSchool &&
  gradeType !== GradeType.OtherClassTerm &&
  gradeType !== GradeType.OtherSchoolTerm;

// Note! the checkBook[X] functions bellow have identical
// versions in the backend in backend/SB.Domain/ClassBook/Entities/ClassBook.cs

export const checkBookAllowsModifications = (schoolYearIsFinalized: boolean, classBookIsFinalized: boolean): boolean =>
  !schoolYearIsFinalized && !classBookIsFinalized;

export const checkBookAllowsGradeModifications = (
  schoolYearIsFinalized: boolean,
  classBookIsFinalized: boolean,
  classBookIsValid: boolean,
  hasFutureEntryLock: boolean,
  pastMonthLockDay: number | null | undefined,
  now: Date,
  gradeType: GradeType,
  gradeDate: Date
): boolean =>
  !schoolYearIsFinalized &&
  !classBookIsFinalized &&
  classBookIsValid &&
  (!gradeTypeRequiresScheduleLesson(gradeType) ||
    (!checkBookHasFutureEntryLock(hasFutureEntryLock, now, gradeDate) &&
      !checkBookHasPastMonthLock(pastMonthLockDay, now, gradeDate)));

export const checkBookAllowsAttendanceAbsenceTopicModifications = (
  schoolYearIsFinalized: boolean,
  classBookIsFinalized: boolean,
  classBookIsValid: boolean,
  hasFutureEntryLock: boolean,
  pastMonthLockDay: number | null | undefined,
  now: Date,
  entryDate: Date
): boolean =>
  !schoolYearIsFinalized &&
  !classBookIsFinalized &&
  classBookIsValid &&
  !checkBookHasFutureEntryLock(hasFutureEntryLock, now, entryDate) &&
  !checkBookHasPastMonthLock(pastMonthLockDay, now, entryDate);

export const checkBookAllowsAdditionalActivityModifications = (
  schoolYearIsFinalized: boolean,
  classBookIsFinalized: boolean,
  classBookIsValid: boolean,
  hasFutureEntryLock: boolean,
  pastMonthLockDay: number | null | undefined,
  now: Date,
  year: number,
  weekNumber: number
): boolean =>
  !schoolYearIsFinalized &&
  !classBookIsFinalized &&
  classBookIsValid &&
  !checkBookHasFutureEntryLock(hasFutureEntryLock, now, setISODay(setISOWeek(new Date(year, 7, 7), weekNumber), 1)) &&
  !checkBookHasPastMonthLock(pastMonthLockDay, now, setISODay(setISOWeek(new Date(year, 7, 7), weekNumber), 7));

export const checkBookHasFutureEntryLock = (hasFutureEntryLock: boolean, now: Date, entryDate: Date): boolean =>
  hasFutureEntryLock && entryDate >= addDays(startOfDay(now), 1);

export const checkBookHasPastMonthLock = (
  pastMonthLockDay: number | null | undefined,
  now: Date,
  entryDate: Date
): boolean => pastMonthLockDay != null && entryDate < getFirstEditableMonthStartDate(pastMonthLockDay, now);

export const getFirstEditableMonthStartDate = (pastMonthLockDay: number, now: Date): Date =>
  addMonths(addDays(startOfDay(now), 1 - now.getDate()), now.getDate() >= pastMonthLockDay ? 0 : -1);

type ClassBookComponentData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
} & {
  [key: string]: unknown | Observable<unknown>;
};
type Student = { personId: number };
type StudentItem = { personId: number };
type ObservableBaseType<T> = T extends Observable<infer TData> ? TData : T;
type ObservableDictionaryBase<T> = {
  [K in keyof T]: ObservableBaseType<T[K]>;
};

interface Selector<T, V> {
  (el: T): V;
}

export function resolveWithRemovedStudents<T extends ClassBookComponentData>(
  classBooksService: ClassBooksService,
  studentsSelector: Selector<ObservableDictionaryBase<T>, Student[]>,
  studentItemsSelector: Selector<ObservableDictionaryBase<T>, StudentItem[]>,
  componentData: T
): Observable<ObservableDictionaryBase<T> & { removedStudents: ClassBooks_GetRemovedStudents }> {
  if (componentData == null) {
    throw new Error('No componentData object provided');
  }

  let result$: Observable<ObservableDictionaryBase<T>>;

  const obsDataEntries = Object.entries(componentData).filter(([k, v]) => v instanceof Observable);
  if (!obsDataEntries.length) {
    result$ = of(componentData as ObservableDictionaryBase<T>);
  } else {
    const mapedData = Object.fromEntries(
      // pipe each observable in the data object through a take(1)
      // as forkJoin completes when the underlying observables complete
      // but we are interested in the first values only
      obsDataEntries.map(([k, v]) => [k, (v as Observable<any>).pipe(take(1))])
    );

    result$ = forkJoin(mapedData)
      // merge the observable properties with the rest in the data object
      .pipe(map((obsData) => Object.assign(componentData, obsData)));
  }

  return result$.pipe(
    switchMap((result) => {
      const students = studentsSelector(result);
      const studentItems = studentItemsSelector(result);
      const studentsMap = new Map(students.map((s) => [s.personId, true]));
      const removedStudentsMap = new Map<number, boolean>();

      for (const studentItem of studentItems) {
        if (!studentsMap.has(studentItem.personId)) {
          removedStudentsMap.set(studentItem.personId, true);
        }
      }

      const removedStudentIds = [...removedStudentsMap.keys()];
      if (removedStudentIds.length > 0) {
        return classBooksService
          .getRemovedStudents({
            schoolYear: componentData.schoolYear,
            instId: componentData.instId,
            classBookId: componentData.classBookId,
            personIds: removedStudentIds
          })
          .pipe(
            map((removedStudents) => {
              return {
                ...result,
                removedStudents
              };
            })
          );
      } else {
        return of({
          ...result,
          removedStudents: []
        });
      }
    })
  );
}

export const FirstGradeBasicClassId = 1;
export const SecondGradeBasicClassId = 2;
export const ThirdGradeBasicClassId = 3;

export function getTermFromDate(classBookInfo: ClassBooks_Get, date: Date) {
  if (classBookInfo.schoolYearStartDateLimit <= date && date <= classBookInfo.firstTermEndDate) {
    return SchoolTerm.TermOne;
  } else if (classBookInfo.secondTermStartDate <= date && date <= classBookInfo.schoolYearEndDateLimit) {
    return SchoolTerm.TermTwo;
  } else {
    return null;
  }
}

export function getFirstTabRoute(classBookInfo: ClassBooks_Get) {
  return mapBookTypeToTabs(false, BookTabRoutesConfig, {
    bookType: classBookInfo.bookType,
    basicClassId: classBookInfo.basicClassId
  })[0][0] as string;
}

export type ClassBookInfoType = ClassBooks_Get & {
  checkBookAllowsGradeModifications: (gradeType: GradeType, gradeDate: Date) => boolean;
  checkBookAllowsAttendanceAbsenceTopicModifications: (entryDate: Date) => boolean;
  checkBookAllowsAdditionalActivityModifications: (year: number, weekNumber: number) => boolean;
  checkBookHasFutureEntryLock: (entryDate: Date) => boolean;
  checkBookHasPastMonthLock: (entryDate: Date) => boolean;
  firstEditableMonthStartDate: Date | null;
  bookAllowsModifications: boolean;
};

export const extendClassBookInfo = (classBookInfo: ClassBooks_Get): ClassBookInfoType => ({
  ...classBookInfo,
  checkBookAllowsGradeModifications: (gradeType: GradeType, gradeDate: Date) =>
    !classBookInfo.hasCBExtProvider &&
    checkBookAllowsGradeModifications(
      classBookInfo.schoolYearIsFinalized,
      classBookInfo.isFinalized,
      classBookInfo.isValid,
      classBookInfo.hasFutureEntryLock,
      classBookInfo.pastMonthLockDay,
      new Date(),
      gradeType,
      gradeDate
    ),
  checkBookAllowsAttendanceAbsenceTopicModifications: (entryDate: Date) =>
    !classBookInfo.hasCBExtProvider &&
    checkBookAllowsAttendanceAbsenceTopicModifications(
      classBookInfo.schoolYearIsFinalized,
      classBookInfo.isFinalized,
      classBookInfo.isValid,
      classBookInfo.hasFutureEntryLock,
      classBookInfo.pastMonthLockDay,
      new Date(),
      entryDate
    ),
  checkBookAllowsAdditionalActivityModifications: (year: number, weekNumber: number) =>
    !classBookInfo.hasCBExtProvider &&
    checkBookAllowsAdditionalActivityModifications(
      classBookInfo.schoolYearIsFinalized,
      classBookInfo.isFinalized,
      classBookInfo.isValid,
      classBookInfo.hasFutureEntryLock,
      classBookInfo.pastMonthLockDay,
      new Date(),
      year,
      weekNumber
    ),
  checkBookHasFutureEntryLock: (entryDate: Date) =>
    checkBookHasFutureEntryLock(classBookInfo.hasFutureEntryLock, new Date(), entryDate),
  checkBookHasPastMonthLock: (entryDate: Date) =>
    checkBookHasPastMonthLock(classBookInfo.pastMonthLockDay, new Date(), entryDate),
  firstEditableMonthStartDate:
    classBookInfo.pastMonthLockDay == null
      ? null
      : getFirstEditableMonthStartDate(classBookInfo.pastMonthLockDay, new Date()),
  bookAllowsModifications:
    !classBookInfo.hasCBExtProvider &&
    classBookInfo.isValid &&
    checkBookAllowsModifications(classBookInfo.schoolYearIsFinalized, classBookInfo.isFinalized)
});

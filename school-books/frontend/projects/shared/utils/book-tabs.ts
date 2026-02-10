import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { FirstGradeBasicClassId, SecondGradeBasicClassId, ThirdGradeBasicClassId } from 'projects/shared/utils/book';
import { throwError } from './various';

export type BookTabConfig<T> = {
  grades: T;
  absences: T;
  absencesDplr: T;
  attendancesDplr: T;
  attendances: T;
  topics: T;
  topicsDplr: T;
  remarks: T;
  schedule: T;
  parentMeetings: T;
  exams: T;
  sanctions: T;
  classBookTopicPlans: T;
  supports: T;
  notes: T;
  firstGradeResults: T;
  pgResults: T;
  individualWorks: T;
  gradeResults: T;
  sessions: T;
  performances: T;
  replrParticipations: T;
};

export const BookTabRoutesConfig: BookTabConfig<unknown[]> = {
  grades: ['grades'],
  absences: ['absences'],
  absencesDplr: ['absences-dplr', { type: 'Absence' }],
  attendancesDplr: ['absences-dplr', { type: 'Attendance' }],
  attendances: ['attendances'],
  topics: ['topics'],
  topicsDplr: ['topics-dplr'],
  remarks: ['remarks'],
  schedule: ['schedule'],
  parentMeetings: ['parent-meetings'],
  exams: ['exams'],
  sanctions: ['sanctions'],
  classBookTopicPlans: ['topic-plans'],
  supports: ['supports'],
  notes: ['notes'],
  firstGradeResults: ['first-grade-results'],
  pgResults: ['pg-results'],
  individualWorks: ['individual-works'],
  gradeResults: ['grade-results'],
  sessions: ['sessions'],
  performances: ['performances'],
  replrParticipations: ['replrParticipations']
};

export function mapBookTypeToTabs<T>(
  isStudent: boolean,
  bookTabConfig: BookTabConfig<T>,
  bookInfo: { bookType: ClassBookType; basicClassId?: number | null }
): T[] {
  switch (bookInfo.bookType) {
    case ClassBookType.Book_PG: // 3-5
      return [
        bookTabConfig.attendances,
        !isStudent ? bookTabConfig.topics : <never>null,
        bookTabConfig.schedule,
        bookTabConfig.parentMeetings,
        bookTabConfig.pgResults,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookTabConfig.notes
      ].filter((t) => t != null);
    case ClassBookType.Book_I_III: // 3-14
      return [
        bookTabConfig.grades,
        bookTabConfig.absences,
        bookTabConfig.topics,
        bookTabConfig.remarks,
        bookTabConfig.schedule,
        bookTabConfig.parentMeetings,
        bookTabConfig.individualWorks,
        bookTabConfig.exams,
        bookTabConfig.supports,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookInfo.basicClassId !== SecondGradeBasicClassId && bookInfo.basicClassId !== ThirdGradeBasicClassId
          ? bookTabConfig.firstGradeResults
          : <never>null,
        bookTabConfig.notes
      ].filter((t) => t != null);
    case ClassBookType.Book_IV: // 3-16
      return [
        bookTabConfig.grades,
        bookTabConfig.absences,
        bookTabConfig.topics,
        bookTabConfig.remarks,
        bookTabConfig.schedule,
        bookTabConfig.parentMeetings,
        bookTabConfig.individualWorks,
        bookTabConfig.exams,
        bookTabConfig.sanctions,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookTabConfig.supports,
        bookTabConfig.notes
      ].filter((t) => t != null);
    case ClassBookType.Book_V_XII: // 3-87
      return [
        bookTabConfig.grades,
        bookTabConfig.absences,
        bookTabConfig.topics,
        bookTabConfig.remarks,
        bookTabConfig.schedule,
        bookTabConfig.parentMeetings,
        bookTabConfig.individualWorks,
        bookTabConfig.exams,
        bookTabConfig.sanctions,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookTabConfig.supports,
        bookTabConfig.gradeResults,
        !isStudent ? bookTabConfig.sessions : <never>null,
        bookTabConfig.notes
      ].filter((t) => t != null);
    case ClassBookType.Book_CDO: // 3-63
      return [
        bookTabConfig.absences,
        !isStudent ? bookTabConfig.topics : <never>null,
        bookTabConfig.schedule,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookTabConfig.notes
      ].filter((t) => t != null);
    case ClassBookType.Book_DPLR: // 3-63.1
      return [
        bookTabConfig.absencesDplr,
        bookTabConfig.attendancesDplr,
        !isStudent ? bookTabConfig.topicsDplr : <never>null,
        bookTabConfig.schedule,
        !isStudent ? bookTabConfig.performances : <never>null,
        !isStudent ? bookTabConfig.replrParticipations : <never>null,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookTabConfig.notes
      ].filter((t) => t != null);
    case ClassBookType.Book_CSOP: // 3-62
      return [
        bookTabConfig.grades,
        bookTabConfig.absences,
        !isStudent ? bookTabConfig.topics : <never>null,
        bookTabConfig.schedule,
        bookTabConfig.parentMeetings,
        bookTabConfig.individualWorks,
        !isStudent ? bookTabConfig.classBookTopicPlans : <never>null,
        bookInfo.basicClassId === FirstGradeBasicClassId ? bookTabConfig.firstGradeResults : <never>null,
        bookTabConfig.supports,
        bookTabConfig.notes
      ].filter((t) => t != null);
    default:
      return throwError('Uknown bookType');
  }
}

export function classBookHasTab(
  isStudent: boolean,
  bookInfo: { bookType: ClassBookType; basicClassId?: number | null },
  tab: keyof BookTabConfig<never>
): boolean {
  const tabConfig = Object.fromEntries(Object.keys(BookTabRoutesConfig).map((k) => [k, k])) as BookTabConfig<string>;

  return mapBookTypeToTabs(isStudent, tabConfig, bookInfo).includes(tab);
}

import * as Absences from '../api/absences';
import * as KeyedCounter from '../tools/keyed-counter';
import { randomItem } from 'src/utils';
import testData, {
  bookTypeAllowsDecimalGrades,
  bookTypeAllowsQualitativeGrades,
  bookTypeAllowsSpecialGrades,
  TestDataClassBook
} from '../data/test-data';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  let classBook: TestDataClassBook | null = null;
  let allowsDecimalGrades: boolean;
  let allowsQualitativeGrades: boolean;
  let allowsSpecialGrades: boolean;
  for (let i = 0; i < 10; i++) {
    const cb = randomItem(testData);
    allowsDecimalGrades = bookTypeAllowsDecimalGrades(cb.bookType);
    allowsQualitativeGrades = bookTypeAllowsQualitativeGrades(cb.bookType);
    allowsSpecialGrades = bookTypeAllowsSpecialGrades(cb.bookType);
    if (allowsDecimalGrades || allowsQualitativeGrades || allowsSpecialGrades) {
      classBook = cb;
      break;
    }
  }

  if (classBook == null) {
    console.log('skipping grade creation as we couldnt find suitable class book');
    return;
  }

  const { schoolYear, instId, classBookId, students, scheduleLessons } = classBook;
  const { personId, classId } = randomItem(students);

  const key = `absence:${classId}:${personId}`;
  const next = KeyedCounter.GetNext(key);

  if (next >= scheduleLessons.length) {
    console.log('skipping absence creation as all scheduleLessons are taken');
    return;
  }

  const { scheduleLessonId, curriculumId, date } = scheduleLessons[next];

  Absences.CreateAbsence(schoolYear, instId, classBookId, {
    classId,
    personId,
    curriculumId,
    scheduleLessonId,
    date,
    type: Absences.AbsenceType.Excused,
    excusedReason: Absences.AbsenceReason.Other,
    excusedReasonComment: '$$perf_testing$$'
  });
};

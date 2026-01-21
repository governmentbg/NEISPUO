import * as Grades from '../api/grades';
import { randomItem } from 'src/utils';
import {
  default as testData,
  TestDataClassBook,
  bookTypeAllowsDecimalGrades,
  bookTypeAllowsQualitativeGrades,
  bookTypeAllowsSpecialGrades
} from '../data/test-data';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  let classBook: TestDataClassBook | null = null;
  let allowsDecimalGrades = false;
  let allowsQualitativeGrades = false;
  let allowsSpecialGrades = false;
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
  const { scheduleLessonId, curriculumId, date } = randomItem(scheduleLessons);

  let grade: {
    category: Grades.GradeCategory;
    type?: Grades.GradeType;
    decimalGrade?: number;
    qualitativeGrade?: Grades.QualitativeGrade;
    specialGrade?: Grades.SpecialNeedsGrade;
  };
  if (allowsDecimalGrades) {
    grade = { category: Grades.GradeCategory.Decimal, decimalGrade: 5 };
  } else if (allowsQualitativeGrades) {
    grade = { category: Grades.GradeCategory.Qualitative, qualitativeGrade: Grades.QualitativeGrade.Excellent };
  } else {
    grade = { category: Grades.GradeCategory.SpecialNeeds, specialGrade: Grades.SpecialNeedsGrade.HasDificulty };
  }

  Grades.CreateGrade(schoolYear, instId, classBookId, {
    classId,
    personId,
    curriculumId,
    scheduleLessonId,
    date,
    ...grade,
    type: Grades.GradeType.General,
    comment: '$$perf_testing$$'
  });
};

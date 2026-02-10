import * as ClassBooks from '../api/classbooks';
import * as Grades from '../api/grades';
import { abortTest, randomItem, shuffle } from 'src/utils';
import { instIds, schoolYear } from '../api/constants';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  const instId = randomItem(instIds);

  const classBooks = ClassBooks.GetAll(schoolYear, instId);
  if (!classBooks?.length) {
    return;
  }

  let classBookId: number | null = null;
  let curriculumId: number | null = null;
  for (const cb of shuffle(classBooks)) {
    const classBook = ClassBooks.Get(schoolYear, instId, cb.classBookId);
    const bookTypeAllowsGrades =
      classBook?.bookType === 'Book_I_III' ||
      classBook?.bookType === 'Book_IV' ||
      classBook?.bookType === 'Book_V_XII' ||
      classBook?.bookType === 'Book_CSOP';
    if (!bookTypeAllowsGrades) {
      continue;
    }

    const curriculum = Grades.GetCurriculums(schoolYear, instId, cb.classBookId);
    if (!curriculum?.length) {
      continue;
    }

    classBookId = cb.classBookId;
    curriculumId = randomItem(curriculum).curriculumId;
    break;
  }

  if (!classBookId || !curriculumId) {
    abortTest({ error: 'No class book found with grades and curriculum' });
    return;
  }

  Grades.GetCurriculumStudents(schoolYear, instId, classBookId, curriculumId);

  const grades = Grades.GetCurriculumGrades(schoolYear, instId, classBookId, curriculumId);
  const gradeId = randomItem(grades)?.gradeId;

  if (!gradeId) {
    return;
  }

  Grades.Get(schoolYear, instId, classBookId, gradeId);
};

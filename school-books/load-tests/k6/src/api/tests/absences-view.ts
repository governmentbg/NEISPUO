import * as ClassBooks from '../api/classbooks';
import * as Absences from '../api/absences';
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
  let personId: number | null = null;
  for (const cb of shuffle(classBooks)) {
    const { bookType } = ClassBooks.Get(schoolYear, instId, cb.classBookId);
    const bookTypeAllowsAbsences =
      bookType === 'Book_I_III' ||
      bookType === 'Book_IV' ||
      bookType === 'Book_V_XII' ||
      bookType === 'Book_CSOP' ||
      bookType === 'Book_CDO';
    if (!bookTypeAllowsAbsences) {
      continue;
    }

    const students = ClassBooks.GetStudents(schoolYear, instId, cb.classBookId);
    if (!students?.length) {
      continue;
    }

    classBookId = cb.classBookId;
    personId = randomItem(students).personId;
    break;
  }

  if (!classBookId || !personId) {
    abortTest({ error: 'No class book found with absences' });
    return;
  }

  Absences.GetAllForClassBook(schoolYear, instId, classBookId);
  Absences.GetAllForStudentAndType(schoolYear, instId, classBookId, personId, 'Late');
};

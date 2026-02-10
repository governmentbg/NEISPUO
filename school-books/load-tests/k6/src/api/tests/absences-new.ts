import { getISOWeeksInYear } from 'date-fns';
import { minScheduleWeekNumber, maxScheduleWeekNumber, schoolYear, instIds } from '../api/constants';
import * as Absences from '../api/absences';
import * as Schedules from '../api/schedules';
import * as ClassBooks from '../api/classbooks';
import { abortTest, randomItem, shuffle } from 'src/utils';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  const instId = randomItem(instIds);

  const classBooks = ClassBooks.GetAll(schoolYear, instId);
  if (!classBooks?.length) {
    return;
  }

  let classBookId: number | null = null;
  for (const cb of shuffle(classBooks)) {
    const classBook = ClassBooks.Get(schoolYear, instId, cb.classBookId);
    const bookTypeAllowsAbsences =
      classBook?.bookType == 'Book_I_III' ||
      classBook?.bookType == 'Book_IV' ||
      classBook?.bookType == 'Book_V_XII' ||
      classBook?.bookType == 'Book_CSOP' ||
      classBook?.bookType == 'Book_CDO';
    if (!bookTypeAllowsAbsences) {
      continue;
    }

    classBookId = cb.classBookId;
    break;
  }

  if (!classBookId) {
    abortTest({ error: 'No class book found with absences' });
    return;
  }

  let students = ClassBooks.GetStudents(schoolYear, instId, classBookId);
  if (!students?.length) {
    return;
  }

  students = students.filter((s) => !s.isTransferred);

  const isoWeeks = [
    ...Array.from({
      length: getISOWeeksInYear(new Date(schoolYear, 1, 1)) - minScheduleWeekNumber + 1
    }).map((_, i) => [minScheduleWeekNumber + i, schoolYear]),
    ...Array.from({
      length: maxScheduleWeekNumber
    }).map((_, i) => [i + 1, schoolYear + 1])
  ];

  const [weekNumber, weekYear] = randomItem(isoWeeks);

  const absences = Absences.GetAllForWeek(schoolYear, instId, classBookId, weekYear, weekNumber);
  if (!absences) {
    return;
  }

  const schedule = Schedules.GetClassBookScheduleForWeek(schoolYear, instId, classBookId, weekYear, weekNumber);
  if (!schedule?.hours?.length) {
    return;
  }

  for (let i = 0; i <= 10; i++) {
    const hour = randomItem(schedule.hours);
    const student = randomItem(students);
    if (
      !absences.find(
        ({ scheduleLessonId, personId }) => hour.scheduleLessonId == scheduleLessonId && student.personId == personId
      )
    ) {
      Absences.CreateAbsence(schoolYear, instId, classBookId, {
        absences: [
          {
            classId: student.classId,
            personId: student.personId,
            curriculumId: hour.curriculumId,
            type: 'Unexcused',
            date: hour.date,
            scheduleLessonId: hour.scheduleLessonId
          }
        ]
      });
      break;
    }
  }
};

import { formatISO, setISODay, setISOWeek } from 'date-fns';
import * as ClassBooks from '../api/classbooks';
import { instIds, minScheduleWeekNumber, schoolYear } from '../api/constants';
import * as Grades from '../api/grades';
import * as Schedules from '../api/schedules';
import { abortTest, randomItem, shuffle } from 'src/utils';

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
    const bookTypeAllowsDecimalGrades =
      classBook?.bookType == 'Book_IV' || classBook?.bookType == 'Book_V_XII' || classBook?.bookType == 'Book_CSOP';
    if (!bookTypeAllowsDecimalGrades) {
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
    abortTest({ error: 'No class book found with decimal grades' });
    return;
  }

  let students = Grades.GetCurriculumStudents(schoolYear, instId, classBookId, curriculumId);
  if (!students?.length) {
    return;
  }

  students = students.filter((s) => !s.isTransferred);

  const weekDate = setISOWeek(new Date(schoolYear, 7, 7), minScheduleWeekNumber);
  for (let i = 1; i <= 7; i++) {
    const date = setISODay(weekDate, i);
    const dateSrt = formatISO(date, { representation: 'date' });
    const scheduleLessons = Schedules.GetScheduleLessons(schoolYear, instId, classBookId, curriculumId, dateSrt);

    if (scheduleLessons?.length) {
      const scheduleLesson = randomItem(scheduleLessons);
      Grades.CreateGrade(schoolYear, instId, classBookId, curriculumId, {
        type: 'General',
        date: dateSrt,
        scheduleLessonId: scheduleLesson.scheduleLessonId,
        students: shuffle(students)
          .slice(0, Math.min(5, students.length))
          .map((s) => ({
            classId: s.classId,
            personId: s.personId,
            decimalGrade: 3.5,
            comment: '$$perf_testing$$'
          }))
      });
      break;
    }
  }
};

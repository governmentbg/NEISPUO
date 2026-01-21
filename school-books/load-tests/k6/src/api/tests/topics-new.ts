import { getISOWeeksInYear } from 'date-fns';
import { minScheduleWeekNumber, maxScheduleWeekNumber, schoolYear, instIds } from '../api/constants';
import * as Topics from '../api/topics';
import * as Schedules from '../api/schedules';
import * as ClassBooks from '../api/classbooks';
import { randomItem } from 'src/utils';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  const instId = randomItem(instIds);

  const classBooks = ClassBooks.GetAll(schoolYear, instId);
  if (!classBooks?.length) {
    return;
  }

  const { classBookId } = randomItem(classBooks);

  const isoWeeks = [
    ...Array.from({
      length: getISOWeeksInYear(new Date(schoolYear, 1, 1)) - minScheduleWeekNumber + 1
    }).map((_, i) => [minScheduleWeekNumber + i, schoolYear]),
    ...Array.from({
      length: maxScheduleWeekNumber
    }).map((_, i) => [i + 1, schoolYear + 1])
  ];

  const [weekNumber, weekYear] = randomItem(isoWeeks);

  const topics = Topics.GetAllForWeek(schoolYear, instId, classBookId, weekYear, weekNumber);
  if (!topics) {
    return;
  }

  const schedule = Schedules.GetClassBookScheduleForWeek(schoolYear, instId, classBookId, weekYear, weekNumber);
  if (!schedule?.hours?.length) {
    return;
  }

  for (let i = 0; i <= 10; i++) {
    const hour = randomItem(schedule.hours);
    if (!topics.find(({ scheduleLessonId }) => hour.scheduleLessonId == scheduleLessonId)) {
      Topics.CreateTopic(schoolYear, instId, classBookId, {
        topics: [
          {
            title: '$$perf_testing$$',
            date: hour.date,
            scheduleLessonId: hour.scheduleLessonId
          }
        ]
      });
      break;
    }
  }
};

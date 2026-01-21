import * as ClassBooks from '../api/classbooks';
import * as Topics from '../api/topics';
import * as Schedules from '../api/schedules';
import { instIds, minScheduleWeekNumber, schoolYear } from '../api/constants';
import { randomItem } from 'src/utils';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  const instId = randomItem(instIds);

  const classBooks = ClassBooks.GetAll(schoolYear, instId);
  if (!classBooks?.length) {
    return;
  }

  const classBookId = randomItem(classBooks).classBookId;

  ClassBooks.Get(schoolYear, instId, classBookId);

  Schedules.GetClassBookScheduleForWeek(schoolYear, instId, classBookId, schoolYear, minScheduleWeekNumber);

  Topics.GetAllForWeek(schoolYear, instId, classBookId, schoolYear, minScheduleWeekNumber);
};

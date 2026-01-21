import * as Topics from '../api/topics';
import * as KeyedCounter from '../tools/keyed-counter';
import { randomItem } from 'src/utils';
import testData from '../data/test-data';

export { setup, teardown, handleSummary, SingleRunOptions as options } from 'src/common';

export default () => {
  const { schoolYear, instId, classBookId, scheduleLessons } = randomItem(testData);

  const key = `topic:${classBookId}`;
  const next = KeyedCounter.GetNext(key);

  if (next >= scheduleLessons.length) {
    console.log('skipping topic creation as all scheduleLessons are taken');
    return;
  }

  const { scheduleLessonId, date } = scheduleLessons[next];

  Topics.CreateTopic(schoolYear, instId, classBookId, {
    date,
    scheduleLessonId,
    title: '$$perf_testing$$'
  });
};

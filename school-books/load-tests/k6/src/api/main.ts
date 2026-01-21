import { Options } from 'k6/options';
import { createScenario } from 'src/utils';
export { default as testGradesView } from './tests/grades-view';
export { default as testGradesNew } from './tests/grades-new';
export { default as testAbsencesView } from './tests/absences-view';
export { default as testAbsencesNew } from './tests/absences-new';
export { default as testTopicsView } from './tests/topics-view';
export { default as testTopicsNew } from './tests/topics-new';

export { setup, teardown, handleSummary } from 'src/common';

const warmupDuration = '30s';
const stageDuration = '5m';
const scenario = (peakRPS: number) => createScenario(peakRPS, warmupDuration, stageDuration);

export let options: Options = {
  insecureSkipTLSVerify: true,
  thresholds: {
    fatalErrors: [{ threshold: 'count<1', abortOnFail: true }],
    checks: ['rate>0.9']
  },
  summaryTrendStats: ['avg', 'med', 'p(95)', 'p(99)'],
  scenarios: {
    // test_grades: {
    //   exec: 'testGradesView',
    //   ...scenario(10)
    // },
    test_grades_new: {
      exec: 'testGradesNew',
      ...scenario(100)
    },
    // test_absences: {
    //   exec: 'testAbsencesView',
    //   ...scenario(10)
    // },
    test_absences_new: {
      exec: 'testAbsencesNew',
      ...scenario(100)
    },
    // test_topics: {
    //   exec: 'testTopicsView',
    //   ...scenario(10)
    // },
    test_topics_new: {
      exec: 'testTopicsNew',
      ...scenario(100)
    }
  }
};

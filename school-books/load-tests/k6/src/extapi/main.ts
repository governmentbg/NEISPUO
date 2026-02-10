import { Options } from 'k6/options';
import { createScenario } from 'src/utils';
export { default as testGradesNew } from './tests/grades-new';
export { default as testAbsencesNew } from './tests/absences-new';
export { default as testTopicsNew } from './tests/topics-new';

export { setup, teardown, handleSummary } from 'src/common';

const warmupDuration = '30s';
const stageDuration = '5m';
const scenario = (peakRPS: number) => createScenario(peakRPS, warmupDuration, stageDuration);

// 1 000 000 ученика
// 50 000    класа

// 6 теми на ден на клас
// 1 отсътвие на ден на ученик
// 1 оценка на ден на ученик
// 1 присъствие на ден на дете в ДГ/ПГ

// 500 rps grades
// 450 rps absences
// 50 rps attendances
// 100 rps topics

export let options: Options = {
  insecureSkipTLSVerify: true,
  thresholds: {
    fatalErrors: [{ threshold: 'count<1', abortOnFail: true }],
    checks: ['rate>0.9']
  },
  summaryTrendStats: ['avg', 'med', 'p(95)', 'p(99)'],
  scenarios: {
    test_grades_new: {
      exec: 'testGradesNew',
      ...scenario(250)
    },
    test_absences_new: {
      exec: 'testAbsencesNew',
      ...scenario(225)
    },
    test_topics_new: {
      exec: 'testTopicsNew',
      ...scenario(50)
    }
  }
};

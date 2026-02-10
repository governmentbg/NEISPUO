import { check } from 'k6';
import { RefinedResponse, ResponseType } from 'k6/http';
import { Counter } from 'k6/metrics';
import { RampingArrivalRateScenario } from 'k6/options';

export const fatalErrorsCounter = new Counter('fatalErrors');

export function abortTest(tags?: { [name: string]: string }) {
  fatalErrorsCounter.add(1, tags);
}

export function json(res: RefinedResponse<ResponseType>) {
  const successful = checkIsSuccessfulJson(res);

  if (successful) {
    return res.json();
  }

  return null;
}

export function checkIsSuccessfulJson(res: RefinedResponse<ResponseType>) {
  const successful = check(res, {
    'is successfull status code': (r) => r.status >= 200 && r.status < 300,
    'is json content-type': (r) => r.headers['Content-Type']?.startsWith('application/json')
  });
  if (!successful) {
    console.log(res.request.url);
  }
  return successful;
}

export function checkIsSuccessful(res: RefinedResponse<ResponseType>) {
  const successful = check(res, {
    'is successfull status code': (r) => r.status >= 200 && r.status < 300
  });
  if (!successful) {
    console.log(res.request.url);
    console.log(res.request.body);
  }
  return successful;
}

export function createScenario(
  peakRPS: number,
  warmupDuration: string,
  stageDuration: string
): RampingArrivalRateScenario {
  return {
    executor: 'ramping-arrival-rate',
    startRate: 10,
    timeUnit: '1s',
    preAllocatedVUs: peakRPS * 4,
    maxVUs: peakRPS * 6,
    stages: [
      { target: 10, duration: warmupDuration }, // warmup
      { target: peakRPS, duration: stageDuration },
      { target: peakRPS, duration: stageDuration },
      { target: 0, duration: stageDuration }
    ]
  };
}

export function randomNumber(min: number, max: number) {
  min = Math.ceil(min);
  max = Math.floor(max);

  //The maximum is exclusive and the minimum is inclusive
  return Math.floor(Math.random() * (max - min) + min);
}

export function randomItem<T>(array: Array<T>): T {
  return array[randomNumber(0, array.length)];
}

export function shuffle<T>(array: Array<T>): Array<T> {
  return array
    .map((value) => ({ value, sort: Math.random() }))
    .sort((a, b) => a.sort - b.sort)
    .map(({ value }) => value);
}

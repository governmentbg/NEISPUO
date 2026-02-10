import { Options } from 'k6/options';
/* @ts-ignore */
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';

export function setup() {}

export function teardown(_data: any) {}

export function handleSummary(data: any) {
  return {
    stdout: textSummary(data, { indent: ' ', enableColors: true })
  };
}

export const SingleRunOptions: Options = {
  insecureSkipTLSVerify: true,
  vus: 1,
  iterations: 1
};

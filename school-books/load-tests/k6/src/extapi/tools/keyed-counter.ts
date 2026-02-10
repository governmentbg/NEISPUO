import http from 'k6/http';
import { abortTest } from 'src/utils';
import { KEYED_COUNTER_URL } from '../api/constants';

export function GetNext(key: string): number {
  const url = `${KEYED_COUNTER_URL}/${key}`;

  for (let i = 0; i < 10; i++) {
    const res = http.get(url, {
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json'
      }
    });

    if (res.status == 200) {
      return res.json() as number;
    }
  }

  abortTest({ error: 'Could not connect to keyed-counter service. Did you start it?' });
  throw new Error('Could not connect to keyed-counter service. Did you start it?');
}

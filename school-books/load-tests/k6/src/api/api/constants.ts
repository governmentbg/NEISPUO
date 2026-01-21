import { Params } from 'k6/http';

export const BASE_URL = `${__ENV.TARGET}`;

export const REQUEST_OPTIONS: Params = {
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
    Authorization: `Bearer ${__ENV.TOKEN}`
  }
};

export const schoolYear = 2022;

export const instIds = [300125];

export const minScheduleWeekNumber = 38;

export const maxScheduleWeekNumber = 21;

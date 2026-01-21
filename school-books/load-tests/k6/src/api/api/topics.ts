import http from 'k6/http';
import { checkIsSuccessful, checkIsSuccessfulJson, json } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export interface CreateTopicsCommand {
  topics: Array<CreateTopicsCommandTopic>;
}

export interface CreateTopicsCommandTopic {
  title: string;
  date: string;
  scheduleLessonId: number;
}

export function GetAllForWeek(
  schoolYear: number,
  instId: number,
  classBookId: number,
  year: number,
  weekNumber: number
) {
  const url =
    BASE_URL + `/api/topics/${schoolYear}/${instId}/${classBookId}/getallforweek?year=${year}&weekNumber=${weekNumber}`;
  const res = http.get(url, REQUEST_OPTIONS);
  if (!checkIsSuccessfulJson(res)) {
    return null;
  }

  return json(res) as {
    topicId: number;
    title: string;
    date: string;
    scheduleLessonId: number;
  }[];
}

export function CreateTopic(schoolYear: number, instId: number, classBookId: number, body: CreateTopicsCommand) {
  const url = BASE_URL + `/api/topics/${schoolYear}/${instId}/${classBookId}/createtopics`;
  const res = http.post(url, JSON.stringify(body), REQUEST_OPTIONS);
  checkIsSuccessful(res);
}

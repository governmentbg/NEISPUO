import http from 'k6/http';
import { checkIsSuccessful } from 'src/utils';
import { BASE_URL, REQUEST_OPTIONS } from './constants';

export interface TopicDO {
  topicId?: number;
  date: string;
  title: string;
  scheduleLessonId: number;
}

export function CreateTopic(schoolYear: number, institutionId: number, classBookId: number, body: TopicDO) {
  const url = BASE_URL + `/${schoolYear}/${institutionId}/classBooks/${classBookId}/topics`;
  const res = http.post(url, JSON.stringify(body), REQUEST_OPTIONS);
  checkIsSuccessful(res);
}

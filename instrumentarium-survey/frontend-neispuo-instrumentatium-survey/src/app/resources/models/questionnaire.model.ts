import { Question } from "./question.model";

export class Questionnaire {
  id: number;
  name: string;
  questions: Question[]
}
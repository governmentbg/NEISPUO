import { Question } from "./question.model";
import { Questionnaire } from "./questionnaire.model";

export interface SubmittedQuestionnaire {
  user_id: number;
  questionnaire: Questionnaire;
  submitted_questionnaire_id: number;
  answeredQuestions: Question[];
}

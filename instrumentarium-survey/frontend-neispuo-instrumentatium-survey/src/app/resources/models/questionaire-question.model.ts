import { Campaign } from './campaign.model';
import { Questionnaire } from './questionnaire.model';

export class QuestionaireQuestion {
  id: number;
  state: number;
  userId: number;
  totalScore: number;
  submittedQuestionaireObject: string;
  campaignsId: Campaign;
  questionaireId: Questionnaire;
}
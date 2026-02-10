export interface Question {
  question_id: number;
  question_orderNumber: number;
  question_title: string;
  question_type: number;
  question_questionAreaId: number;
  question_criteriaId: number;
  question_indicatorId: number;
  question_subindicatorId: number;
  questionArea_id: number;
  questionArea_orderNumber: number;
  questionArea_title: string;
  questionArea_description: null;
  criteria_id: number;
  criteria_orderNumber: number;
  criteria_name: string;
  criteria_questionAreaID: number;
  indicator_id: number;
  indicator_orderNumber: number;
  indicator_name: string;
  indicator_weight: number;
  indicator_criteriaId: number;
  subindicator_id: number;
  subindicator_orderNumber: number;
  subindicator_name: string;
  subindicator_weight: number;
  subindicator_indicatorId: number;
  choices: Choice[];
}

interface Choice {
  choices_id: number;
  choices_title: string;
  choices_description: string;
  choices_value: number | string;
  choices_weight: number;
  choices_questionId: number;
  marked?: boolean; // for demo
}

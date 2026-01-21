export class SubmitChoice {
  choiceId: number;
  value: number;
  weight: number;
  indicatorId: number;
  questionId: number;

  constructor(
    choiceId: number,
    value: number,
    weight: number,
    indicatorId: number,
    questionId: number
  ) {
    this.choiceId = choiceId;
    this.value = value;
    this.weight = weight;
    this.indicatorId = indicatorId;
    this.questionId = questionId;
  }
}
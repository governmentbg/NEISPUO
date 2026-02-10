import { Choice } from './choice.model';

export class Question {
  id: number;
  orderNumber: number;
  title: string;
  type: number;
  questionAreaId: number;
  criteriaId: number;
  indicatorId: number;
  subindicatorId: number;
  indicatorName: string;
  indicatorOrderNumber: number;
  indicatorWeight: number;
  subindicatorName: string;
  subindicatorOrderNumber: number;
  subindicatorWeight: number;
  criteriaOrderNumber: number;
  criteriaName: string;
  questionAreaTitle: string[];
  choices: Choice[];
}
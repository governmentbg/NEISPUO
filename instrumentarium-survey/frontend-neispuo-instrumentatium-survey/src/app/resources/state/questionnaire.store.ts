import { Injectable } from '@angular/core';
import { EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { Questionnaire } from '../models/questionnaire.model';

export interface QuestionnaireState extends EntityState<Questionnaire[]> {}

@Injectable({
    providedIn: 'root'
})
@StoreConfig({ name: 'questionnaire' })
export class QuestionnaireStore extends EntityStore<QuestionnaireState, Questionnaire[]> {
  constructor() {
    super();
  }
}
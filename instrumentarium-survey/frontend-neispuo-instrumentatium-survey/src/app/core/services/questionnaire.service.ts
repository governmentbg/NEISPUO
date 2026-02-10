import { Injectable } from '@angular/core';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireService {
  constructor(private readonly apiService: ApiService) {}

  // Get DB questionnaires
  getQuestionnaires(): Observable<any> {
    return this.apiService.get(`/v1/questionaire`);
  }

  // Get questions from campaign
  getQuestionnaireAndQuestions(campaignId: number, submittedQuestionnaireId?: number) {
    return submittedQuestionnaireId
      ? this.apiService.get(`/v1/submitted-questionaire/whole-questionaire/${campaignId}?id=${submittedQuestionnaireId}`)
      : this.apiService.get(`/v1/submitted-questionaire/whole-questionaire/${campaignId}`);
  }

  // Get user campaigns
  getUserFilledQuestionnaires(userID: number, campaignID: number, questionnaireID: number): Observable<any> {
    return this.apiService.get(
      `/v1/submitted-questionaire/?s={"userId": ${userID}, "campaignsId.id": ${campaignID}, "questionaireId.id": ${questionnaireID}}`
    );
  }

  // Submit questionnaire with all selected choices
  submitQuestionnaire(data, id: number): Observable<any> {
    return this.apiService.patch(`/v1/submitted-questionaire/${id}`, data);
  }

  getSubmittedQuestionnaires(queryParams?: RequestQueryBuilder['queryObject']): Observable<any> {
    return this.apiService.get(`/v1/submitted-questionaire`, queryParams);
  }
}

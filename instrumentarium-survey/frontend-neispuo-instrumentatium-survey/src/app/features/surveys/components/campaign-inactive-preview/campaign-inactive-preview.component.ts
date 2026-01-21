import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TEXT_CONTENT_CONSTANTS } from '@shared/constants/text-content.constants';
import { Subscription } from 'rxjs';
import { CampaignService } from 'src/app/core/services/campaign.service';
import { QuestionnaireService } from 'src/app/core/services/questionnaire.service';
import { Campaign } from 'src/app/resources/models/campaign.model';

@Component({
  selector: 'app-campaign-inactive-preview',
  templateUrl: './campaign-inactive-preview.component.html',
  styleUrls: ['./campaign-inactive-preview.component.scss']
})
export class CampaignInactivePreviewComponent implements OnInit {
  score: any;
  totalUsersSubmitted: number;
  totalUsersAssignedTo: number;
  averageScore: number = 0;

  questionnaires = {};

  campaign: Campaign;
  subscription: Subscription;

  content = TEXT_CONTENT_CONSTANTS.CAMPAIGN_INACTIVE_PREVIEW;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly campaignService: CampaignService,
    private readonly questionnaireService: QuestionnaireService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    this.campaignService.getCampaignById(id).subscribe((campaign: Campaign) => {
      this.campaign = campaign;
    });

    this.getQuestionnaires(id);
  }

  private getCampaignScore(campaignId) {
    this.campaignService.getAggregatedResults(campaignId).subscribe((score) => {
      this.score = score;
      this.splitIndicators();
      this.calculateAverageScore();
    });
  }

  private splitIndicators() {
    this.score.scoreByIndicators.forEach((el) => {
      this.questionnaires[el.questionaireId.name].push(el);
    });
  }

  private getQuestionnaires(id) {
    this.questionnaireService.getQuestionnaires().subscribe((questionaires) => {
      questionaires.forEach((questionaire) => {
        this.questionnaires[questionaire.name] = [];
      });
      this.getCampaignScore(id);
      this.numberOfPeopleWhoFilledInTheCampaign(id);
    });
  }

  private numberOfPeopleWhoFilledInTheCampaign(campaignId) {
    this.campaignService.getNumberOfUsersWhoFilledInTheCampaign(+campaignId).subscribe((data: any) => {
      this.totalUsersSubmitted = data.totalUsersSubmitted;
      this.totalUsersAssignedTo = data.totalUsersAssignedTo;
    });
  }

  private calculateAverageScore() {
    this.score.scoreByIndicators.forEach((indicator) => {
      this.averageScore += indicator.totalScore;
    });
    this.averageScore = Math.round((this.averageScore / this.score.scoreByIndicators.length) * 100) / 100;
  }
}

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SurveysRoutingModule } from './surveys-routing.module';
import { SharedModule } from '@shared/shared.module';
import { CampaignListComponent } from './components/campaign-list/campaign-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslatePipe } from '@shared/modules/pipes/translate.pipe';
import { MessageService } from 'primeng/api';
import { NewCampaignComponent } from './components/new-campaign/new-campaign.component';
import { QuestionnaireComponent } from './components/questionnaire/questionnaire.component';
import { CampaignActivePreviewComponent } from './components/campaign-active-preview/campaign-active-preview.component';
import { CampaignInactivePreviewComponent } from './components/campaign-inactive-preview/campaign-inactive-preview.component';
import { FilledQuestionnairesListComponent } from './components/filled-questionnaires-list/filled-questionnaires-list.component';

@NgModule({
  declarations: [
    CampaignListComponent,
    TranslatePipe,
    NewCampaignComponent,
    QuestionnaireComponent,
    CampaignActivePreviewComponent,
    CampaignInactivePreviewComponent,
    FilledQuestionnairesListComponent
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, SurveysRoutingModule, SharedModule],
  providers: [TranslatePipe, MessageService]
})
export class SurveysModule {}

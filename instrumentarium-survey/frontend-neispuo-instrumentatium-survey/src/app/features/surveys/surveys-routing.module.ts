import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@authentication/guards/auth.guard';
import { RoleGuard } from '@authentication/guards/role.guard';
import { RoleEnum } from '@authentication/models/role.enum';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';
import { PortalLayoutPage } from '@shared/modules/portal/pages/portal-layout/portal-layout.page';
import { CampaignActivePreviewComponent } from './components/campaign-active-preview/campaign-active-preview.component';
import { CampaignInactivePreviewComponent } from './components/campaign-inactive-preview/campaign-inactive-preview.component';
import { CampaignListComponent } from './components/campaign-list/campaign-list.component';
import { FilledQuestionnairesListComponent } from './components/filled-questionnaires-list/filled-questionnaires-list.component';
import { NewCampaignComponent } from './components/new-campaign/new-campaign.component';
import { QuestionnaireComponent } from './components/questionnaire/questionnaire.component';

const routes: Routes = [
  {
    path: '',
    component: PortalLayoutPage,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: CampaignListComponent
      },
      {
        path: `${ROUTING_CONSTANTS.NEW}/${ROUTING_CONSTANTS.CAMPAIGN}`,
        component: NewCampaignComponent,
        data: {
          roles: [RoleEnum.INSTITUTION]
        },
        canActivate: [RoleGuard]
      },
      {
        path: `${ROUTING_CONSTANTS.CAMPAIGN}/:campaignId/${ROUTING_CONSTANTS.QUESTIONNAIRE}`,
        component: QuestionnaireComponent,
        data: {
          roles: [
            RoleEnum.MON_ADMIN,
            RoleEnum.MON_EXPERT,
            RoleEnum.MON_OBGUM,
            RoleEnum.MON_OBGUM_FINANCES,
            RoleEnum.MON_CHRAO,
            RoleEnum.MON_USER_ADMIN,
            RoleEnum.NIO,
            RoleEnum.INSTITUTION,
            RoleEnum.TEACHER,
            RoleEnum.STUDENT,
            RoleEnum.PARENT
          ]
        },
        canActivate: [RoleGuard]
      },
      {
        path: `${ROUTING_CONSTANTS.CAMPAIGN}/:campaignId/${ROUTING_CONSTANTS.QUESTIONNAIRE}/:submittedQuestionnaireId`,
        component: QuestionnaireComponent,
        data: {
          roles: [
            RoleEnum.MON_ADMIN,
            RoleEnum.MON_EXPERT,
            RoleEnum.MON_OBGUM,
            RoleEnum.MON_OBGUM_FINANCES,
            RoleEnum.MON_CHRAO,
            RoleEnum.MON_USER_ADMIN,
            RoleEnum.NIO
          ]
        },
        canActivate: [RoleGuard] 
      },
      {
        path: `${ROUTING_CONSTANTS.ACTIVE}/${ROUTING_CONSTANTS.CAMPAIGN}/:id/${ROUTING_CONSTANTS.PREVIEW}`,
        component: CampaignActivePreviewComponent,
        data: {
          roles: [RoleEnum.INSTITUTION]
        },
        canActivate: [RoleGuard]
      },
      {
        path: `${ROUTING_CONSTANTS.INACTIVE}/${ROUTING_CONSTANTS.CAMPAIGN}/:id/${ROUTING_CONSTANTS.PREVIEW}`,
        component: CampaignInactivePreviewComponent,
        data: {
          roles: [RoleEnum.INSTITUTION]
        },
        canActivate: [RoleGuard]
      },
      {
        path: `${ROUTING_CONSTANTS.FILLED}/${ROUTING_CONSTANTS.QUESTIONNAIRES}/:id`,
        component: FilledQuestionnairesListComponent,
        data: {
          roles: [
            RoleEnum.MON_ADMIN,
            RoleEnum.MON_EXPERT,
            RoleEnum.MON_OBGUM,
            RoleEnum.MON_OBGUM_FINANCES,
            RoleEnum.MON_CHRAO,
            RoleEnum.MON_USER_ADMIN,
            RoleEnum.NIO
          ]
        },
        canActivate: [RoleGuard] 
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: ''
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SurveysRoutingModule {}

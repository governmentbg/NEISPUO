import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import moment from 'moment';
import { map } from 'rxjs/operators';
import { TEXT_CONTENT_CONSTANTS } from '@shared/constants/text-content.constants';
import { CampaignService } from 'src/app/core/services/campaign.service';
import { QuestionnaireService } from 'src/app/core/services/questionnaire.service';
import { CampaignType } from 'src/app/resources/enums/campaign-type.enum';
import { Campaign } from 'src/app/resources/models/campaign.model';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { SelectedRole } from '@authentication/auth-state-manager/interfaces/selected-role.interface';
import { QuestionaireQuestion } from 'src/app/resources/models/questionaire-question.model';

@Component({
  selector: 'app-campaign-active-preview',
  templateUrl: './campaign-active-preview.component.html',
  styleUrls: ['./campaign-active-preview.component.scss']
})
export class CampaignActivePreviewComponent implements OnInit {
  id: string;
  isDialogDisplayed: boolean = false;

  startDate: Date;
  todaysDate: Date = new Date();
  form: FormGroup;
  campaign: Campaign;
  campaignType = CampaignType;
  showSpinner = true;
  totalUsersSubmitted: number;
  totalUsersAssignedTo: number;

  numberUserFilledQuestionnaires: number;

  questionnaire;

  content = TEXT_CONTENT_CONSTANTS.CAMPAIGN_ACTIVE_PREVIEW;
  isInstitution$ = this.authQuery.isInstitution$;
  isMon$ = this.authQuery.isMon$;

  constructor(
    private readonly router: Router,
    private readonly authQuery: AuthQuery,
    private readonly route: ActivatedRoute,
    private readonly formBuilder: FormBuilder,
    private readonly campaignService: CampaignService,
    private readonly questionnaireService: QuestionnaireService,
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');

    this.getCampaign();

    this.getCampaignQuestionnaires(+this.id);

    this.numberOfPeopleWhoFilledInTheCampaign();
  }

  // Events
  onSave() {
    if (typeof this.form.value.endDate === 'string') {
      let splitDate = this.form.value.endDate.split('/');
      this.form.value.endDate = new Date(splitDate[2], splitDate[1]-1, splitDate[0]);
    }

    let updateCampaign: Campaign = new Campaign(this.form.value.name, this.form.value.startDate, this.form.value.endDate);
    updateCampaign.updatedAt = new Date();
    delete updateCampaign.startDate;

    this.campaignService.editCampaign(this.campaign.id, updateCampaign)
      .pipe(
        map(() => {
          this.getCampaign();
        })
      ).subscribe();

    this.isDialogDisplayed = false;
  }

  goToQuestionnaire() {
    this.router.navigateByUrl(`survey/campaign/${this.campaign.id}/questionnaire`);
  }


  private numberOfPeopleWhoFilledInTheCampaign() {
    this.campaignService.getNumberOfUsersWhoFilledInTheCampaign(+this.id).subscribe((data: any) => {
      this.totalUsersSubmitted = data.totalUsersSubmitted;
      this.totalUsersAssignedTo = data.totalUsersAssignedTo;
    });
  }

  private getCampaignQuestionnaires(campaignID: number) {
    this.questionnaireService.getQuestionnaireAndQuestions(campaignID).subscribe((questionnaire: QuestionaireQuestion) => {
      this.questionnaire = questionnaire;
      this.showSpinner = false;
    });
  }

  private getCampaign() {
    this.campaignService.getCampaignById(this.id).subscribe((campaign: Campaign) => {
      this.campaign = campaign;
      this.startDate = new Date(this.campaign.startDate);

      this.initForm();
    });
  }

  private initForm() {
    this.form = this.formBuilder.group({
      name: [this.campaign.name, [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-ZА-я ][a-zA-ZА-я0-9 ]*')]],
      startDate: [moment(this.campaign.startDate).format('DD/MM/YYYY'), [Validators.required]],
      endDate: [moment(this.campaign.endDate).format('DD/MM/YYYY'), [Validators.required]]
    });
  }
}

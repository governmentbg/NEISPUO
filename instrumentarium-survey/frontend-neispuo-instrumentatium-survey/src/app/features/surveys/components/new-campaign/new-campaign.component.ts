import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TEXT_CONTENT_CONSTANTS } from '@shared/constants/text-content.constants';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { CampaignService } from 'src/app/core/services/campaign.service';
import { CampaignType } from 'src/app/resources/enums/campaign-type.enum';
import { CampaignStore } from 'src/app/resources/state/campaign.store';

@Component({
  selector: 'app-new-campaign',
  templateUrl: './new-campaign.component.html',
  styleUrls: ['./new-campaign.component.scss'],
  providers: [CampaignService]
})
export class NewCampaignComponent implements OnInit {
  startDate: Date;
  endDate: Date;
  todaysDate: Date = new Date();

  detailsForm: FormGroup;
  campaignStore: CampaignStore;

  content = TEXT_CONTENT_CONSTANTS.CAMPAIGN_DETAILS;

  type = [
    {
      name: this.content.SELF_EVALUATE,
      value: CampaignType.SELFEVALUATE
    }
  ];

  constructor(
    campaignStore: CampaignStore,
    private readonly formBuilder: FormBuilder,
    private readonly campaignService: CampaignService,
    private ref: DynamicDialogRef
  ) {
    this.campaignStore = campaignStore;
  }

  ngOnInit(): void {
    this.initForm();
  }

  onCreateCampaign() {
    this.formatCampaignType();

    this.campaignService.createCampaign(this.detailsForm.value).subscribe(data => {
      this.ref.close(this.detailsForm.value.name);
    });
  }

  private initForm() {
    this.detailsForm = this.formBuilder.group({
      name: ['', [
        Validators.required,
        Validators.maxLength(100),
        Validators.pattern('[a-zA-ZА-я ][a-zA-ZА-я0-9 ]*')]
      ],
      type: ['', [Validators.required]],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]]
    });
  }

  private formatCampaignType() {
    this.detailsForm.value.type = this.detailsForm.value.type[0].value;
  }
}

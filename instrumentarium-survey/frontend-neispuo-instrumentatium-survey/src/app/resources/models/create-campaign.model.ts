export class CreateCampaignDTO {
  campaign: {
    name: string;
    type: number;
    startDate: string;
    endDate: string;
    isActive: number;
    isLocked: number;
    createdBy: number;
    institutionId: number;
  };
}

class CampaignQuestionairesModel {
  questionaireId: number;
}

class CampaignSysUserModel {
  sysUser: number;
}
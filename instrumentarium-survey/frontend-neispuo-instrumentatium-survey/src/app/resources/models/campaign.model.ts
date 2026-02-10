import { CampaignType } from '../enums/campaign-type.enum';

export class Campaign {
  id: number;
  name: string;
  type: CampaignType;
  startDate: Date;
  endDate: Date;
  isActive: boolean;
  isLocked: boolean;
  updatedAt: Date;
  institutionId: number;
  createdBy: number;

  constructor(
    name: string,
    startDate: Date,
    endDate: Date,
  ) {
    this.name = name;
    this.startDate = startDate;
    this.endDate = endDate;
  }
}
import { Injectable } from '@angular/core';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
import { Observable } from 'rxjs';
import { Campaign } from 'src/app/resources/models/campaign.model';
import { CreateCampaignDTO } from 'src/app/resources/models/create-campaign.model';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class CampaignService {

  constructor(
    private apiService: ApiService
  ) {}

  getAllCampaigns(queryParams?: RequestQueryBuilder['queryObject']) {
    return this.apiService.get(`/v1/campaign`, queryParams);
  }

  getCampaignById(id: string): Observable<Campaign> {
    return this.apiService.get<Campaign>(`/v1/campaign/${id}`);
  }

  createCampaign(data: CreateCampaignDTO): Observable<Response> {
    return this.apiService.post('/v1/campaign', data);
  }

  editCampaign(id: number, data: Campaign): Observable<Response> {
    return this.apiService.patch(`/v1/campaign/${id}`, data);
  }

  getNumberOfUsersWhoFilledInTheCampaign(id: number): Observable<Response> {
    return this.apiService.get(`/v1/campaign/aggregated?campaignId=${id}`);
  }

  getAggregatedResults(id: number) {
    return this.apiService.get(`/v1/aggregated-results/?s={"campaignId.ID": ${id}}`);
  }

  deleteCampaign(campaigns: Campaign[] = []): Observable<Response> {
    let campaignsIds = `/v1/campaign/delete-many?campaign[]=${campaigns[0]}`;

    campaigns.forEach((el, index) => {
      campaignsIds += `&campaign[]=${campaigns[index]}`;
    });

    return this.apiService.delete(campaignsIds);
  }
}

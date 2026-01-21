import { Injectable } from '@angular/core';
import { RequestQueryBuilder } from '@nestjsx/crud-request';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private readonly apiService: ApiService) {}

  getUsersFromCampaign(campaignID: number): Observable<any> {
    return this.apiService.get<any>(`/v1/campaign-sysUser/?s={"campaign.id": ${campaignID}}`);
  }
}

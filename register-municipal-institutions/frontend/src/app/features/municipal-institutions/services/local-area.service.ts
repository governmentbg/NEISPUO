import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { Town } from '@municipal-institutions/models/town';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LocalAreaService {
  private readonly baseEndpoint;

  constructor(
    private httpClient: HttpClient,
    private envService: EnvironmentService,
  ) {
    this.baseEndpoint = envService.environment.BACKEND_URL;
  }

  getLocalAreas(town: Town): Observable<any> {
    return this.httpClient.get<[]>(`${this.baseEndpoint}/v1/local-area?s={"TownCode":${town.Code}}`);
  }
}

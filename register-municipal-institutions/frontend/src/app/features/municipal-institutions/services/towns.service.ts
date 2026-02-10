import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { Municipality } from '@municipal-institutions/models/municipality';
import { Town } from '@municipal-institutions/models/town';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TownsService {
  private readonly baseEndpoint;

  constructor(
    private httpClient: HttpClient,
    private envService: EnvironmentService,
  ) {
    this.baseEndpoint = envService.environment.BACKEND_URL;
  }

  getTowns(municipality: Municipality): Observable<any> {
    return this.httpClient.get<Town[]>(
      `${this.baseEndpoint}/v1/town?s={"Municipality.MunicipalityID":${municipality.MunicipalityID}}`,
    );
  }
}

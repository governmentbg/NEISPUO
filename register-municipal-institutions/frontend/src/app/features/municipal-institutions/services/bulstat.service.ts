import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class BulstatService {
  private readonly baseEndpoint;

  private readonly _municipalInstitution$ = new BehaviorSubject<MunicipalInstitution>(null);

  constructor(
    private httpClient: HttpClient,
    private envService: EnvironmentService,
  ) {
    this.baseEndpoint = this.envService.environment.BACKEND_URL;
  }

  get municipalInstitution$() {
    return this._municipalInstitution$.asObservable();
  }

  set municipalInstitution(mi: MunicipalInstitution) {
    this._municipalInstitution$.next(mi);
  }

  public verify(bulstat: string): Observable<any> {
    return this.httpClient
      .get(`${this.baseEndpoint}/v1/bulstat/bulstat-check/${bulstat}`);
  }

}

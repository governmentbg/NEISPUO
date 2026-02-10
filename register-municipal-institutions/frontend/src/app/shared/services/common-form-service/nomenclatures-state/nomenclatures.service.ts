import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { forkJoin } from 'rxjs';
import { tap } from 'rxjs/operators';
import { NomenclatureStore } from './nomenclatures.store';

@Injectable({
  providedIn: 'root',
})
export class NomenclatureService {
  private readonly baseEndpoint;

  constructor(
    private httpClient: HttpClient,
    private nomenclatureStore: NomenclatureStore,
    private envService: EnvironmentService,
  ) {
    this.baseEndpoint = envService.environment.BACKEND_URL;
  }

  loadAll() {
    return forkJoin([
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/municipality`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/region`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/financial-school-type`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/base-school-type`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/budgeting-institution`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/detailed-school-type`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/procedure-type`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/transform-type`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/cplr-area-type`),
      this.httpClient.get<any>(`${this.baseEndpoint}/v1/version`),
      this.httpClient.get<[]>(`${this.baseEndpoint}/v1/current-year`),
    ]).pipe(
      tap(
        ([
          Municipalities,
          Regions,
          FinancialSchoolTypes,
          BaseSchoolTypes,
          BudgetingInstitutions,
          DetailedSchoolTypes,
          ProcedureTypes,
          TransformTypes,
          CPLRAreaTypes,
          BackendVersion,
          CurrentYear
        ]) => {
          this.nomenclatureStore.update({
            Municipalities,
            Regions,
            FinancialSchoolTypes,
            BaseSchoolTypes,
            BudgetingInstitutions,
            DetailedSchoolTypes,
            ProcedureTypes,
            TransformTypes,
            CPLRAreaTypes,
            BackendVersion,
            CurrentYear
          });
        },
      ),
    );
  }
}

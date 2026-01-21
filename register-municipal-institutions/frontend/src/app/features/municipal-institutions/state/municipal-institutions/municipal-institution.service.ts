import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetManyDefaultResponse } from '@municipal-institutions/pages/municipal-institution-list/municipal-institution-list.page';
import { tap } from 'rxjs/operators';
import { EnvironmentService } from '@core/services/environment.service';
import { MunicipalInstitution } from './municipal-institution.interface';
import { MunicipalInstitutionStore } from './municipal-institution.store';

type PaginatedMunicipalInstitution = GetManyDefaultResponse<MunicipalInstitution>;

@Injectable({
  providedIn: 'root',
})
export class MunicipalInstitutionService {
  constructor(
    private httpClient: HttpClient,
    private miStore: MunicipalInstitutionStore,
    private envService: EnvironmentService,
  ) {}

  private baseEndpoint = this.envService.environment.BACKEND_URL;

  get(path: string, params = {}) {
    this.miStore.set([]);
    this.miStore.setLoading(true);

    return this.httpClient
      .get<PaginatedMunicipalInstitution>(`${this.baseEndpoint}${path}`, { params })
      .pipe(
        tap((resp: PaginatedMunicipalInstitution) => {
          this.miStore.set(resp.data);
          this.miStore.setLoading(false);
          this.miStore.update({ total: resp.total });
        }),
      )
      .toPromise();
  }

  getOne(path: string) {
    return this.httpClient
      .get<MunicipalInstitution>(`${this.baseEndpoint}${path}`)
      .pipe(
        tap((resp: MunicipalInstitution) => {
          this.miStore.set([resp]);
          this.miStore.update({ total: 1 });
        }),
      )
      .toPromise();
  }

  createOne(path: string, body: MunicipalInstitution) {
    return this.httpClient.post<MunicipalInstitution>(`${this.baseEndpoint}${path}`, body);
  }

  updateOne(path: string, body: MunicipalInstitution) {
    return this.httpClient.patch<MunicipalInstitution>(`${this.baseEndpoint}${path}`, body);
  }

  deleteOne(path: string, body?: MunicipalInstitution) {
    return this.httpClient.request<MunicipalInstitution>('delete', `${this.baseEndpoint}${path}`, { body });
  }

  getHistory(id: number) {
    return this.httpClient.get<MunicipalInstitution[]>(`${this.baseEndpoint}/v1/ri-institution/history/${id}`);
  }
}

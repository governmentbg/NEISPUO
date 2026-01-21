import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { tap } from 'rxjs/operators';
import { EnvironmentService } from '@core/services/environment.service';
import { MergeProcedureStore } from './merge-procedure.store';

export interface GetManyDefaultResponse<MunicipalInstitution> {
  data: MunicipalInstitution[];
  count: number;
  total: number;
  page: number;
  pageCount: number;
}

type PaginatedMunicipalInstitution = GetManyDefaultResponse<MunicipalInstitution>;

@Injectable()
export class MergeProcedureService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(private httpClient: HttpClient, private jpStore: MergeProcedureStore, private envService: EnvironmentService) { }

  /** General */

  updateMIsToDelete(mis: MunicipalInstitution[]) {
    this.jpStore.update({ misToDelete: mis });
  }

  updateMIToCreate(mi: MunicipalInstitution) {
    this.jpStore.update({ miToCreate: mi });
  }

  updateActiveMIs(updatedMIs: MunicipalInstitution[]) {
    const activeMIs = this.jpStore.getValue().activeMIs;
    for (const umi of updatedMIs) {
      for (let i = 0; i < activeMIs.length; i += 1) {
        if (activeMIs[i].RIInstitutionID === umi.RIInstitutionID) {
          activeMIs[i] = { ...activeMIs[i], RIProcedure: umi.RIProcedure };
        }
      }
    }
    this.jpStore.update({ activeMIs });
  }

  updateActiveInstitutions() {
    return this.httpClient.get<PaginatedMunicipalInstitution>(`${this.baseEndpoint}/v1/ri-institution-latest-active`)
      .pipe(
        tap(
          ((institutionsResponse: PaginatedMunicipalInstitution) => {
            this.jpStore.update({ activeMIs: institutionsResponse.data });
          }),
        ),
      ).toPromise();
  }

  addRIDocumentToMIsToCreate(mi: MunicipalInstitution) {
    const mis = this.jpStore.getValue().miToCreate;
    this.jpStore.update({ miToCreate: { ...mis, RIProcedure: { ...mis.RIProcedure, RIDocument: mi.RIProcedure.RIDocument } } });
  }

  addRIDocumentToMIToDelete(mi: MunicipalInstitution) {
    const mis = this.jpStore.getValue().misToDelete;
    const misToDeleteNew = mis.map((m) => ({
      ...m,
      RIProcedure: { ...m.RIProcedure, RIDocument: mi.RIProcedure.RIDocument },
    }));
    this.jpStore.update({ misToDelete: misToDeleteNew });
  }

  merge() {
    return this.httpClient.post(
      `${this.baseEndpoint}/v1/ri-institution-merge`,
      { miToCreate: this.jpStore.getValue().miToCreate, misToDelete: this.jpStore.getValue().misToDelete },
    );
  }

  resetState() {
    this.jpStore.reset();
  }
}

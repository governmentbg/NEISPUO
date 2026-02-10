import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { tap } from 'rxjs/operators';
import { EnvironmentService } from '@core/services/environment.service';
import { JoinProcedureStore } from './join-procedure.store';

export interface GetManyDefaultResponse<MunicipalInstitution> {
  data: MunicipalInstitution[];
  count: number;
  total: number;
  page: number;
  pageCount: number;
}

type PaginatedMunicipalInstitution = GetManyDefaultResponse<MunicipalInstitution>;

@Injectable()
export class JoinProcedureService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(private httpClient: HttpClient, private jpStore: JoinProcedureStore, private envService: EnvironmentService) { }

  /** General */

  updateMIsToDelete(mis: MunicipalInstitution[]) {
    this.jpStore.update({ misToDelete: mis });
  }

  updateMIToUpdate(mi: MunicipalInstitution) {
    this.jpStore.update({ miToUpdate: mi });
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

  addRIDocumentToMIToUpdate(mi: MunicipalInstitution) {
    const mis = this.jpStore.getValue().miToUpdate;
    this.jpStore.update({ miToUpdate: { ...mis, RIProcedure: { ...mis.RIProcedure, RIDocument: mi.RIProcedure.RIDocument } } });
  }

  addRIDocumentToMIsToDelete(mi: MunicipalInstitution) {
    const mis = this.jpStore.getValue().misToDelete;
    const misToDeleteNew = mis.map((m) => ({
      ...m,
      RIProcedure: { ...m.RIProcedure, RIDocument: mi.RIProcedure.RIDocument },
    }));
    this.jpStore.update({ misToDelete: misToDeleteNew });
  }

  join() {
    return this.httpClient.post(
      `${this.baseEndpoint}/v1/ri-institution-join`,
      { miToUpdate: this.jpStore.getValue().miToUpdate, misToDelete: this.jpStore.getValue().misToDelete },
    );
  }

  resetState() {
    this.jpStore.reset();
  }
}

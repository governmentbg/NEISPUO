import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { tap } from 'rxjs/operators';
import { EnvironmentService } from '@core/services/environment.service';
import { DivideProcedureStore } from './divide-procedure.store';

export interface GetManyDefaultResponse<MunicipalInstitution> {
  data: MunicipalInstitution[];
  count: number;
  total: number;
  page: number;
  pageCount: number;
}

type PaginatedMunicipalInstitution = GetManyDefaultResponse<MunicipalInstitution>;

@Injectable()
export class DivideProcedureService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;
  isEditMode: boolean;

  constructor(private httpClient: HttpClient, private dpStore: DivideProcedureStore, private envService: EnvironmentService) { }

  /** General */

  updateMIsToCreate(mis: MunicipalInstitution[]) {
    this.dpStore.update({ misToCreate: mis });
  }

  updateMIToDelete(mi: MunicipalInstitution) {
    this.dpStore.update({ miToDelete: mi });
  }

  updateActiveMIs(updatedMIs: MunicipalInstitution[]) {
    const activeMIs = this.dpStore.getValue().activeMIs;
    for (const umi of updatedMIs) {
      for (let i = 0; i < activeMIs.length; i += 1) {
        if (activeMIs[i].RIInstitutionID === umi.RIInstitutionID) {
          activeMIs[i] = { ...activeMIs[i], RIProcedure: umi.RIProcedure };
        }
      }
    }
    this.dpStore.update({ activeMIs });
  }

  updateActiveInstitutions() {
    return this.httpClient.get<PaginatedMunicipalInstitution>(`${this.baseEndpoint}/v1/ri-institution-latest-active`)
      .pipe(
        tap(
          ((institutionsResponse: PaginatedMunicipalInstitution) => {
            this.dpStore.update({ activeMIs: institutionsResponse.data });
          }),
        ),
      ).toPromise();
  }

  addMIToCreate(mi: MunicipalInstitution) {
    const mis = this.dpStore.getValue().misToCreate;
    this.dpStore.update({ misToCreate: [...mis, mi] });
  }

  updateMIToCreate(mi: MunicipalInstitution, index: number) {
    const misInStore = this.dpStore.getValue().misToCreate;
    if (misInStore[index]) {
      misInStore[index] = mi;
    }
    this.dpStore.update({ misToCreate: [...misInStore] });
  }

  removeMIToCreate(index: number) {
    const misInStore = this.dpStore.getValue().misToCreate;
    misInStore.splice(index, 1);
    this.dpStore.update({ misToCreate: [...misInStore] });
  }

  addRIDocumentToMIsToCreate(mi: MunicipalInstitution) {
    const mis = this.dpStore.getValue().misToCreate;
    const misToCreateNew = mis.map((m) => ({
      ...m,
      RIProcedure: { ...m.RIProcedure, RIDocument: mi.RIProcedure.RIDocument },
    }));
    this.dpStore.update({ misToCreate: misToCreateNew });
  }

  addRIDocumentToMIToDelete(mi: MunicipalInstitution) {
    const mis = this.dpStore.getValue().miToDelete;
    this.dpStore.update({ miToDelete: { ...mis, RIProcedure: { ...mis.RIProcedure, RIDocument: mi.RIProcedure.RIDocument } } });
  }

  divide() {
    return this.httpClient.post(
      `${this.baseEndpoint}/v1/ri-institution-divide`,
      { misToCreate: this.dpStore.getValue().misToCreate, miToDelete: this.dpStore.getValue().miToDelete },
    );
  }

  resetState() {
    this.dpStore.reset();
  }
}

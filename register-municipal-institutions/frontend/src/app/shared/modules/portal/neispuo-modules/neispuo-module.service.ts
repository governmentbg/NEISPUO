import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthQuery } from 'src/app/core/authentication/auth-state-manager/auth.query';
import { tap } from 'rxjs/operators';
import { EnvironmentService } from '@core/services/environment.service';
import { NeispuoModule } from './neispuo-module.interface';
import { NeispuoModuleStore } from './neispuo-module.store';

@Injectable({
  providedIn: 'root',
})
export class NeispuoModuleService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(
    private httpClient: HttpClient,
    private nmStore: NeispuoModuleStore,
    private authQuery: AuthQuery,
    private envService: EnvironmentService,
  ) { }

  get() {
    return this.httpClient.get<NeispuoModule[]>(`${this.baseEndpoint}/v1/neispuo-module`)
      .pipe(
        tap((resp) => this.nmStore.update({ modules: resp })),
      );
  }

  getCategories() {
    return this.httpClient.get<NeispuoModule[]>(`${this.baseEndpoint}/v1/neispuo-category`)
      .pipe(
        tap((resp) => this.nmStore.update({ categories: resp })),
      );
  }
}

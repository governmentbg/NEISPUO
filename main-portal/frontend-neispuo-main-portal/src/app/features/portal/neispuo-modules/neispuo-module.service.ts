import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { EnvironmentService } from '@shared/services/environment.service';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { NeispuoModule } from './neispuo-module.interface';
import { NeispuoModuleStore } from './neispuo-module.store';
import { NeispuoUserGuide } from './neispuo-user-guide.interface';
import { NeispuoCategory } from './neispuo-category.interface';

@Injectable({
  providedIn: 'root'
})
export class NeispuoModuleService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;
  constructor(
    private httpClient: HttpClient,
    private nmStore: NeispuoModuleStore,
    private authQuery: AuthQuery,
    private envService: EnvironmentService
  ) {}

  get() {
    return this.httpClient
      .get<NeispuoModule[]>(`${this.baseEndpoint}/v1/neispuo-module`)
      .pipe(tap((resp) => this.nmStore.update({ modules: resp })));
  }

  getCategories() {
    return this.httpClient.get<NeispuoCategory[]>(`${this.baseEndpoint}/v1/neispuo-category`).pipe(
      tap((resp) => {
        this.nmStore.update({ categories: resp }), this.nmStore.setLoading(false);
      }),
      catchError((error: HttpErrorResponse) => {
        this.nmStore.setError(error);
        return throwError(error);
      })
    );
  }

  getUserGuides() {
    return this.httpClient
      .get<NeispuoUserGuide[]>(`${this.baseEndpoint}/v1/user-guide`)
      .pipe(tap((resp) => this.nmStore.update({ userGuides: resp })));
  }

  downloadFile(path: number): Observable<Blob> {
    return this.httpClient.get(`${this.baseEndpoint}/v1/file/download/${path}`, {
      responseType: 'blob'
    });
  }
}

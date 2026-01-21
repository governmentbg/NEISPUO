import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EnvironmentService } from './environment.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private errorMessage = '';
  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(private http: HttpClient, private envService: EnvironmentService) {}

  get<T = any>(path: string, params = {}): Observable<T> {
    return this.http.get<T>(`${this.baseEndpoint}${path}`, { params }).pipe(catchError(this.error));
  }

  post(path: string, body: object = {}): Observable<any> {
    return this.http.post(`${this.baseEndpoint}${path}`, body).pipe(catchError(this.error));
  }

  put(path: string, body: object = {}): Observable<any> {
    return this.http.put(`${this.baseEndpoint}${path}`, body).pipe(catchError(this.error));
  }

  patch(path: string, body: object = {}): Observable<any> {
    return this.http.patch(`${this.baseEndpoint}${path}`, body).pipe(catchError(this.error));
  }

  delete(path: string): Observable<any> {
    return this.http.delete(`${this.baseEndpoint}${path}`).pipe(catchError(this.error));
  }

  downloadFile(path: string): Observable<Blob> {
    return this.http.get(`${this.baseEndpoint}${path}`, { responseType: 'blob' }).pipe(catchError(this.error));
  }

  private error(error: HttpErrorResponse) {
    error.error instanceof ErrorEvent
      ? (this.errorMessage = error.error.message)
      : (this.errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`);

    return throwError(this.errorMessage);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EnvironmentService } from '@shared/services/environment.service';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseEndpoint = this.envService.environment.BACKEND_URL;

  constructor(private http: HttpClient, private envService: EnvironmentService) {}

  get<T = any>(path: string, params = {}): Observable<T> {
    return this.http.get<T>(`${this.baseEndpoint}${path}`, { params });
  }

  post<T = any>(path: string, body: object = {}): Observable<T> {
    return this.http.post<T>(`${this.baseEndpoint}${path}`, body);
  }

  put<T = any>(path: string, body: object = {}): Observable<T> {
    return this.http.put<T>(`${this.baseEndpoint}${path}`, body);
  }

  delete(path: string): Observable<any> {
    return this.http.delete(`${this.baseEndpoint}${path}`);
  }
  downloadFile(path: string, body: object = {}): Observable<Blob> {
    return this.http.post(`${this.baseEndpoint}${path}`, body, {
      responseType: 'blob' as 'blob'
    });
  }
}

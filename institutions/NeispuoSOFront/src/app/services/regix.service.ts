import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { AuthService } from '../auth/auth.service';

@Injectable()
export class RegixService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getRegixData(data: { operation: string; xmlns: string; requestName: string; params }): Observable<any> {
    return this.http.get(`${environment.regixUrl}`, {
      params: data,
      headers: new HttpHeaders({
        Authorization: `Bearer ${this.authService.getToken()}`
      })
    });
  }
}

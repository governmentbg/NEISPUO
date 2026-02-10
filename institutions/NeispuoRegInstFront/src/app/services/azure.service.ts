import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class AzureService {
  constructor(private http: HttpClient) {}

  createSchool(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/school/create`, body);
  }

  deleteSchool(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/school/delete`, body);
  }
}

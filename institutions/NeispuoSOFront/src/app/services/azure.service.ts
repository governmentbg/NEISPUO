import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class AzureService {
  constructor(private http: HttpClient) {}

  createTeacher(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/teacher/create`, body);
  }

  createInstitutionTeacher(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/teacher/enrollment-school-create`, body);
  }

  updateTeacher(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/teacher/update`, body);
  }

  deleteTeacher(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/teacher/enrollment-school-delete`, body);
  }

  createClass(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/class/create`, body);
  }

  updateClass(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/class/update`, body);
  }

  deleteClass(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/class/delete`, body);
  }

  teacherClassesEnroll(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/teacher/enrollment-class-create`, body);
  }

  teacherClassesDelete(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/teacher/enrollment-class-delete`, body);
  }

  studentClassesEnroll(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/student/enrollment-class-create`, body);
  }

  studentClassesDelete(body): Observable<any> {
    return this.http.post(`/v1/azure-integrations/student/enrollment-class-delete`, body);
  }
}

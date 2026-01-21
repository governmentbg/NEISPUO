import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { FormType, FormTypeInt } from "../enums/formType.enum";

@Injectable()
export class TreeDataService {
  constructor(private http: HttpClient) {}

  getTreeData(formType: FormType): Observable<any> {
    const instType = FormTypeInt[formType] + 1;
    return this.http.post(`/uimeta/getnav/institution-tree`, { instType });
  }
}

import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { FormType, FormTypeInt } from "../enums/formType.enum";

@Injectable()
export class TreeDataService {
  constructor(private http: HttpClient) {}

  getTreeData(isActive: boolean, formType: FormType): Observable<any> {
    const active = isActive ? 1 : 0;
    const instType = FormTypeInt[formType] + 1;
    return this.http.post(`/uimeta/getnav/institution-tree`, { isRIActive: active, instType });

    //  return this.http.get(`assets/json/tree-data/${formType}-tree.json`);
  }
}

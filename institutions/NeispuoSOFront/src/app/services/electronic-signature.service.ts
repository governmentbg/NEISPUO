import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { timeout } from "rxjs/operators";
import { environment } from "../../environments/environment";

@Injectable({ providedIn: "root" })
export class ElectronicSignatureService {
  constructor(private http: HttpClient) {}

  sign(data: Object, eik?: string) {
    return this.http.post(`${environment.localServer}/api/certificate/sign`, {
      xml: `<xml><contents>${JSON.stringify(data)}</contents></xml>`,
      eik: eik || ""
    });
  }

  getVersion() {
    return this.http.get(`${environment.localServer}/api/server/version`).pipe(timeout(2000));
  }
}

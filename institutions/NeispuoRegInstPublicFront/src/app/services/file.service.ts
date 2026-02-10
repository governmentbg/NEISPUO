import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({ providedIn: "root" })
export class FileService {
  constructor(private http: HttpClient) {}

  getFile(fileBlobId, instid, procid): Observable<any> {
    return this.http.get<Blob>(`/data/file/${fileBlobId}/${instid}/${procid}`, {
      responseType: "blob" as "json"
    });
  }

  getXlsx(xclName: string) {
    return this.http.post<Blob>(`/data/generatexls/${xclName}`, {}, { responseType: "blob" as "json" });
  }
}

import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { AuthService } from "../auth/auth.service";

@Injectable()
export class FileService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  private real: boolean = true;

  getCertificateFile(fileBlobId, submissionDataId): Observable<any> {
    return this.http.get<Blob>(`/data/file/certificate/${fileBlobId}/${submissionDataId}`, { responseType: "blob" as "json" });
  }

  getBuildingDocument(buildingId, initialOwnershipDoc, latestOwnershipDoc): Observable<any> {
    return this.http.get<Blob>(`/data/file/building/${buildingId}/${initialOwnershipDoc}/${latestOwnershipDoc}`, {
      responseType: "blob" as "json"
    });
  }

  getClassGroupDocument(fileBlobId, classID): Observable<any> {
    return this.http.get<Blob>(`/data/file/classGroup/${fileBlobId}/${classID}`, { responseType: "blob" as "json" });
  }

  uploadFile(file: File) {
    const formData: FormData = new FormData();
    formData.append("file", file, file.name);
    return this.http.post(environment.blobsMon, formData, {
      headers: new HttpHeaders({
        Authorization: `Bearer ${this.authService.getToken()}`
      })
    });
  }

  getCertificateTemplate() {
    return this.http.get<Blob>("assets/templates/certificate-template.html", { responseType: "blob" as "json" });
  }

  getCertificateData(submissionDataID: string | number) {
    return this.real
      ? this.http.post("/data/get/soSubmissionCertificate", { submissionDataID })
      : this.http.get("assets/json/fake-data/certificate.json");
  }

  substituteTemplateInfo(html: string, fieldValues: Object) {
    if (fieldValues) {
      for (let key in fieldValues) {
        if (fieldValues[key] && typeof fieldValues[key] === "object" && fieldValues[key].length) {
          for (let innerKey in fieldValues[key][0]) {
            const regexp = new RegExp(`{{${key}.${innerKey}}}`, "g");
            html = html.replace(regexp, fieldValues[key][0][innerKey]);
          }
        } else {
          const regexp = new RegExp(`{{${key}}}`, "g");
          html = html.replace(regexp, fieldValues[key]);
        }
      }
    }

    html = html.replace(/{{\w*}}/g, "");

    return html;
  }

  downloadPDF(blob, fileName: string) {
    const file = new File([blob], `${fileName}.pdf`, { type: ".pdf" });
    const anchor = document.createElement("a");
    anchor.href = window.URL.createObjectURL(file);
    anchor.download = `${fileName}.pdf`;
    anchor.click();
  }
}

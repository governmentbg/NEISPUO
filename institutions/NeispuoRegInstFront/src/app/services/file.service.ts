import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class FileService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getFile(institutionId, fileBlobId): Observable<any> {
    return this.http.get<Blob>(`/data/file/${institutionId}/${fileBlobId}`, { responseType: 'blob' as 'json' });
  }

  uploadFile(file: File) {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post(environment.blobsMon, formData, {
      headers: new HttpHeaders({
        Authorization: `Bearer ${this.authService.getToken()}`
      })
    });
  }

  getCertificateData(body) {
    return this.http.post('/data/get/certificate', body);
  }

  getCertificateTemplate() {
    return this.http.get<Blob>('assets/templates/certificate/template.html', { responseType: 'blob' as 'json' });
  }

  saveCertificateData(body) {
    return this.http.post('/data/save', body);
  }

  substituteTemplateInfo(html: string, fieldValues: Object) {
    if (fieldValues) {
      for (let key in fieldValues) {
        if (fieldValues[key] && typeof fieldValues[key] === 'object' && fieldValues[key].length) {
          for (let innerKey in fieldValues[key][0]) {
            const regexp = new RegExp(`{{${key}.${innerKey}}}`, 'g');
            html = html.replace(regexp, fieldValues[key][0][innerKey]);
          }
        } else {
          const regexp = new RegExp(`{{${key}}}`, 'g');
          html = html.replace(regexp, fieldValues[key]);
        }
      }
    }

    html = html.replace(/{{\w*}}/g, '');

    return html;
  }
}

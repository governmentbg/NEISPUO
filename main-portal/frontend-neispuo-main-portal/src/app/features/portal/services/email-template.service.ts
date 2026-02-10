import { Injectable } from '@angular/core';
import { EmailTemplate } from '@portal/components/email-template-list/interfaces/email-template.interface';
import { EmailTemplateForm } from '@portal/components/email-template/interfaces/email-template-form.interface';
import { EmailTemplateTypeResponse } from '@portal/components/email-template/interfaces/email-template-type-response.interface';
import { ApiService } from '@shared/services/api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmailTemplateService {
  constructor(private apiService: ApiService) {}

  getTemplateTypes(): Observable<EmailTemplateTypeResponse[]> {
    return this.apiService.get('/v1/email-template-type');
  }

  saveTemplate(payload: EmailTemplateForm): Observable<any> {
    return this.apiService.post('/v1/email-template', payload);
  }

  updateTemplate(id: number, payload: EmailTemplateForm): Observable<any> {
    return this.apiService.put(`/v1/email-template/${id}`, payload);
  }

  deleteTemplate(id: number): Observable<any> {
    return this.apiService.delete(`/v1/email-template/${id}`);
  }

  getAllTemplates(): Observable<EmailTemplate[]> {
    return this.apiService.get('/v1/email-template');
  }

  getOneTemplate(id: number): Observable<EmailTemplate> {
    return this.apiService.get(`/v1/email-template/${id}`);
  }

  sendTemplate(id: number, payload: { fromDate?: Date; toDate?: Date }): Observable<any> {
    return this.apiService.post(`/v1/email-template/${id}/send`, payload);
  }
}

import { Injectable } from '@angular/core';
import { SystemMessagePayload } from '@portal/components/system-message-form/system-message-form.interface';
import { SystemUserMessage } from '@portal/components/system-user-messages/system-user-message.interface';
import { SysRole } from '@shared/interfaces/sys-role.interface';
import { ApiService } from '@shared/services/api.service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SystemUserMessagesService {
  constructor(private apiService: ApiService) {}

  getSystemUserMessages(): Observable<SystemUserMessage[]> {
    return this.apiService.get('/v1/system-user-message');
  }

  createSystemMessage(message: SystemMessagePayload) {
    return this.apiService.post('/v1/system-user-message', message);
  }

  updateSystemMessage(id: number, message: SystemMessagePayload) {
    return this.apiService.put(`/v1/system-user-message/${id}`, message);
  }

  deleteSystemMessage(id: number) {
    return this.apiService.delete(`/v1/system-user-message/${id}`);
  }

  getAllSysRoles(): Observable<SysRole[]> {
    return this.apiService.get('/v1/sys-role');
  }
}

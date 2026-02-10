import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPerson } from '@shared/modals/person.modal';
import { EnvironmentService } from '@shared/services/environment.service';

@Injectable({
  providedIn: 'root'
})
export class ParentChildrenService {
  private userMngmtBaseEndpoint = this.envService.environment.USER_MANAGMENT_BACKEND_URL;
  constructor(private http: HttpClient, private envService: EnvironmentService) {}
  // service is not using the ApiService, because we need to use the User Managment backend API instead of main portal BE

  getParentChildren() {
    return this.http.get<IPerson[]>(`${this.userMngmtBaseEndpoint}/v1/person/children`);
  }

  enrollChild(child, parentID: number) {
    return this.http.post(`${this.userMngmtBaseEndpoint}/v1/parent-access/v1/child/enroll`, {
      parentID,
      childPersonalID: child.personalID,
      childSchoolBookCode: child.schoolBookCode
    });
  }

  delistChild(child: IPerson, parentID: number) {
    return this.http.post(`${this.userMngmtBaseEndpoint}/v1/parent-access/v1/child/unenroll`, {
      parentID,
      childPersonalID: child.personalID,
      childSchoolBookCode: child.schoolBooksCodes
    });
  }
}

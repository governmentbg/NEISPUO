import { Component, OnInit } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { CondOperator, RequestQueryBuilder } from '@nestjsx/crud-request';
import { IUserProfile } from '@shared/modals/user-profile.modal';
import { ApiService } from '@shared/services/api.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  selectedRole$ = this.authQuery.select('selected_role');
  userData: IUserProfile;

  constructor(private authQuery: AuthQuery, private apiService: ApiService) {}

  ngOnInit(): void {
    this.selectedRole$.subscribe((r) => {
      this.apiService.get<IUserProfile>(`/v1/user-profile`).subscribe((resp) => {
        this.userData = resp;
      });
    });
  }
}

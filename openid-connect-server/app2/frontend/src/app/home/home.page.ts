import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { OIDCService } from '../oidc.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.scss'],
})
export class HomePage implements OnInit {
  constructor(public oidcService: OIDCService) { }

  ngOnInit(): void {
  }

  async login(): Promise<void> {
    await this.oidcService.userManager.signinRedirect();
  }

  async logout() {
    await this.oidcService.userManager.signoutRedirect()
  }
}

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { OIDCService } from '../oidc.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.scss'],
})
export class HomePage {
  lastLoadedAt: Date;
  unprotectedResource: string;
  protectedResource: string;
  protectedResourceWithForeignJwt: string;

  constructor(public oidcService: OIDCService, private httpClient: HttpClient) { }

  async demoGetResources() {
    const jwt = (await this.oidcService.userManager.getUser())?.access_token
    const authHeaders = `Bearer ${jwt}`
    const protectedUrl = `${environment.resourceServerUrl}/protected-resource`
    const unprotectedUrl = `${environment.resourceServerUrl}/unprotected-resource`
    await this.httpClient.get<{ message: string }>(protectedUrl, { headers: { authorization: authHeaders } })
      .toPromise()
      .then(v => this.protectedResource = v.message)
      .catch(v => this.protectedResource = null)
    await this.httpClient.get<{ message: string }>(unprotectedUrl, { headers: { authorization: authHeaders } })
      .toPromise()
      .then(v => this.unprotectedResource = v.message)
      .catch(v => this.unprotectedResource = null)

    /** A jwt signed with a diferent private key than OIDC's. For demo's negative case. */
    const foreignAuthHeaders = `Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6InIxTGtiQm8zOTI1UmIyWkZGckt5VTNNVmV4OVQyODE3S3gwdmJpNmlfS2MifQ.eyJzdWIiOiJhYWEiLCJhdF9oYXNoIjoiRHVQQVVuMkhLYXUtbWx3OHMxZllvdyIsImF1ZCI6ImFwcDEiLCJleHAiOjIwMDYxMjg0NzAsImlhdCI6MTYwNjEyNDg3MCwiaXNzIjoiaHR0cHM6Ly9kc3Mtb2lkYy1zZXJ2ZXIuemVub25jdWx0dXJhbC5jb20ifQ.eAGrZBjEyNdftF8LlekXYjscVTFAomd6ar1MCzQDuYN2evGCJVr2ANRJSMiw79Gz3ibQYJrrJVDL92OVGVt7iVpi5O0CkNwutio3S2vJJaN95HYjH5_3WX5Q6lA8WWkw86sqRJGEU2zvVxX-UZw9E33lRVQSx2qAOMdJIusNH-0`
    await this.httpClient.get<{ message: string }>(protectedUrl, { headers: { authorization: foreignAuthHeaders } })
      .toPromise()
      .then(v => this.protectedResourceWithForeignJwt = v.message)
      .catch(v => this.protectedResourceWithForeignJwt = null)

    this.lastLoadedAt = new Date()
  }

  async login(): Promise<void> {
    await this.oidcService.userManager.signinRedirect();
  }

  async logout() {
    await this.oidcService.userManager.signoutRedirect()
  }
}
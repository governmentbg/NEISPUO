import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthConfig, OAuthErrorEvent, OAuthService } from 'angular-oauth2-oidc';
import { filter } from 'rxjs/operators';
import { debounceUntilVisible } from '../utils/rxjs';

const AUTH_STORAGE_ITEMS: string[] = [
  'access_token',
  'access_token_stored_at',
  'expires_at',
  'granted_scopes',
  'id_token',
  'id_token_claims_obj',
  'id_token_expires_at',
  'id_token_stored_at',
  'nonce',
  'PKCE_verifier',
  'refresh_token',
  'session_state'
];

const cleanOAuthServiceSessionStorage = (sessionStorage: Storage) => {
  AUTH_STORAGE_ITEMS.map((item: string) => {
    sessionStorage.removeItem(item);
  });
};

const trimTrailingSlash = (url: string) => (url.endsWith('/') ? url.slice(0, -1) : url);

const initiateCodeFlowWithReturnUrl = (window: Window, oauthService: OAuthService): Promise<void> => {
  cleanOAuthServiceSessionStorage(window.sessionStorage);

  const basePath = trimTrailingSlash(window.document.baseURI);

  const returnUrl = window.document.location.href.substring(basePath.length);

  oauthService.initCodeFlow(returnUrl === '' || returnUrl === '/' ? undefined : returnUrl);

  // return a never resolving promise
  // as initCodeFlow should redirect us to the OIDC server
  return new Promise(() => {
    // do nothing
  });
};

@Injectable({
  providedIn: 'root'
})
export class AuthInitializerService {
  constructor(private oauthService: OAuthService) {}

  initialize(authServerPath: string, authRequireHttps: boolean): Promise<void> {
    const authCodeFlowConfig: AuthConfig = {
      issuer: authServerPath,
      redirectUri: document.baseURI,
      redirectUriAsPostLogoutRedirectUriFallback: true,
      silentRefreshRedirectUri: `${trimTrailingSlash(document.baseURI)}/silent-refresh.html`,
      clientId: 'school-books-frontend',
      responseType: 'code',
      scope: 'openid profile email',
      useSilentRefresh: true,
      sessionChecksEnabled: true,
      sessionCheckIntervall: 60000,
      requireHttps: authRequireHttps,
      showDebugInformation: false
    };

    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.setupAutomaticSilentRefresh();

    this.oauthService.events
      .pipe(filter((event) => event instanceof OAuthErrorEvent && event.type === 'silent_refresh_timeout'))
      .subscribe(() => {
        // If for whatever reason the silent-refresh.html is inaccessible the refresh will timeout
        // and will never be retried, leaving the session to quietly expire and the user will start
        // seeing the 'unexpeced error' snackbar for the 401 responses.
        // The solution here is to restart the silentRefresh process if a timeout occurs.
        // This will effectively be sending a request to the silent-refresh.html url every 20sec
        // (default for silentRefreshTimeout option) untill a refresh is successful.
        this.oauthService.setupAutomaticSilentRefresh();
      });

    this.oauthService.events
      .pipe(
        filter(
          (event) =>
            event instanceof OAuthErrorEvent &&
            ((event.type === 'silent_refresh_error' &&
              event.reason instanceof OAuthErrorEvent &&
              event.reason.type === 'code_error' &&
              (<any>event.reason.params)?.error === 'login_required') ||
              (event.type === 'user_profile_load_error' &&
                event.reason instanceof HttpErrorResponse &&
                event.reason.status === 401))
        ),
        debounceUntilVisible()
      )
      .subscribe(() => {
        initiateCodeFlowWithReturnUrl(window, this.oauthService);
      });

    return this.oauthService
      .loadDiscoveryDocument()
      .then(() =>
        this.oauthService.tryLogin().catch(() => {
          // In rare ocasions (being logged in and typing the url slowly and waiting 5-10 sec before enter)
          // the tryLogin will return a rejected promise with a 'Validating access_token failed, wrong state/nonce.'
          // error in the validateNonce method in this file:
          // https://github.com/manfredsteyer/angular-oauth2-oidc/blob/master/projects/lib/src/oauth-service.ts
          // It looks as if the prerendered page's sessionStorage is not available to the main window
          // and the nonce is missing, leading to the failure.
          return initiateCodeFlowWithReturnUrl(window, this.oauthService);
        })
      )
      .then((_) => {
        if (this.oauthService.hasValidIdToken() && this.oauthService.hasValidAccessToken()) {
          return;
        }

        return initiateCodeFlowWithReturnUrl(window, this.oauthService);
      });
  }
}

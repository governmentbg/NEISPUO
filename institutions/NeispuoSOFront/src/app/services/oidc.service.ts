import { Injectable } from "@angular/core";
import * as Oidc from "oidc-client";
import { UserManager } from "oidc-client";
import { environment } from "../../environments/environment";

// if (!environment.production) {
//   Oidc.Log.logger = console;
//   Oidc.Log.level = Oidc.Log.DEBUG;
// }

@Injectable({
  providedIn: "root"
})
export class OIDCService {
  authState = null;

  userManager = new UserManager({
    authority: environment.oidcBaseUrl,
    client_id: "inst_basic",  // TEST: "inst_basic", Staging: "inst_basic_staging"
    redirect_uri: `${environment.thisAppUrl}/signin-callback`,
    silent_redirect_uri: `${environment.thisAppUrl}/silent-signin-callback`,
    post_logout_redirect_uri: `${environment.thisAppUrl}/home`,
    response_type: "code",
    scope: "openid offline_access",
    monitorSession: true,
    checkSessionInterval: 5000,
    stopCheckSessionOnError: false,
    automaticSilentRenew: true
  });

  async start() {
    // // if uncommented => uncoment home.component.ts subscription
    // await this.userManager.removeUser(); // clear local session to prevent OP rejection when another RP has logged in with different account
    // await this.userManager.signinSilent(); // auto-login using iframe

    // update state when user changes
    this.userManager.events.addUserLoaded(v => (this.authState = { profile: v.profile, id_token: v.id_token }));
    this.userManager.events.addUserUnloaded(() => (this.authState = null));
    this.userManager.events.addUserSignedOut(() => this.userManager.removeUser());
  }
}

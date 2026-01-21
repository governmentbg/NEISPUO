import { UserManager
//  , WebStorageStateStore
} from 'oidc-client';
import { SecureStorageStateStore } from '@/auth/SecureStorageStateStore';
import { config } from '@/common/config';
import store from '@/store/index';

const settings = {
    userStore: new SecureStorageStateStore({ store: window.localStorage }),
    authority: config.stsDomain,
    client_id: 'students_code_client',
    redirect_uri: config.ownUrl + '/callback.html',
    automaticSilentRenew: true,
    silent_redirect_uri: config.ownUrl + '/silent-renew.html',
    response_type: 'code',
    scope: 'openid profile roles selected_role offline_access',
    post_logout_redirect_uri: config.ownUrl,
    filterProtocolClaims: true,
    checkSessionInterval: 30000,
    monitorSession: true
};
var userManager = new UserManager(settings);

userManager.events.addUserLoaded(function (user) {
  if(config.mode !== 'prod') {
    console.log('New User Loaded：', arguments);
  }

  store.dispatch('setUser', user);
});

userManager.events.addAccessTokenExpiring(function () {
  if(config.mode !== 'prod') {
    console.log('AccessToken Expiring：', arguments);
  }
});

userManager.events.addAccessTokenExpired(function () {
  if(config.mode !== 'prod') {
    console.log('AccessToken Expired：', arguments);
  }
});

userManager.events.addSilentRenewError(function () {
  if(config.mode !== 'prod') {
    console.error('Silent Renew Error：', arguments);
  }
});

// Мисля, че не трябва да се използва. Постоянно ще редиректва към изход rmnikolov
// userManager.events.addUserSignedOut(function () {
//     console.log('UserSignedOut：', arguments);

//     userManager.signoutRedirect().then(function (resp) {
//         console.log('signed out', resp);
//     }).catch(function (err) {
//         console.log(err);
//     });
// });
export default class AuthService {

    getUser() {
        return userManager.getUser();
    }

    login(redirect) {
      return userManager.signinRedirect({state:{redirect: redirect}}).catch((err) => {
            console.log(err.response);
        });
    }

    logout() {
        console.log('Logout...');
        return userManager.signoutRedirect().then((resp) => {
            store.dispatch('clearPermissions');
            console.log('signed out', resp);
        }).catch(function (err) {
            console.log(err);
        });
    }

    getAccessToken() {
        return userManager.getUser().then((data) => {
            return data.access_token;
        });
    }

    getIdToken() {
        return userManager.getUser().then((data) => {
            return data.id_token;
        });
    }

    // Get the session state
    getSessionState() {
        let self = this;
        return new Promise((resolve, reject) => {
            userManager.getUser().then(function (user) {
                if (user == null) {
                    self.login();
                    return resolve(null);
                } else {
                    return resolve(user.session_state);
                }
            }).catch(function (err) {
                console.log(err);
                return reject(err);
            });
        });
    }

    // Takes the scopes of the logged in user
    getScopes() {
        let self = this;
        return new Promise((resolve, reject) => {
            userManager.getUser().then(function (user) {
                if (user == null) {
                    self.login();
                    return resolve(null);
                } else {
                    return resolve(user.scopes);
                }
            }).catch(function (err) {
                console.log(err);
                return reject(err);
            });
        });
    }

    // Renew the token manually
    renewToken() {
        let self = this;
        return new Promise((resolve, reject) => {
            userManager.signinSilent().then(function (user) {
                if (user == null) {
                    self.signIn(null);
                } else {
                    return resolve(user);
                }
            }).catch(function (err) {
                console.log(err);
                return reject(err);
            });
        });
    }

    // Get the user roles logged in
    getRole() {
        let self = this;
        return new Promise((resolve, reject) => {
            userManager.getUser().then(function (user) {
                if (user == null) {
                    self.login();
                    return resolve(null);
                } else {
                    return resolve(user.profile.role);
                }
            }).catch(function (err) {
                console.log(err);
                return reject(err);
            });
        });
    }
}

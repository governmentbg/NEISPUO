// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  CUBEJS_API_TOKEN: '',
  CUBEJS_API_URL: 'http://localhost:4000/cubejs-api/v1',
  APP_URL: 'http://localhost:4201',
  BACKEND_URL: 'http://localhost:3004',
  USER_MANAGMENT_BACKEND_URL: 'http://localhost:3005',
  OIDC_BASE_URL: 'http://localhost:3000',
  OIDC_CLIENT_ID: 'neispuo-portal',
  HELPDESK_URL: 'https://helpdesk-neispuo.mon.bg/',
  ALLOWED_DOMAINS: 'localhost:3004,localhost:3005',
  // DISALLOWED_ROUTES: 'http://localhost:3000',
  SECONDS_TO_CHECK_VERSION: 300,
  MAIN_PORTAL_URL: 'http://localhost:4200'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.

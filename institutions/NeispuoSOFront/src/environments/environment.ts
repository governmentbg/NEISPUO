// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  apiUrl: "https://10.10.4.40:4443", // TEST: "http://10.10.4.40:4443" , STAGING: "http://10.10.4.40:9999"
  oidcBaseUrl: "http://10.10.4.40:3000",
  azureUrl: "https://test-usermng-server.mon.bg",
  thisAppUrl: "http://10.10.4.40", // DEV: "http://localhost:4200" , TEST: "http://10.10.4.40" , STAGING: "http://10.10.4.40:9080"
  blobsMon: "https://dev-blobs.mon.bg",
  unauthorizedRedirectUrl: "unauthorized", // thisAppUrl appended automatically
  localServer: "http://127.0.0.1:5339",
  ipUrl: "http://api.ipify.org/?format=json",
  portalUrl: "https://test-portal.mon.bg/login",
  regixUrl: "https://10.10.4.40:7143",
  production: false
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.

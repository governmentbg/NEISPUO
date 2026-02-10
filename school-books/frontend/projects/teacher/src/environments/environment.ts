// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  envType: 'development',
  apiBasePath: 'http://localhost:5001',
  authServerPath: 'http://localhost:3000',
  authRequireHttps: 'FALSE',
  docsUrl: 'https://dev-docs-dnevnik.mon.bg/',
  releaseNotesUrl: 'https://dev-docs-dnevnik.mon.bg/release-notes/',
  blobServerPath: 'http://localhost:5100',
  publicationMaxFileSizeInBytes: 50 * 1024 * 1024, // 50MB
  topicPlanImportMaxFileSizeInBytes: 50 * 1024 * 1024, // 50MB
  signingServerPath: 'http://localhost:5339',
  signingServerPageUrl: 'https://students.mon.bg/software/localServer',
  teachersAppUrl: 'http://localhost:4200/t/',
  studentsAppUrl: 'http://localhost:4201/s/',
  kontraxStudentsUrl: 'https://dev-students.mon.bg/',
  vapidPubKey: 'BD0bd45v26nHYo39Aa3Ayv3mvR4bsUbqhQPei8eLOm_qd_h8M57bXiNHniVlRQCM0TvjzFbQGCcZ9llBsd82-eI',
  appVersion: '1.0.0'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.

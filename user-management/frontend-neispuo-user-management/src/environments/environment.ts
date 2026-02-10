// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    APP_URL: 'http://localhost:4205',
    BACKEND_URL: 'http://localhost:3005',
    MAIN_PORTAL_URL: 'http://localhost:4200',
    OIDC_BASE_URL: 'http://localhost:3000',
    OIDC_CLIENT_ID: 'user-management',
    ALLOWED_DOMAINS: 'localhost:3005',
    DISALLOWED_ROUTES: 'http://localhost:3000',
    HELPDESK_URL: 'https://helpdesk-neispuo.mon.bg',
    IS_IMPERSONATION_ENABLED: false,
    IS_SCHOOL_BOOK_ACCESS_ENABLED: false,
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.

export const environment = {
  production: true,
  envType: '$SB__FRONTEND__ENV',
  apiBasePath: '$SB__FRONTEND__API__BASE__PATH',
  authServerPath: '$SB__FRONTEND__AUTH__SERVER__PATH',
  authRequireHttps: '$SB__FRONTEND__AUTH__REQUIRE__HTTPS',
  docsUrl: '$SB__FRONTEND__DOCS__URL',
  releaseNotesUrl: '$SB__FRONTEND__RELEASE__NOTES__URL',
  teachersAppUrl: '$SB__FRONTEND__TEACHERS__APP__URL',
  studentsAppUrl: '$SB__FRONTEND__STUDENTS__APP__URL',
  appVersion: '$SB__FRONTEND__APP__VERSION',
  vapidPubKey: '$SB__FRONTEND__VAPID__PUBLIC__KEY',
  cacheBuster: 1 //TODO: Remove this in future
};

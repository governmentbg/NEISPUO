export const environment = {
  production: true,
  envType: '$SB__FRONTEND__ENV',
  apiBasePath: '$SB__FRONTEND__API__BASE__PATH',
  authServerPath: '$SB__FRONTEND__AUTH__SERVER__PATH',
  authRequireHttps: '$SB__FRONTEND__AUTH__REQUIRE__HTTPS',
  docsUrl: '$SB__FRONTEND__DOCS__URL',
  releaseNotesUrl: '$SB__FRONTEND__RELEASE__NOTES__URL',
  blobServerPath: '$SB__FRONTEND__BLOB__SERVER__PATH',
  publicationMaxFileSizeInBytes: 50 * 1024 * 1024, // 50MB
  topicPlanImportMaxFileSizeInBytes: 50 * 1024 * 1024, // 50MB
  signingServerPath: '$SB__FRONTEND__SIGNING__SERVER__PATH',
  signingServerPageUrl: '$SB__FRONTEND__SIGNING__PAGE__URL',
  teachersAppUrl: '$SB__FRONTEND__TEACHERS__APP__URL',
  studentsAppUrl: '$SB__FRONTEND__STUDENTS__APP__URL',
  kontraxStudentsUrl: '$SB__FRONTEND__KONTRAX__STUDENTS__URL',
  appVersion: '$SB__FRONTEND__APP__VERSION',
  vapidPubKey: '$SB__FRONTEND__VAPID__PUBLIC__KEY',
  cacheBuster: 1 //TODO: Remove this in future
};

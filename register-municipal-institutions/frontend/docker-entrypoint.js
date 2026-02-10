const fs = require('fs');
const config = {
  production: process.env.BUILD_ENV === 'production' ? true : false,
  APP_URL: process.env.APP_URL,
  MAIN_PORTAL_URL: process.env.MAIN_PORTAL_URL,
  BACKEND_URL: process.env.BACKEND_URL,
  OIDC_BASE_URL: process.env.OIDC_BASE_URL,
  OIDC_CLIENT_ID: process.env.OIDC_CLIENT_ID,
  BLOB_SERVER_URL: process.env.BLOB_SERVER_URL,
  ALLOWED_DOMAINS: process.env.ALLOWED_DOMAINS,
  DISALLOWED_ROUTES: process.env.DISALLOWED_ROUTES,
  HELPDESK_URL: process.env.HELPDESK_URL
};

fs.writeFileSync('src/assets/config/config.json', JSON.stringify(config));

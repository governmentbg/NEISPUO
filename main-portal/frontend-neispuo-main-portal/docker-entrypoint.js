const fs = require('fs');
const config = {
  production: process.env.BUILD_ENV === 'production' ? true : false,
  APP_URL: process.env.APP_URL,
  BACKEND_URL: process.env.BACKEND_URL,
  USER_MANAGMENT_BACKEND_URL: process.env.USER_MANAGMENT_BACKEND_URL,
  OIDC_BASE_URL: process.env.OIDC_BASE_URL,
  OIDC_CLIENT_ID: process.env.OIDC_CLIENT_ID,
  HELP_DESK_URL: process.env.HELP_DESK_URL,
  ALLOWED_DOMAINS: process.env.ALLOWED_DOMAINS,
  DISALLOWED_ROUTES: process.env.DISALLOWED_ROUTES,
  SECONDS_TO_CHECK_VERSION: 300
};

fs.writeFileSync('src/assets/config/config.json', JSON.stringify(config));

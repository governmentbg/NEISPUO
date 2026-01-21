const fs = require('fs');
const config = {
    production: process.env.BUILD_ENV === 'production' ? true : false,
    APP_URL: process.env.APP_URL,
    BACKEND_URL: process.env.BACKEND_URL,
    MAIN_PORTAL_URL: process.env.MAIN_PORTAL_URL,
    OIDC_BASE_URL: process.env.OIDC_BASE_URL,
    OIDC_CLIENT_ID: process.env.OIDC_CLIENT_ID,
    ALLOWED_DOMAINS: process.env.ALLOWED_DOMAINS,
    DISALLOWED_ROUTES: process.env.DISALLOWED_ROUTES,
    HELPDESK_URL: process.env.HELPDESK_URL,
    IS_IMPERSONATION_ENABLED: process.env.IS_IMPERSONATION_ENABLED,
    IS_SCHOOL_BOOK_ACCESS_ENABLED: process.env.IS_SCHOOL_BOOK_ACCESS_ENABLED,
};

fs.writeFileSync('src/assets/config/config.json', JSON.stringify(config));

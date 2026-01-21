export const environment = {
  production: true,
  APP_URL: 'http://test-assessment.int',
  BACKEND_URL: 'http://test-assessmet-server.int',
  OIDC_BASE_URL: 'https://test-oidc.int',
  OIDC_CLIENT_ID: 'survey',
  ALLOWED_DOMAINS: 'test-assessmet-server.int', // comma separated, NO protocol on domain (no http/s)
  DISALLOWED_ROUTES: 'https://test-oidc.int' // comma separated, include protocol (with http/s)
};

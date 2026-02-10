/* eslint-disable node/no-missing-require */
/* eslint-disable import/extensions */
/* eslint-disable import/no-unresolved */
/* eslint-disable no-use-before-define */
const fs = require('fs');
const path = require('path');
const ejs = require('ejs');
const {
  interactionPolicy: { base },
} = require('../../lib'); // require('oidc-provider');
const htmlSafe = require('../../lib/helpers/html_safe');
const Account = require('./account');

const jwksKeys =
  process.env.NODE_ENV === 'development' ? require('./jwks-dev') : require('./jwks-prod');

const cookieKeys =
  process.env.NODE_ENV === 'development'
    ? require('./cookie-keys-dev')
    : require('./cookie-keys-prod');

const clients = JSON.parse(
  fs.readFileSync(path.resolve(__dirname, './oidc-client-apps.config.json')),
);

const header = fs.readFileSync(path.join(__dirname, '../views/header.ejs'), 'ascii');

// copies the default policy, already has login and consent prompt policies
const policy = base();
policy.remove('consent'); // remove consent Prompt from it

const selectRole = require('./selectRole');

policy.add(selectRole);

const JWT_EXPIRATION_IN_S = +process.env.JWT_EXPIRATION_IN_S || 900;
module.exports = {
  clients,
  interactions: {
    policy,
    async url(ctx, interaction) {
      if (!(await Account.sessionExists(ctx.oidc.uid)))
        Account.createAudit({
          LoginSessionId: ctx.oidc.uid,
          DateUtc: new Date(),
          Action: 'SESSION_CREATE',
          AuditModuleId: 401,
          RemoteIpAddress: ctx.request.ip,
          UserAgent: ctx.request.get('User-Agent'),
          Data: '{}',
        });

      return `/interaction/${ctx.oidc.uid}`;
    },
  },
  cookies: {
    long: { signed: true, maxAge: 1 * 24 * 60 * 60 * 1000 }, // 1 day in ms
    /** Be aware that short cookie configuration is used by many oidc functions including interaction policies: https://github.com/panva/node-oidc-provider/issues/680 */
    short: { signed: true, maxAge: JWT_EXPIRATION_IN_S * 1000 },
    keys: cookieKeys,
  },
  claims: {
    openid: [
      'sub',
      'FirstName',
      'LastName',
      'roles',
      'selected_role',
      'sessionID',
      'impersonator',
      'impersonatorSysUserID',
    ],
  },
  /** Include same claims from id_token in access_token */
  extraAccessTokenClaims: async function extraAccessTokenClaims(ctx, token) {
    const acc = await Account.findAccount(ctx, token.accountId, token);
    const claims = await acc.claims('id_token');
    return {
      ...claims,
      sessionID: token.sessionUid,
      impersonator: acc?.profile?.impersonator || null,
      impersonatorSysUserID: acc?.profile?.impersonatorSysUserID || null,
    };
  },
  features: {
    devInteractions: { enabled: false }, // defaults to true
    sessionManagement: { enabled: true }, // defaults to false
    introspection: { enabled: true }, // defaults to false
    revocation: { enabled: true }, // defaults to false
    rpInitiatedLogout: {
      // https://github.com/panva/node-oidc-provider/blob/main/docs/README.md#featuresrpinitiatedlogout
      enabled: true,
      logoutSource,
    },
  },
  formats: {
    AccessToken: 'jwt',
  },
  jwks: {
    keys: jwksKeys,
  },
  ttl: {
    AccessToken: JWT_EXPIRATION_IN_S,
    AuthorizationCode: 10 * 60, // 10 minutes in seconds
    IdToken: JWT_EXPIRATION_IN_S,
    DeviceCode: 10 * 60, // 10 minutes in seconds
    RefreshToken: 1 * 24 * 60 * 60, // 1 day in seconds
  },
  renderError,
};

async function renderError(ctx, out, error) {
  ctx.type = 'html';
  ctx.body = `<!DOCTYPE html>
    <head>
      <title>Възникна грешка</title>
      <!-- Consider moving out of cdn -->
      <link rel="stylesheet" href="/css/neispuo-theme-styles/theme.min.css" />
      <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500" rel="stylesheet" />
      <link rel="stylesheet" type="text/css" href="/css/error_page.min.css" />
    </head>
    <body>
      <div class="error-page container-fluid">
      ${ejs.render(header)}
        <div class="error-page-content card">
          <div class="card-body">
            <p class="h1 mb-6">${out.title || 'Възникна грешка'}</p>
            <p>
            ${
              out.summary ||
              'Ако продължавате да виждате тази грешка, моля свържете с администратор.'
            }  
            </p>
            <p class="mb-4"> Натиснете <a href="${
              process.env.MAIN_PORTAL_URL
            }">тук</a>, за да се върнете към основния портал.</p>
            <div class="text-bottom pt-4">
              <p> <strong>error</strong>: ${htmlSafe(out.error)} </p>
              <p> <strong>error_description</strong>: ${htmlSafe(out.error_description)} 
              </p>
            </div>
          </div>
        </div>
      </div>

    </body>
    </html>`;
  return ctx.body;
}

async function logoutSource(ctx, form) {
  // @param ctx - koa request context
  // @param form - form source (id="op.logoutForm") to be embedded in the page and submitted by
  //   the End-User
  ctx.body = `<!DOCTYPE html>
    <head>
      <meta charset="utf-8">
      <title>НЕИСПУО - Изход</title>
      <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
      <meta http-equiv="x-ua-compatible" content="ie=edge">
      <!-- Consider moving out of cdn -->
      <link rel="stylesheet" href="/css/neispuo-theme-styles/theme.min.css" />
      <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500" rel="stylesheet">
      <link rel="stylesheet" type="text/css" href="/css/logout_page.min.css" />
      <script src="/js/logout.js"> </script>
    </head>
    <body>
      <div class="logout-page container-fluid">
        ${ejs.render(header)}
        <div class="logout-page-content card">
          <div class="card-body">
            <p class="h1 mb-6">Изход</p>
            <p class="h3 mb-4">С това действие ще излезете от следните платформи:</p>
            <div class="platform-wrapper">
              <div>dnevnik.mon.bg</div>
              <div>institutions.mon.bg</div>
              <div>neispuo.mon.bg</div>
              <div>regdiploms.mon.bg</div>
              <div>reporting.mon.bg</div>
              <div>ri.mon.bg</div>
              <div>rmi.mon.bg</div>
              <div>so.mon.bg</div>
              <div>students.mon.bg</div>
              <div>survey.mon.bg</div>
              <div>usermng.mon.bg</div>
            </div>
            <p class="h3 mt-4">Желаете ли да продължите?</p>
            <div class="card-footer">
              ${form}
              <button
                class="btn btn-primary me-3"
                type="submit"
                autofocus
                type="submit"
                form="op.logoutForm"
                value="yes"
                name="logout"
              >
                Да, излез
              </button>
      
              <button id="cancelLogout" class="btn btn-outline-primary" type="button">
                НЕ, назад
              </button>
            </div>
          </div>
        </div>
      </div>
    </body>
    </html>`;
}

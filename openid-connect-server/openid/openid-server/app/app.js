/* eslint-disable no-underscore-dangle */
require('dotenv').config();

const fs = require('fs');
const path = require('path');
const url = require('url');

const set = require('lodash/set');
const express = require('express');
const helmet = require('helmet');

const { Provider } = require('../lib'); // require('oidc-provider');

const adapter = process.env.REDIS_ON === 'true' ? require('./adapters/redis') : undefined;
const Account = require('./support/account');
const configuration = require('./support/configuration');
const routes = require('./routes/express');

const siemLogger = require('./integrations/siem-integration');

const { PORT = 3000, ROOT_URI } = process.env;
configuration.findAccount = Account.findAccount;

const app = express();

const clients = JSON.parse(
  fs.readFileSync(path.resolve(__dirname, './support/oidc-client-apps.config.json')),
);
const clientHosts = clients.map((c) => new URL(c.redirect_uris[0]).origin);

app.use(
  helmet({
    originAgentCluster: false,
    crossOriginEmbedderPolicy: false,
    referrerPolicy: { policy: 'strict-origin' },
    contentSecurityPolicy: {
      directives: {
        /**
         * SHA256 hashes
         *
         * The SHA256 configuration is added for Cloudflare rocket loader script - sha256-NuF9mPEA+1gRp+HtOtdJgJHdzeLTSFCIKJnBJbvpSuM=
         * The SHA256 configuration is added for Recaptcha - sha256-QV8WjEUiXJyYK+qgbcwPpfgWIlJH7bMfSuT8XsIP0dc, sha256-Cb38MeezfThVHA8mtsJsW3L49+Nfg5MJQi+ogfDwntU
         * The SHA256 configuration is added for check session script - sha256-AMFl99TLzyWiCl6tjxNl12HlSNoJJxIYbboSpDKGzg4=
         * The https://cdnjs.cloudflare.com/polyfill/v3/polyfill.min.js host configuration is added for Session checks
         *
         */
        'script-src': [
          "'self'",
          "'unsafe-inline'",
          'https://cdnjs.cloudflare.com/ajax/libs/jsSHA/2.3.1/sha256.js',
          'https://www.google.com/recaptcha/api.js',
          'https://www.gstatic.com/recaptcha/',
          'https://cdnjs.cloudflare.com/polyfill/v3/polyfill.min.js',
        ],
        'script-src-attr': null,
        'script-src-elem': null,
        'style-src': ["'self'", 'https://fonts.googleapis.com', "'unsafe-inline'"],
        'font-src': ["'self'", 'https://fonts.gstatic.com'],
        'connect-src': ["'self'"],
        'form-action': [
          "'self'",
          'https://login.microsoftonline.com',
          'https://*.b2clogin.com',
          ...clientHosts,
        ],
        'frame-ancestors': ["'self'", ...clientHosts],
        'frame-src': ['https://www.google.com'],
      },
    },
  }),
);

app.set('siemLogger', siemLogger);

app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');
app.use(express.static(path.join(__dirname, 'views', 'dist')));

let server;
(async () => {
  const prod = process.env.NODE_ENV === 'production';

  if (prod) {
    set(configuration, 'cookies.short.secure', true);
    set(configuration, 'cookies.long.secure', true);
  }

  const provider = new Provider(ROOT_URI, { adapter, ...configuration });
  provider.Session.prototype.promptedScopesFor = () => new Set(['openid', 'offline_access']); // Remove this if implementing consent for third party RPs. Also adjust configuration.
  provider.Session.prototype.getSelectedRole = function getSelectedRole() {
    // meta.selected_role should be one and the same, regardless of client
    const authorizations = Object.values(this.authorizations);
    let selectedRoleConcatID;
    for (let i = 0; i < authorizations.length; i += 1) {
      // linter is against i++
      const authorization = authorizations[i];
      selectedRoleConcatID =
        authorization && authorization.meta && authorization.meta.selected_role;
      if (selectedRoleConcatID) {
        break;
      }
    }

    return selectedRoleConcatID;
  };

  provider.use(async (ctx, next) => {
    // https://github.com/panva/node-oidc-provider/blob/main/docs/README.md#pre--and-post-middlewares

    await next();

    if (ctx?.oidc && ctx?.oidc?.route === 'end_session_confirm') {
      const account = await Account.findAccount(
        ctx,
        ctx.oidc.session.account,
        ctx.oidc.accessToken,
      );

      if (account?.profile) {
        await Account.createAudit({
          SysUserID: account.profile.SysUserID,
          SysRoleID: account.profile.roles.find((r) => r._concatID === account.selected_role)
            ?.SysRoleID,
          PersonID:
            account.profile.PersonID.length > 0
              ? account.profile.PersonID[0]
              : account.profile.PersonID,
          Username: account.accountId,
          LoginSessionId: ctx.oidc.session.uid,
          DateUtc: new Date(),
          Action: 'LOGOUT',
          AuditModuleId: 401,
          RemoteIpAddress: ctx.request.ip,
          UserAgent: ctx.request.get('User-Agent'),
          Data: '{}',
        });
      }
    }
  });

  if (prod) {
    app.enable('trust proxy');
    provider.proxy = true;

    app.use((req, res, next) => {
      if (req.secure) {
        next();
      } else if (req.method === 'GET' || req.method === 'HEAD') {
        res.redirect(
          url.format({
            protocol: 'https',
            host: req.get('host'),
            pathname: req.originalUrl,
          }),
        );
      } else {
        res.status(400).json({
          error: 'invalid_request',
          error_description: 'connection is not https',
        });
      }
    });
  }

  routes(app, provider);
  app.use(provider.callback);

  server = app.listen(PORT, () => {
    console.log(
      `application is listening on port ${PORT}, check its /.well-known/openid-configuration`,
    );
  });
})().catch((err) => {
  if (server && server.listening) server.close();
  console.error(err);
  process.exitCode = 1;
});

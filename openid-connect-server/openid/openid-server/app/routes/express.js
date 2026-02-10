/* eslint-disable consistent-return */
/* eslint-disable no-underscore-dangle */
/* eslint-disable camelcase */
/* eslint-disable import/no-extraneous-dependencies */
const { strict: assert } = require('assert');
const querystring = require('querystring');
const { inspect } = require('util');

const { generators } = require('openid-client');
const isEmpty = require('lodash/isEmpty');
const { urlencoded } = require('express');

const fetch = require('node-fetch');
const Account = require('../support/account');
const azureIntegration = require('../integrations/azure-integration');
const recaptchaIntegration = require('../integrations/recaptcha-integration');
const impersonateRoute = require('../support/impersonate/impersonate.route');
const ssHandler = require('../../lib/helpers/samesite_handler');
const endImpersonate = require('../support/impersonate/end-impersonate.route');
const instance = require('../../lib/helpers/weak_cache');

const pjson = require('../../package.json');

const {
  NeispuoAccessDeniedError,
  NeispuoAccountNotSynchronized,
} = require('../support/custom-errors');

const azureParentIntegration = require('../integrations/azure-parent-integration');
const parentLoginCallbackRoute = require('../support/internal-user-login/parents/routes/parent-login-callback.route');
const parentLoginInteractionRoute = require('../support/internal-user-login/parents/routes/parent-login-interaction.route');

const body = urlencoded({ extended: false });

const keys = new Set();
const debug = (obj) =>
  querystring.stringify(
    Object.entries(obj).reduce((acc, [key, value]) => {
      keys.add(key);
      if (isEmpty(value)) return acc;
      acc[key] = inspect(value, { depth: null });
      return acc;
    }, {}),
    '<br/>',
    ': ',
    {
      encodeURIComponent(value) {
        return keys.has(value) ? `<strong>${value}</strong>` : value;
      },
    },
  );

const CHECK_RECAPTCHA = process.env.RECAPTCHA_ON === 'true';

module.exports = (app, provider) => {
  const {
    constructor: {
      errors: { SessionNotFound },
    },
  } = provider;

  app.use((req, res, next) => {
    const orig = res.render;
    // you'll probably want to use a full blown render engine capable of layouts
    res.render = (view, locals) => {
      app.render(view, locals, (err, html) => {
        if (err) throw err;
        orig.call(res, '_layout', {
          ...locals,
          body: html,
        });
      });
    };
    next();
  });

  function setNoCache(req, res, next) {
    res.set('Pragma', 'no-cache');
    res.set('Cache-Control', 'no-cache, no-store');
    next();
  }

  app.get('/v1/version', async (req, res, next) => {
    res.status(200).send({ name: pjson.name, version: pjson.version });
  });

  app.get('/interaction/:uid/local-login', async (req, res, next) => {
    try {
      /**
       * The IP check has been commented out since the IPs in the cluster are always received
       * with the wrong internal IP value which makes it impossible to correctly whitelist IPs
       *       if (process.env.LOCAL_LOGIN_WHITELIST?.split(',').indexOf(req.ip) === -1)
       */

      if (process.env.APP_ENV === 'prod')
        return res.redirect(`/interaction/${req.params.uid}?error=forbidden`);
      const { uid, prompt, params, session } = await provider.interactionDetails(req, res);

      return res.render('local_login', {
        uid,
        details: prompt.details,
        CHECK_RECAPTCHA,
        recaptchaSiteKey: process.env.RECAPTCHA_SITE_KEY,
        params,
        title: 'Local Sign-in',
        session: session ? debug(session) : undefined,
        dbg: {
          params: debug(params),
          prompt: debug(prompt),
        },
      });
    } catch (err) {
      return next(err);
    }
  });

  app.get('/interaction/:uid', setNoCache, async (req, res, next) => {
    try {
      const { uid, prompt, params, session } = await provider.interactionDetails(req, res);

      const client = await provider.Client.find(params.client_id);

      switch (prompt.name) {
        case 'login': {
          return res.render('login', {
            r_client: true,
            uid,
            details: prompt.details,
            CHECK_RECAPTCHA,
            recaptchaSiteKey: process.env.RECAPTCHA_SITE_KEY,
            params,
            title: 'Sign-in',
            session: session ? debug(session) : undefined,
            dbg: {
              params: debug(params),
              prompt: debug(prompt),
            },
            MAIN_PORTAL_URL: process.env.MAIN_PORTAL_URL,
          });
        }
        case 'select_role': {
          return res.render('select_role', {
            r_client: true,
            uid,
            details: prompt.details,
            params,
            title: 'Моля изберете роля',
            session: session ? debug(session) : undefined,
            dbg: {
              params: debug(params),
              prompt: debug(prompt),
            },
          });
        }
        default:
          return undefined;
      }
    } catch (err) {
      return next(err);
    }
  });

  app.post('/interaction/:uid/login', setNoCache, body, async (req, res, next) => {
    try {
      const {
        prompt: { name },
      } = await provider.interactionDetails(req, res);
      assert.strictEqual(name, 'login');

      const isRecaptchaValid =
        CHECK_RECAPTCHA &&
        (await recaptchaIntegration.validateRecaptcha(req.body['g-recaptcha-response']));
      if (CHECK_RECAPTCHA && !isRecaptchaValid) {
        return res.redirect(`/interaction/${req.params.uid}?error=invalid_recaptcha`);
      }

      const account = req.body.login && (await Account.findAccount(null, req.body.login, null));
      const isLocalUser = account && account.profile && !account.profile.IsAzureUser;
      const passwordValid = isLocalUser && (await account.validatePassword(req.body.password));
      if (!passwordValid) {
        return res.redirect(`/interaction/${req.params.uid}?error=invalid_username_or_password`);
      }

      const result = {
        login: {
          account: account.accountId,
        },
        /** Auto select role when only one role is present */
        meta: {
          selected_role:
            account.profile.roles.length === 1 ? account.profile.roles[0]._concatID : undefined,
        },
      };

      await provider.interactionFinished(req, res, result, { mergeWithLastSubmission: false });
    } catch (err) {
      next(err);
    }
  });

  app.post('/interaction/:uid/login-with-azure', setNoCache, body, async (req, res, next) => {
    try {
      const client = await azureIntegration;
      const code_verifier = generators.codeVerifier();
      const code_challenge = generators.codeChallenge(code_verifier);

      const interactionDetails = await provider.interactionDetails(req, res);
      const { uid, adapter } = interactionDetails;
      await adapter.upsert(
        `oidc-azure-challenge:${uid}`,
        code_verifier,
        +process.env.AZURE_CHALLENGE_EXPIRATION,
      );

      const cookieOptions = instance(provider).configuration('cookies.short');
      const ctx = provider.app.createContext(req, res);
      ssHandler.set(ctx.cookies, provider.cookieName('interaction'), uid, {
        ...cookieOptions,
        httpOnly: true,
        sameSite: false, // needed as azure does additional requests behind the scenes when client has multiple accounts
      });

      const authUrl = client.authorizationUrl({
        scope: 'openid profile email',
        code_challenge,
        code_challenge_method: 'S256',
        prompt: 'select_account',
        domain_hint: 'edu.mon.bg',
      });
      return res.redirect(authUrl);
    } catch (err) {
      next(err);
    }
  });

  app.get('/azure-integration-callback', async (req, res, next) => {
    try {
      const { uid, adapter } = await provider.interactionDetails(req, res);
      const code_verifier = await adapter.find(`oidc-azure-challenge:${uid}`);

      const client = await azureIntegration;
      const params = client.callbackParams(req);
      const tokenSet = await client.callback(
        `${process.env.ROOT_URI}/azure-integration-callback`,
        params,
        {
          code_verifier,
        },
      );

      const azureProfile = await (
        await fetch('https://graph.microsoft.com/beta/me', {
          headers: { Authorization: `Bearer ${tokenSet.access_token}` },
        })
      ).json();
      const hasNeispuoAccess =
        azureProfile.extension_3fd591523047404483834154c3498778_HAS_NEISPUO_ACCESS || false;
      const hasExternalRole = Account.hasExternalRole(azureProfile);
      const isAzureAllowedAccountType =
        Account.isStudentOrTeacher(azureProfile) ||
        Account.isSchool(azureProfile) ||
        hasExternalRole;

      if (hasExternalRole && !hasNeispuoAccess) {
        throw new NeispuoAccessDeniedError(
          `User ${azureProfile.userPrincipalName} has disabled neispuo access.`,
        );
      }

      let account =
        azureProfile.userPrincipalName &&
        (await Account.findAccount(null, azureProfile.userPrincipalName, null));

      /** Account check will allow us to use testing account to authenticate.
       * E.g. test444444@edu.mon.bg user has multiple roles so we want to be able to login and troubleshoot.
       * TO DO: Add role to external account so we can skip multiple queries for account find in DB.
       */

      if (!account && !isAzureAllowedAccountType) {
        throw new NeispuoAccessDeniedError(
          `User ${azureProfile.userPrincipalName} has an invalid account type.`,
        );
        /**
         * TO DO: Need to remove the account check here after updateAccountRole logic is implemented.
         */
      } else if (!account && hasExternalRole) {
        account = await Account.upsertAccount(azureProfile);
      }
      const isLocalUser = account && account.profile && !account.profile.IsAzureUser;

      if (!isLocalUser) {
        const hasChanges = await Account.syncAccountWithAzure(account, azureProfile);
        /* 
          If there are changes between NEISPUO & Azure we need to change the roles array.
          Since there's RegionName, RegionID, _concatID, etc. in the role it's easier to refresh the whole account object.
        */
        account = hasChanges
          ? await Account.findAccount(null, azureProfile.userPrincipalName, null)
          : account;
      }

      const result = isLocalUser
        ? {
            error: 'not_registered_as_azure_user',
            error_description: 'User is not registered as an azure user.',
          }
        : {
            login: {
              account: account.accountId,
            },
            /** Auto select role when only one role is present */
            meta: {
              selected_role:
                account.profile.roles.length === 1 ? account.profile.roles[0]._concatID : undefined,
            },
          };

      // the user has only 1 role and is auto logged in with it
      // that's why we just get his [0] role from his profile
      if (result && result.meta && result.meta.selected_role) {
        const auditLoginResult = (
          await Account.auditLogin(
            account.profile.roles[0],
            // http://expressjs.com/en/4x/api.html#req.ip
            // req.ip should work because 'trust proxy' is set to true
            req.ip,
          )
        )?.[0];

        const siemLogger = req.app.get('siemLogger');
        siemLogger.sendLoginSuccessLog(auditLoginResult);

        const messages = await Account.getSysUserMessages(account.profile.roles[0]);
        // instead of ending interaction, load User messages screen
        if (messages.length > 0) {
          res.render('user_messages', {
            uid,
            messages,
            account: account.accountId,
            selected_role: result.meta.selected_role,
          });
          // do not finish interaction!
          return;
        }
      }
      await provider.interactionFinished(req, res, result, { mergeWithLastSubmission: false });
    } catch (err) {
      next(err);
    }
  });

  app.post('/interaction/:uid/login-as-parent', setNoCache, body, async (req, res, next) => {
    await parentLoginInteractionRoute({
      provider,
      azureParentIntegration,
      generators,
      instance,
      ssHandler,
      req,
      res,
      next,
    });
  });

  app.get('/azure-parent-integration-callback', async (req, res, next) => {
    await parentLoginCallbackRoute({ provider, azureParentIntegration, req, res, next });
  });

  app.post('/interaction/:uid/select_role', setNoCache, body, async (req, res, next) => {
    try {
      const interactionDetails = await provider.interactionDetails(req, res);
      assert.strictEqual(interactionDetails.prompt.name, 'select_role');
      const { selected_role } = req.body;

      if (!selected_role) {
        return res.redirect(`/interaction/${req.params.uid}?error=No role selected.`);
      }

      const result = {
        meta: {
          selected_role,
        },
      };

      const role = interactionDetails.prompt.details.roles.find(
        (r) => r._concatID === selected_role,
      );
      if (role) {
        const auditLoginResult = (
          await Account.auditLogin(
            role,
            // http://expressjs.com/en/4x/api.html#req.ip
            // req.ip should work because 'trust proxy' is set to true
            req.ip,
          )
        )?.[0];

        const siemLogger = req.app.get('siemLogger');
        siemLogger.sendLoginSuccessLog(auditLoginResult);

        const messages = await Account.getSysUserMessages(role);
        if (messages.length > 0) {
          res.render('user_messages', {
            uid: interactionDetails.uid,
            messages,
            account: undefined,
            selected_role: result.meta.selected_role,
          });
          // do not finish interaction!
          return;
        }
      }
      await provider.interactionFinished(req, res, result, { mergeWithLastSubmission: true });
    } catch (err) {
      next(err);
    }
  });

  app.post('/interaction/:uid/successful_login', body, async (req, res, next) => {
    const interactionDetails = await provider.interactionDetails(req, res);

    // if user comes from select_role he does not have accountId
    const result = req.body.account
      ? {
          login: {
            account: req.body.account,
          },
          meta: {
            selected_role: req.body.selected_role,
          },
        }
      : {
          meta: {
            selected_role: req.body.selected_role,
          },
        };

    await provider.interactionFinished(req, res, result, {
      // if user comes from select_role mergeWithLastSubmission should be true
      mergeWithLastSubmission: !req.body.account,
    });
  });

  app.get('/interaction/:uid/select_role/cancel', setNoCache, body, async (req, res, next) => {
    try {
      const interactionDetails = await provider.interactionDetails(req, res);
      assert.strictEqual(interactionDetails.prompt.name, 'select_role');

      const result = {
        // login empty for canceling: https://github.com/panva/node-oidc-provider/blob/main/docs/README.md#user-flows
        login: {},
        // an error field used as error code indicating a failure during the interaction
        error: 'select_role_canceled',
        // an optional description for this error
        error_description: 'User canceled during role selection',
      };

      await provider.interactionFinished(req, res, result, { mergeWithLastSubmission: false });
    } catch (err) {
      next(err);
    }
  });

  app.post('/impersonate', setNoCache, body, async (req, res, next) => {
    await impersonateRoute({ provider, req, res, next });
  });

  app.post('/end_impersonate', setNoCache, body, async (req, res, next) => {
    await endImpersonate({ provider, req, res, next });
  });
  app.use(async (err, req, res, next) => {
    const renderError = instance(provider).configuration('renderError');
    let html;

    if (err instanceof SessionNotFound) {
      html = await renderError(
        {},
        {
          title: 'Невалидна сесия',
          summary: `Вашата сесия е невалидна или изтекла. Моля натиснете <a href="${process.env.MAIN_PORTAL_URL}">тук<a/> и опитайте отново. `,
          error: 'session_not_found',
          error_description: err.error_description,
        },
        err,
      );
    } else if (err instanceof NeispuoAccessDeniedError) {
      html = await renderError(
        {},
        {
          title: 'Забранен достъп до НЕИСПУО',
          summary: `<p>Вашият акаунт няма нужните права за достъп до платформата на НЕИСПУО.</p>
          <p>За повече информация се свържете с вашия администратор.</p>`,
          error: 'forbidden_neispuo_access',
          error_description: 'forbidden_neispuo_access',
        },
        err,
      );
    } else if (err instanceof NeispuoAccountNotSynchronized) {
      html = await renderError(
        {},
        {
          title: 'Забранен достъп до НЕИСПУО',
          summary: `<p>Вашият акаунт е в процес на синхронизация, моля опитайте по-късно.</p>
          <p>В случай, че проблемът продължава повече от ден, моля да се свържете с поддръжка.</p>`,
          error: 'account_not_synchronized',
          error_description: 'account_not_synchronized',
        },
        err,
      );
    } else {
      html = await renderError(
        {},
        {
          title: 'Възникна грешка',
          summary: `
          <p>Моля опитайте отново.</p>
          <p>Ако продължавате да виждате тази грешка повече от 24 часа, моля свържете се с администратор.</p>`,
          error: 'uncaught_error',
          error_description: 'uncaught_error',
        },
        err,
      );
      console.error('Unknown error (unknown_error): ', err, '\n\n', (err && err.message) || '');
    }

    res.set('Content-Type', 'text/html');
    res.send(html);
  });
};

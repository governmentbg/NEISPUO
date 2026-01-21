import path from 'path';
import express from 'express';
import bodyParser from 'body-parser';
import Provider from 'oidc-provider';

// simple account model for this application, user list is defined like so
import Account from './account.mjs';

const oidc = new Provider('http://localhost:3000', {
  clients: [
    {
      client_id: 'school-books-frontend',
      client_secret: 'super-secret-text',
      redirect_uris: ['http://example.com/'], // required to pass config validation
      token_endpoint_auth_method: 'none'
    },
  ],
  features: {
    sessionManagement: { enabled: true },

    // disable the packaged interactions
    devInteractions: { enabled: false }
  },
  formats: {
    AccessToken: 'jwt',
  },

  // oidc-provider only looks up the accounts by their ID when it has to read the claims,
  // passing it our Account model method is sufficient, it should return a Promise that resolves
  // with an object with accountId property and a claims method.
  findAccount: Account.findAccount,

  // let's tell oidc-provider you also support the email scope, which will contain email and
  // email_verified claims
  claims: {
    openid: ['sub', 'FirstName', 'LastName'],
    email: ['email', 'email_verified'],
    profile: ["userid", "role", "instid", "personid"]
  },

  extraAccessTokenClaims: async function extraAccessTokenClaims(ctx, token) {
    var acc = await Account.findAccount(ctx, token.accountId);
    var claims = await acc.claims();
    return claims;
  },

  // let's tell oidc-provider where our own interactions will be
  // setting a nested route is just good practice so that users
  // don't run into weird issues with multiple interactions open
  // at a time.
  interactions: {
    url(ctx) {
      return `/interaction/${ctx.oidc.uid}`;
    },
  },

  ttl: {
    AccessToken: 300, // 5 minutes
    IdToken: 300 // 5 minutes
  }
});

// allow any redirect uri and check session origin for development
oidc.Client.prototype.redirectUriAllowed = () => true;
oidc.Client.prototype.postLogoutRedirectUriAllowed = () => true;
oidc.Client.prototype.checkSessionOriginAllowed = () => true;

// let's work with express here, below is just the interaction definition
const expressApp = express();
expressApp.set('trust proxy', true);
expressApp.set('view engine', 'ejs');
expressApp.set('views', path.resolve('views'));

const parse = bodyParser.urlencoded({ extended: false });

function setNoCache(req, res, next) {
  res.set('Pragma', 'no-cache');
  res.set('Cache-Control', 'no-cache, no-store');
  next();
}

expressApp.get('/interaction/:uid', setNoCache, async (req, res, next) => {
  try {
    const details = await oidc.interactionDetails(req, res);
    console.log('see what else is available to you for interaction views', details);
    const { uid, prompt, params } = details;

    const client = await oidc.Client.find(params.client_id);

    if (prompt.name === 'login') {
      return res.render('login', {
        client,
        uid,
        details: prompt.details,
        params,
        title: 'Sign-in',
        flash: undefined,
      });
    }

    return res.render('interaction', {
      client,
      uid,
      details: prompt.details,
      params,
      title: 'Authorize',
    });
  } catch (err) {
    return next(err);
  }
});

expressApp.post('/interaction/:uid/login', setNoCache, parse, async (req, res, next) => {
  try {
    const { uid, prompt, params } = await oidc.interactionDetails(req, res);
    const client = await oidc.Client.find(params.client_id);

    const accountId = await Account.authenticate(req.body.email, req.body.password);

    if (!accountId) {
      res.render('login', {
        client,
        uid,
        details: prompt.details,
        params: {
          ...params,
          login_hint: req.body.email,
        },
        title: 'Sign-in',
        flash: 'Invalid email or password.',
      });
      return;
    }

    const result = {
      login: {
        account: accountId,
      },
    };

    await oidc.interactionFinished(req, res, result, { mergeWithLastSubmission: false });
  } catch (err) {
    next(err);
  }
});

expressApp.post('/interaction/:uid/confirm', setNoCache, parse, async (req, res, next) => {
  try {
    const result = {
      consent: {
        // rejectedScopes: [], // < uncomment and add rejections here
        // rejectedClaims: [], // < uncomment and add rejections here
      },
    };
    await oidc.interactionFinished(req, res, result, { mergeWithLastSubmission: true });
  } catch (err) {
    next(err);
  }
});

expressApp.get('/interaction/:uid/abort', setNoCache, async (req, res, next) => {
  try {
    const result = {
      error: 'access_denied',
      error_description: 'End-User aborted interaction',
    };
    await oidc.interactionFinished(req, res, result, { mergeWithLastSubmission: false });
  } catch (err) {
    next(err);
  }
});

// leave the rest of the requests to be handled by oidc-provider, there's a catch all 404 there
expressApp.use(oidc.callback);

// express listen
expressApp.listen('3000');

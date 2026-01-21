/* eslint-disable no-underscore-dangle */
const Account = require('../account');

const endImpersonateRoute = async (routeOptions) => {
  const { provider, req, res, next } = { ...routeOptions };
  /* Helper function for handling errors. */
  const forbiddenError = (errorMessage) => {
    /* this next() applies cors headers that are in /lib/helpers/initialize_app.js */
    next();
    res.status(403);
    res.send({ error: errorMessage });
  };

  const ctx = provider.app.createContext(req, res);
  const session = await provider.Session.get(ctx);
  const clients = Object.keys(session.authorizations);

  const impersonatorAccountUsername =
    session.authorizations[process.env.IMPERSONATION_CLIENT]?.meta?.impersonator;

  if (!impersonatorAccountUsername) {
    forbiddenError('Missing impersonatorAccountUsername.');
    return;
  }
  const impersonatorAccount = await Account.findAccount(null, impersonatorAccountUsername, null);

  const meta = {};
  clients.forEach((client) => {
    meta[client] = {
      selected_role:
        impersonatorAccount.profile.roles.length === 1
          ? impersonatorAccount.profile.roles[0]._concatID
          : undefined,
    };
  });

  /** Override old session with new metadata */
  await provider.setProviderSession(req, res, {
    account: impersonatorAccount.accountId,
    clients,
    meta,
  });

  /* this next() applies cors headers that are in /lib/helpers/initialize_app.js */
  next();
  res.status(200);
  res.end();
};

module.exports = endImpersonateRoute;

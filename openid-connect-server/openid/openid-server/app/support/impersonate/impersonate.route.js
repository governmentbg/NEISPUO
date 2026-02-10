/* eslint-disable no-param-reassign */
/* eslint-disable no-underscore-dangle */
const Account = require('../account');
const {
  hasAllowedImpersonationRole,
  hasAllowedTargetAccountRole,
  canImpersonateTargetAccount,
} = require('./role.validator');
const { getAllowedTargetAccountRolesForInstitution } = require('./institution.validator');
const { getAllowedTargetAccountRolesForRUO } = require('./ruo.validator');

const impersonateRoute = async (routeOptions) => {
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

  const impersonatorAccount = await Account.findAccount(null, session.account, null);
  const { targetAccountUsername, targetAccountSysRoleID } = {
    ...req.body,
  };
  if (!targetAccountUsername || !targetAccountSysRoleID) {
    forbiddenError('Missing targetAccountUsername or targetAccountSysRoleID.');
    return;
  }

  if (!hasAllowedImpersonationRole(impersonatorAccount)) {
    forbiddenError(`User ${impersonatorAccount.accountId} has an invalid account type.`);
    return;
  }

  const targetAccount = await Account.findAccount(
    null,
    targetAccountUsername,
    null,
    impersonatorAccount.accountId,
    impersonatorAccount.profile.SysUserID,
  );

  if (!hasAllowedTargetAccountRole(targetAccount)) {
    forbiddenError(`User ${targetAccount.accountId} has an invalid account type.`);
    return;
  }

  if (!canImpersonateTargetAccount(impersonatorAccount, targetAccount)) {
    forbiddenError(
      `User ${impersonatorAccount.accountId} cannot impersonate user ${targetAccount.accountId}.`,
    );
    return;
  }

  const allowedRUOTargetRoles = await getAllowedTargetAccountRolesForRUO(
    impersonatorAccount,
    targetAccount,
    targetAccountSysRoleID,
  );

  if (allowedRUOTargetRoles.length > 0) {
    targetAccount.profile.roles = allowedRUOTargetRoles;
  }

  const allowedInstitutionAccountRoles = getAllowedTargetAccountRolesForInstitution(
    impersonatorAccount,
    targetAccount,
  );

  if (allowedInstitutionAccountRoles.length > 0) {
    targetAccount.profile.roles = allowedInstitutionAccountRoles;
  }

  targetAccount.profile.roles = targetAccount.profile.roles.filter(
    (r) => r.SysRoleID === +targetAccountSysRoleID,
  );

  const meta = {};
  clients.forEach((client) => {
    meta[client] = {
      selected_role:
        targetAccount.profile.roles.length === 1
          ? targetAccount.profile.roles[0]._concatID
          : undefined,
      impersonator: impersonatorAccount.accountId,
      impersonatorSysUserID: impersonatorAccount.profile.SysUserID,
    };
  });

  /** Override old session with new metadata */
  await provider.setProviderSession(req, res, {
    account: targetAccount.accountId,
    clients,
    meta,
  });

  /* this next() applies cors headers that are in /lib/helpers/initialize_app.js */
  next();
  res.status(200);
  res.end();
};

module.exports = impersonateRoute;

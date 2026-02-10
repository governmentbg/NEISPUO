/* eslint-disable no-underscore-dangle */
/* eslint-disable camelcase */
const Account = require('../../../account');
const {
  NeispuoAccessDeniedError,
  NeispuoAccountNotSynchronized,
} = require('../../../custom-errors');
const { createNeispuoParentAccount } = require('../utils/parent-account.utils');
const JWT = require('../../../../../lib/helpers/jwt');

const parentLoginCallbackRoute = async (routeOptions) => {
  const { provider, azureParentIntegration, req, res, next } = routeOptions;
  try {
    const { uid, adapter } = await provider.interactionDetails(req, res);
    const code_verifier = await adapter.find(`oidc-azure-challenge:${uid}`);

    const client = await azureParentIntegration;
    const params = client.callbackParams(req);
    const tokenSet = await client.callback(
      `${process.env.ROOT_URI}/azure-parent-integration-callback`,
      params,
      {
        code_verifier,
      },
    );

    /**
     *
     * @type {{
     *  emails: [string],
     *  given_name: string,
     *  family_name: string,
     *  aud: string
     * }}
     */
    const azureProfile = JWT.decode(tokenSet.id_token).payload;

    let account = await Account.findAccount(null, azureProfile.emails[0], null);

    if (azureProfile.emails[0] && !account) {
      const sysUserID = await createNeispuoParentAccount({
        email: azureProfile.emails[0],
        firstName: azureProfile.given_name,
        lastName: azureProfile.family_name,
        azureID: azureProfile.aud,
      });
      if (!sysUserID) {
        throw new NeispuoAccountNotSynchronized(
          `Parent user ${azureProfile.emails[0]} is not synchronized yet.`,
        );
      }
      account = await Account.findAccount(null, azureProfile.emails[0], null);
    }

    if (!account) {
      throw new NeispuoAccessDeniedError(
        `Parent user ${azureProfile.emails[0]} does not exist in NEISPUO.`,
      );
    }
    const isLocalUser = account && account.profile && !account.profile.IsAzureUser;

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
              // for parent should always have 1 role since they join through a special tennant
              account.profile.roles[0]._concatID,
          },
        };

    // the user has only 1 role and is auto logged in with it
    // that's why we just get his [0] role from his profile
    if (result && result.meta && result.meta.selected_role) {
      await Account.auditLogin(
        account.profile.roles[0],
        // http://expressjs.com/en/4x/api.html#req.ip
        // req.ip should work because 'trust proxy' is set to true
        req.ip,
      );

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
};

module.exports = parentLoginCallbackRoute;

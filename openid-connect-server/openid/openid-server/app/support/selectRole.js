const Check = require('../../lib/helpers/interaction_policy/check');
const Prompt = require('../../lib/helpers/interaction_policy/prompt');

const Account = require('./account');

module.exports = new Prompt(
  {
    name: 'select_role',
    requestable: true,
  },
  async (ctx) => {
    const { oidc } = ctx;
    const { profile } = await Account.findAccount(undefined, oidc.session.account);
    return { roles: (profile && profile.roles) || [] };
  },

  new Check(
    'role_selection_required',
    'role was not selected',
    'role_selection_required',
    (ctx) => {
      const { oidc } = ctx;

      const isAuthorizing = !!oidc.result;
      if (isAuthorizing) {
        const hasSelectedRole = !!(
          oidc.result &&
          oidc.result.meta &&
          oidc.result.meta.selected_role
        );
        return !hasSelectedRole;
      }

      const hasSelectedRole = !!(oidc.session && oidc.session.getSelectedRole());
      return !hasSelectedRole;
    },
  ),
);

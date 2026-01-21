const { externalRolesEnum, internalRolesEnum } = require('../shared/roles.enum');

const roles = { ...externalRolesEnum, ...internalRolesEnum };

const allowedImpersonatorRoles = [
  { id: roles.MON_ADMINISTRATOR, order: 0 },
  { id: roles.CIOO, order: 1 },
  // Comment out as we currently will not allow impersonation for: RUO_IO_AI, RUO_EXPERT, INSTITUTION
  // { id: roles.RUO_IO_AI, order: 2 },
  // { id: roles.RUO_EXPERT, order: 3 },
  // { id: roles.INSTITUTION, order: 4 },
];

const allowedTargetAccountRoles = [
  { id: roles.CIOO, order: 1 },
  { id: roles.RUO_IO_AI, order: 2 },
  { id: roles.RUO_EXPERT, order: 3 },
  { id: roles.INSTITUTION, order: 4 },
  { id: roles.TEACHER, order: 5 },
  { id: roles.STUDENT, order: 6 },
  { id: roles.ACCOUNTANT, order: 20 },
];

function getIsAdminLevelRole(impersonatorAccount) {
  return impersonatorAccount.profile.roles.some((r) =>
    [roles.MON_ADMINISTRATOR, roles.CIOO].includes(r.SysRoleID),
  );
}

function getIsRUOLevelRole(impersonatorAccount) {
  return impersonatorAccount.profile.roles.some((r) =>
    [roles.RUO_IO_AI, roles.RUO_EXPERT].includes(r.SysRoleID),
  );
}

function hasAllowedImpersonationRole(impersonatorAccount) {
  return impersonatorAccount.profile.roles.some((r) =>
    allowedImpersonatorRoles.map((role) => role.id).includes(r.SysRoleID),
  );
}

function hasAllowedTargetAccountRole(targetAccount) {
  return targetAccount.profile.roles.some((r) =>
    allowedTargetAccountRoles.map((role) => role.id).includes(r.SysRoleID),
  );
}

function canImpersonateTargetAccount(impersonatorAccount, targetAccount) {
  /** Sort roles of impersonator account based on the allowedImpersonatorRoles order. */
  const impersonatorRoles = impersonatorAccount.profile.roles
    .map((r) => r.SysRoleID)
    .filter((r) => allowedImpersonatorRoles.find((role) => role.id === r))
    .map((r) => allowedImpersonatorRoles.find((role) => role.id === r).order)
    .sort((a, b) => a - b);

  /** Sort roles of impersonator account based on the allowedTargetAccountRoles order. */

  const targetAccountRoles = targetAccount.profile.roles
    .map((r) => r.SysRoleID)
    .filter((r) => allowedTargetAccountRoles.find((role) => role.id === r))
    .map((r) => allowedTargetAccountRoles.find((role) => role.id === r).order)
    .sort((a, b) => a - b);

  /** If the impersonator has a greater role than the target account they can impersonate */
  if (impersonatorRoles[0] >= targetAccountRoles[0]) {
    return false;
  }

  return true;
}

module.exports = {
  roles,
  hasAllowedImpersonationRole,
  hasAllowedTargetAccountRole,
  canImpersonateTargetAccount,
  getIsAdminLevelRole,
  getIsRUOLevelRole,
};

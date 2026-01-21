const { roles, getIsAdminLevelRole, getIsRUOLevelRole } = require('./role.validator');

function getAllowedTargetAccountRolesForInstitution(impersonatorAccount, targetAccount) {
  const institutionRole = impersonatorAccount.profile.roles.find((r) =>
    [roles.INSTITUTION].includes(r.SysRoleID),
  );

  if (getIsAdminLevelRole(impersonatorAccount) || getIsRUOLevelRole(impersonatorAccount)) {
    return [];
  }
  const institutionID = institutionRole.InstitutionID;
  const filteredTargetAccountRoles = targetAccount.profile.roles.filter(
    (r) => institutionID === r.InstitutionID,
  );
  return filteredTargetAccountRoles;
}

module.exports = {
  getAllowedTargetAccountRolesForInstitution,
};

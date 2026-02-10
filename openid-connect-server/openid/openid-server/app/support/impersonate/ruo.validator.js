/* eslint-disable no-param-reassign */
const knex = require('knex')({
  client: 'mssql',
  connection: {
    server: process.env.MSSQL_HOST,
    port: +process.env.MSSQL_PORT,
    database: process.env.MSSQL_DB,
    user: process.env.MSSQL_USER,
    password: process.env.MSSQL_PASS,
  },
});

const { getIsAdminLevelRole, roles } = require('./role.validator');

function getInstitutionRegionID(institutionID) {
  return knex
    .queryBuilder()
    .select(
      'core.Institution.InstitutionID',
      'location.Region.RegionID',
      'location.Region.Name as RegionName',
      'location.Municipality.MunicipalityID',
      'location.Municipality.Name as MunicipalityName',
    )
    .from('core.Institution')
    .leftJoin('location.Town', 'location.Town.TownID', 'core.Institution.TownID')
    .leftJoin(
      'location.Municipality',
      'location.Town.MunicipalityID',
      'location.Municipality.MunicipalityID',
    )
    .leftJoin('location.Region', 'location.Municipality.RegionID', 'location.Region.RegionID')
    .where('core.Institution.InstitutionID', institutionID)
    .first();
}

function getRolesWithUniqueInstitution(targetAccountRoles) {
  const rolesWithUniqueInstitution = [];
  const institutionIDs = [];
  // eslint-disable-next-line no-restricted-syntax
  for (const role of targetAccountRoles) {
    if (role.InstitutionID && !institutionIDs.includes(role.InstitutionID)) {
      institutionIDs.push(role.InstitutionID);
      rolesWithUniqueInstitution.push(role);
    }
  }
  return rolesWithUniqueInstitution;
}

async function getAllowedTargetAccountRolesForRUO(impersonatorAccount, targetAccount) {
  const ruoRole = impersonatorAccount.profile.roles.find((r) =>
    [roles.RUO_IO_AI, roles.RUO_EXPERT].includes(r.SysRoleID),
  );
  if (getIsAdminLevelRole(impersonatorAccount)) {
    return [];
  }
  /**
   * structuredClone is available from Node 17.0.0 that's why we use JSON.parse(JSON.stringify(obj))
   */
  let filteredTargetAccountRoles = JSON.parse(JSON.stringify(targetAccount.profile.roles));
  const ruoRegionID = ruoRole.RegionID;
  /**
   * The most roles a teacher can have is 3: Institution, Teacher, Accountant.
   * We perform a filter to get only the roles with unique institution to reduce the number of calls to the database.
   */
  const targetAccountRolesWithUniqueInstitution = getRolesWithUniqueInstitution(
    targetAccount.profile.roles,
  );
  const targetAccountRolesPromises = [];

  // eslint-disable-next-line no-restricted-syntax
  for (const role of targetAccountRolesWithUniqueInstitution) {
    targetAccountRolesPromises.push(getInstitutionRegionID(role.InstitutionID));
  }
  const targetAccountRolesResolvedPromises = await Promise.all(targetAccountRolesPromises);
  // eslint-disable-next-line no-restricted-syntax
  for (const rp of targetAccountRolesResolvedPromises) {
    filteredTargetAccountRoles = filteredTargetAccountRoles.map((r) => {
      if (r.InstitutionID === rp.InstitutionID) {
        r.RegionID = rp.RegionID;
        r.RegionName = rp.RegionName;
        r.MunicipalityID = rp.MunicipalityID;
        r.MunicipalityName = rp.MunicipalityName;
      }
      return r;
    });
  }

  filteredTargetAccountRoles = filteredTargetAccountRoles.filter((r) => r.RegionID === ruoRegionID);
  return filteredTargetAccountRoles;
}

module.exports = { getAllowedTargetAccountRolesForRUO };

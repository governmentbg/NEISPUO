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

const sysUserTypesEnum = require('./shared/sys-user-types.enum');

const IGNORE_USERS_LIST = process.env.IGNORE_USERS_LIST
  ? process.env.IGNORE_USERS_LIST.split(',').map((role) => role.trim())
  : [];

const externalRolesEnum = Object.freeze({
  MON_ADMINISTRATOR: 1,
  RUO_IO_AI: 2,
  MUNICIPALITY: 3,
  BUDGETING_INSTITUTION: 4,
  /** Parent is created via NEISPUO Telelink API but is not specifically managed in neispuo yet */
  PARENT: 7,
  RUO_EXPERT: 9,
  CIOO: 10,
  EXTERNAL_INSTITUTION_EXPERTS: 11,
  MON_EXPERT: 12,
  MON_OBGUM: 15,
  MON_OBGUM_FINANCE: 16,
  MON_CHRAO: 17,
  NEISPUO_CONSORTIUM_ADMIN: 18,
  NIO: 19,
  ACCOUNTANT: 20,
  OUTSIDER_PROVIDER: 21,
  VU_TEACHER: 22,
});

/**
 * @type {string[]}
 * @description This array stores string values
 */
let accountRoles = [];

class ExternalUserUtils {
  static hasExternalRole(azureProfile) {
    const azureAssignedSysRoleID =
      +azureProfile.extension_3fd591523047404483834154c3498778_NEISPUO_ROLE || null;
    if (!azureAssignedSysRoleID) return false;
    const result = Object.keys(externalRolesEnum).some(
      (k) =>
        externalRolesEnum[k] === azureAssignedSysRoleID &&
        /** HOTFIX: exclude accountant role as incorrect has_nesipuo_access is added from NUM project */
        azureAssignedSysRoleID !== externalRolesEnum.ACCOUNTANT,
    );
    return result;
  }

  static async getRoles() {
    return await knex.queryBuilder().select('*').from('core.sysRole');
  }

  static async syncAccount(account, azureProfile) {
    const sysRoleId = +azureProfile.extension_3fd591523047404483834154c3498778_NEISPUO_ROLE;
    const additionalIdentifier =
      azureProfile.extension_3fd591523047404483834154c3498778_NEISPUO_ROLE_ADDITIONAL_IDENTIFIER ||
      null;

    const neispuoRoleSysId = (await this.getRoles()).find(
      (r) => r.SysRoleID === sysRoleId,
    )?.SysRoleID;

    if (!neispuoRoleSysId) {
      return false;
    }

    accountRoles = account.profile.roles.find((r) => r.SysRoleID === neispuoRoleSysId);

    const user = await knex
      .queryBuilder()
      .select('*')
      .from('core.sysUser')
      .leftJoin('core.person', 'core.sysUser.personID', 'core.person.personID')
      .join('core.SysUserSysRole', 'core.SysUser.sysUserID', 'core.sysUserSysRole.sysUserID')
      .where('core.person.AzureID', azureProfile.id);

    if (accountRoles)
      switch (neispuoRoleSysId) {
        case externalRolesEnum.RUO_EXPERT:
        case externalRolesEnum.RUO_IO_AI:
          if (accountRoles.RegionID !== additionalIdentifier) {
            await knex('core.SysUserSysRole')
              .update({ RegionID: additionalIdentifier })
              .where({ SysUserID: accountRoles.SysUserID })
              .andWhere({ SysRoleID: accountRoles.SysRoleID });
            return true;
          }
          break;
        case externalRolesEnum.MUNICIPALITY:
          if (accountRoles.MunicipalityID !== additionalIdentifier) {
            await knex('core.SysUserSysRole')
              .update({ MunicipalityID: additionalIdentifier })
              .where({ SysUserID: accountRoles.SysUserID })
              .andWhere({ SysRoleID: accountRoles.SysRoleID });
            return true;
          }
          break;
        case externalRolesEnum.BUDGETING_INSTITUTION:
          if (accountRoles.BudgetingInstitutionID !== additionalIdentifier) {
            await knex('core.SysUserSysRole')
              .update({ BudgetingInstitutionID: additionalIdentifier })
              .where({ SysUserID: accountRoles.SysUserID })
              .andWhere({ SysRoleID: accountRoles.SysRoleID });
            return true;
          }
          break;
        default:
          return false;
      }
    if (!accountRoles && !IGNORE_USERS_LIST.includes(account.profile.Username)) {
      const neispuoProfile = account.profile;
      await this.updateNeispuoRole(
        user[0].SysUserID[0],
        sysRoleId,
        neispuoProfile.roles[0].SysRoleID,
        additionalIdentifier,
      );
      return true;
    }

    return false;
  }

  static async updateNeispuoRole(sysUserID, sysRoleID, oldRoleID, additionalIdentifier) {
    await knex.transaction(async (trx) => {
      await trx('core.SysUserSysRole').where({ SysUserID: sysUserID, SysRoleID: oldRoleID }).del();

      /* eslint-disable default-case */
      switch (sysRoleID) {
        case externalRolesEnum.CIOO:
        case externalRolesEnum.MON_ADMINISTRATOR:
        case externalRolesEnum.MON_EXPERT:
        case externalRolesEnum.MON_OBGUM:
        case externalRolesEnum.MON_OBGUM_FINANCE:
        case externalRolesEnum.MON_CHRAO:
        case externalRolesEnum.NEISPUO_CONSORTIUM_ADMIN:
        case externalRolesEnum.RUO_EXPERT:
        case externalRolesEnum.RUO_IO_AI:
          await trx('core.SysUserSysRole')
            .returning(['*'])
            .insert({
              SysUserID: sysUserID,
              SysRoleID: sysRoleID,
              RegionID: additionalIdentifier,
            })
            .transacting(trx)
            .then((newRole) => {
              accountRoles = newRole;
            });
          break;
        case externalRolesEnum.MUNICIPALITY:
          await trx('core.SysUserSysRole')
            .returning(['*'])
            .insert({
              SysUserID: sysUserID,
              SysRoleID: sysRoleID,
              MunicipalityID: additionalIdentifier,
            })
            .transacting(trx)
            .then((newRole) => {
              accountRoles = newRole;
            });
          break;
        case externalRolesEnum.BUDGETING_INSTITUTION:
          await trx('core.SysUserSysRole')
            .returning(['*'])
            .insert({
              SysUserID: sysUserID,
              SysRoleID: sysRoleID,
              BudgetingInstitutionID: additionalIdentifier,
            })
            .transacting(trx)
            .then((newRole) => {
              accountRoles = newRole;
            });
          break;
      }
    });
  }

  static async createParentAccount(azureProfile) {
    const neispuoRoleSysId = externalRolesEnum.PARENT;

    await knex.transaction(async (trx) => {
      const personID = await trx('core.Person')
        .insert(
          {
            FirstName: azureProfile.given_name || 'none',
            MiddleName: azureProfile.middleName || null,
            LastName: azureProfile.family_name || 'none',
            AzureID: azureProfile.oid || null,
            SysUserType: sysUserTypesEnum.PARENT,
          },
          'PersonID',
        )
        .transacting(trx);

      const sysUserID = await trx('core.sysUser')
        .insert(
          {
            Username: azureProfile.emails[0],
            PersonID: personID[0],
            IsAzureUser: true,
          },
          'SysUserID',
        )
        .transacting(trx);

      const sysUserRole = await trx('core.sysUserSysRole')
        .insert({
          SysUserID: sysUserID[0],
          SysRoleId: neispuoRoleSysId,
        })
        .transacting(trx);
    });
  }

  static async createAccount(azureProfile) {
    const sysRoleId = +azureProfile.extension_3fd591523047404483834154c3498778_NEISPUO_ROLE;
    const additionalIdentifier =
      azureProfile.extension_3fd591523047404483834154c3498778_NEISPUO_ROLE_ADDITIONAL_IDENTIFIER ||
      null;

    const neispuoRoleSysId = (await this.getRoles()).find(
      (r) => r.SysRoleID === sysRoleId,
    ).SysRoleID;
    /** roles that require additional mappings are:
     *  РУО - (Region any type) , Община(Municipality), Финансираща инститиуция техните(Budgeting institution).
     * These roles do not have English and their SysRoleIds should never change. That's why we currently will
     * enumerate them in code. Best way is to add the english word eg. Region, Municipality etc. in the code
     */

    if (!neispuoRoleSysId) {
      return null;
    }

    switch (neispuoRoleSysId) {
      case externalRolesEnum.CIOO:
      case externalRolesEnum.MON_ADMINISTRATOR:
      case externalRolesEnum.MON_EXPERT:
      case externalRolesEnum.MON_OBGUM:
      case externalRolesEnum.MON_OBGUM_FINANCE:
      case externalRolesEnum.MON_CHRAO:
      case externalRolesEnum.NEISPUO_CONSORTIUM_ADMIN:
      case externalRolesEnum.NIO:
        await knex.transaction(async (trx) => {
          const personID = await trx('core.Person')
            .insert(
              {
                FirstName: azureProfile.givenName || 'none',
                MiddleName: azureProfile.middleName || null,
                LastName: azureProfile.surname || 'none',
                AzureID: azureProfile.id || null,
                SysUserType: sysUserTypesEnum.ADMINISTRATIVE,
              },
              'PersonID',
            )
            .transacting(trx);

          const sysUserID = await trx('core.sysUser')
            .insert(
              {
                Username: azureProfile.userPrincipalName,
                PersonID: personID[0],
                IsAzureUser: true,
              },
              'SysUserID',
            )
            .transacting(trx);

          const sysUserRole = await trx('core.sysUserSysRole')
            .insert({
              SysUserID: sysUserID[0],
              SysRoleId: neispuoRoleSysId,
            })
            .transacting(trx);
        });
        break;
      case externalRolesEnum.RUO_EXPERT:
      case externalRolesEnum.RUO_IO_AI:
        await knex.transaction(async (trx) => {
          const region = (
            await trx('location.Region')
              .select('RegionID')
              .where('RegionID', '=', additionalIdentifier)
              .from('location.Region')
          )[0];

          if (!region) {
            return null;
          }

          const personID = await trx('core.Person')
            .insert(
              {
                FirstName: azureProfile.givenName || 'none',
                MiddleName: azureProfile.middleName || null,
                LastName: azureProfile.surname || 'none',
                AzureID: azureProfile.id || null,
                SysUserType: sysUserTypesEnum.ADMINISTRATIVE,
              },
              'PersonID',
            )
            .transacting(trx);

          const sysUserID = await trx('core.sysUser')
            .insert(
              {
                Username: azureProfile.userPrincipalName,
                PersonID: personID[0],
                IsAzureUser: true,
              },
              'SysUserID',
            )
            .transacting(trx);
          const sysUserRole = await trx('core.sysUserSysRole')
            .insert({
              SysUserID: sysUserID[0],
              SysRoleId: neispuoRoleSysId,
              RegionID: region.RegionID,
            })
            .transacting(trx);
        });
        break;
      case externalRolesEnum.MUNICIPALITY:
        await knex.transaction(async (trx) => {
          const municipality = (
            await trx('location.Municipality')
              .select('MunicipalityID', 'RegionID')
              .where('MunicipalityID', '=', additionalIdentifier)
              .from('location.Municipality')
          )[0];
          if (!municipality) {
            return null;
          }

          const personID = await trx('core.Person')
            .insert(
              {
                FirstName: azureProfile.givenName || 'none',
                MiddleName: azureProfile.middleName || null,
                LastName: azureProfile.surname || 'none',
                AzureID: azureProfile.id || null,
                SysUserType: sysUserTypesEnum.ADMINISTRATIVE,
              },
              'PersonID',
            )
            .transacting(trx);

          const sysUserID = await trx('core.sysUser')
            .insert(
              {
                Username: azureProfile.userPrincipalName,
                PersonID: personID[0],
                IsAzureUser: true,
              },
              'SysUserID',
            )
            .transacting(trx);
          const sysUserRole = await trx('core.sysUserSysRole')
            .insert({
              SysUserID: sysUserID[0],
              SysRoleId: neispuoRoleSysId,
              MunicipalityID: municipality.MunicipalityID,
            })
            .transacting(trx);
        });
        break;
      case externalRolesEnum.BUDGETING_INSTITUTION:
        await knex.transaction(async (trx) => {
          const budgetingInstitution = (
            await trx('noms.BudgetingInstitution')
              .select('BudgetingInstitutionID')
              .where('BudgetingInstitutionID', '=', additionalIdentifier)
              .from('noms.BudgetingInstitution')
          )[0];
          if (!budgetingInstitution) {
            return null;
          }

          const personID = await trx('core.Person')
            .insert(
              {
                FirstName: azureProfile.givenName || 'none',
                MiddleName: azureProfile.middleName || null,
                LastName: azureProfile.surname || 'none',
                AzureID: azureProfile.id || null,
                SysUserType: sysUserTypesEnum.ADMINISTRATIVE,
              },
              'PersonID',
            )
            .transacting(trx);

          const sysUserID = await trx('core.sysUser')
            .insert(
              {
                Username: azureProfile.userPrincipalName,
                PersonID: personID[0],
                IsAzureUser: true,
              },
              'SysUserID',
            )
            .transacting(trx);
          const sysUserRole = await trx('core.sysUserSysRole')
            .insert({
              SysUserID: sysUserID[0],
              SysRoleId: neispuoRoleSysId,
              BudgetingInstitutionID: budgetingInstitution.BudgetingInstitutionID,
            })
            .transacting(trx);
        });
        break;
      default:
        return null;
    }
  }
}

module.exports.ExternalUserUtils = ExternalUserUtils;

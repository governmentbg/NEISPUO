/* eslint-disable no-promise-executor-return */
/* eslint-disable no-param-reassign */
/* eslint-disable guard-for-in */
/* eslint-disable no-underscore-dangle */
/* eslint-disable camelcase */
/* eslint-disable no-await-in-loop */
/* eslint-disable no-restricted-syntax */

const { nanoid } = require('nanoid');
const bcrypt = require('bcryptjs');
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
const { ExternalUserUtils } = require('./external-user-utils');
const { RoleRevokedError } = require('./custom-errors');
const { internalRolesEnum } = require('./shared/roles.enum');

/**
 * USER_ROLES_QUERY bellow can be ran and tweaked against db and is used as a view ('with' clause)
 *
 * !!! IMPORTANT !!!
 * MSSQL will NOT ensure that both sides of the UNION bellow align perfectly. It is therefore
 * your responsibility to do so by making sure both selects have the same column order.
 */
const USER_ROLES_QUERY = `--
-- Select roles from educational state
 SELECT
	su.Username as Username,
	su.PersonID as PersonID,
	su.SysUserID as SysUserID,
	su.DeletedOn as DeletedOn,
	p2.SysRoleID as SysRoleID,
  sr.Name as SysRoleName,
  1 as IsRoleFromEducationalState,
	i.InstitutionID as InstitutionID,
  i.Name as InstitutionName,
	p2.PositionID as PositionID,
	null as MunicipalityID,
  null as MunicipalityName,
	null as RegionID,
  null as RegionName,
	null as BudgetingInstitutionID,
	null as BudgetingInstitutionName
FROM
	core.SysUser su
left join core.Person p on
  p.PersonID = su.PersonID
left join core.EducationalState es on
	es.PersonID = p.PersonID
left join core.[Position] p2 on
	p2.PositionID = es.PositionID
left join core.Institution i on
	i.InstitutionID = es.InstitutionID
	-- inner join as we want to only select rows that would lead to a role match
inner join core.SysRole sr on
	sr.SysRoleID = p2.SysRoleID

UNION

-- Select roles from SysUserSysRole table
 SELECT
	su2.Username as Username,
	su2.PersonID as PersonID,
	su2.SysUserID as SysUserID,
	su2.DeletedOn as DeletedOn,
	susr.SysRoleID as SysRoleID,
  sr2.Name as SysRoleName,
  0 as IsRoleFromEducationalState,
	susr.InstitutionID as InstitutionID,
  i2.Name as InstitutionName,
	null as PositionID,
	susr.MunicipalityID as MunicipalityID,
  m.Name as MunicipalityName,
	susr.RegionID as RegionID,
  r.Name as RegionName,
	susr.BudgetingInstitutionID as BudgetingInstitutionID,
	bi.Name as BudgetingInstitutionName
FROM
	core.SysUser su2
	-- inner join as we want to only select rows that would lead to a role match
inner join core.SysUserSysRole susr on
	susr.SysUserID = su2.SysUserID
left join core.SysRole sr2 on
	sr2.SysRoleID = susr.SysRoleID
left join location.Region r on
	r.RegionID = susr.RegionID
left join location.Municipality m on
	m.MunicipalityID = susr.MunicipalityID
left join core.Institution i2 on
	i2.InstitutionID = susr.InstitutionID
left join noms.BudgetingInstitution bi on
	bi.BudgetingInstitutionID = susr.BudgetingInstitutionID`
  .replace(/--.*/gm, ' ') // Remove comments
  .replace(/\n/gm, ' '); // Flatten query into single line

const LEAD_TEACHER_CLASSES_QUERY = `
      SELECT leadClassTeachers.ClassID, leadClassTeachers.PersonID, leadClassTeachers.InstitutionID
      --- inst_year.ClassTeacher199 - Database View which contains information about Lead Teachers
      FROM inst_year.ClassTeacher199 leadClassTeachers
`
  .replace(/--.*/gm, ' ') // Remove comments
  .replace(/\n/gm, ' '); // Flatten query into single line
async function findLocalUser(username) {
  if (!username) {
    return null;
  }

  let user = await knex
    .queryBuilder()
    .select('*')
    .from('core.sysUser')
    .leftJoin('core.person', 'core.sysUser.personID', 'core.person.personID')
    .whereNull('DeletedOn')
    .andWhere('Username', username);
  if (!user || user.length !== 1) {
    return null;
  }
  // eslint-disable-next-line prefer-destructuring
  user = user[0];

  const userRoles = await knex
    .queryBuilder()
    .with('userRoles', knex.raw(USER_ROLES_QUERY))
    .select('*')
    .from('userRoles')
    .whereNull('DeletedOn')
    .andWhere('Username', username);

  const roles = [];
  for (const role of userRoles) {
    if (role.SysRoleID === internalRolesEnum.TEACHER) {
      const leadTeacherClasses = await knex
        .queryBuilder()
        .with('leadTeacherClasses', knex.raw(LEAD_TEACHER_CLASSES_QUERY))
        .select('*')
        .from('leadTeacherClasses')
        .where('PersonID', role.PersonID)
        .andWhere('InstitutionID', role.InstitutionID);

      const isLeadTeacher = !!leadTeacherClasses && leadTeacherClasses.length > 0;
      role.IsLeadTeacher = isLeadTeacher;
      role.LeadTeacherClasses = null;
      if (isLeadTeacher) {
        role.LeadTeacherClasses = [...leadTeacherClasses.map((c) => c.ClassID)];
      }
    } else if (role.SysRoleID === internalRolesEnum.STUDENT) {
      const classBookIDs = await knex
        .queryBuilder()
        .select('classBookId')
        .from('school_books.vwStudentClassBooks')
        .where('PersonID', role.PersonID)
        .andWhere('InstId', role.InstitutionID);

      role.ClassBookIDs = [...classBookIDs.map((c) => c.classBookId)];
    } else if (role.SysRoleID === internalRolesEnum.PARENT) {
      const classBookIDs = await knex
        .queryBuilder()
        .select('classBookId')
        .from('school_books.vwStudentClassBooks')
        .join(
          'core.ParentChildSchoolBookAccess',
          'core.ParentChildSchoolBookAccess.ChildID',
          '=',
          'school_books.vwStudentClassBooks.PersonId',
        )
        .where('ParentID', role.PersonID)
        .andWhere('HasAccess', 1);
      const studentPersonIDs = await knex
        .queryBuilder()
        .select('ChildID')
        .from('core.ParentChildSchoolBookAccess')
        .where('ParentID', role.PersonID)
        .andWhere('HasAccess', 1);
      role.StudentPersonIDs = [...studentPersonIDs.map((s) => s.ChildID)];
      role.ClassBookIDs = [...classBookIDs.map((c) => c.classBookId)];
    }

    /**
      _concatId is used to know which row the user has selected.
      Using row index would be brittle in case the number of rows changed.
      Using json stringify would be brittle in case any human friendly changed.
    */
    role._concatID = '';
    for (const [key, value] of Object.entries(role)) {
      if (key.toLowerCase().endsWith('id')) {
        role._concatID += value === null ? 'n' : value;
      }
    }

    roles.push(role);
  }

  return { ...user, roles };
}

class Account {
  constructor(id, profile, selected_role = null) {
    this.accountId = id || nanoid();
    this.profile = profile;
    this.selected_role = selected_role;
  }

  /**
   * @param use - can either be "id_token" or "userinfo", depending on
   *   where the specific claims are intended to be put in.
   * @param scope - the intended scope, while oidc-provider will mask
   *   claims depending on the scope automatically you might want to skip
   *   loading some claims from external resources etc. based on this detail
   *   or not return them in id tokens but only userinfo and so on.
   */
  async claims(use, scope, claims, rejected) {
    let selected_role;
    let profile = {};
    if (use === 'id_token' && this.selected_role) {
      // provide selected role if it is still valid
      selected_role = this.profile.roles.find((r) => r._concatID === this.selected_role);
      if (!selected_role) {
        throw new RoleRevokedError(
          `User ${this.accountId} no longer possesses the selected role. _concatID: ${this.selected_role}`,
        );
      }

      // include only id's in selected_role to keep jwt lightweight
      for (const key in selected_role || {}) {
        const isLeadTeacherProperty = key === 'LeadTeacherClasses' || key === 'IsLeadTeacher';
        const isClassBookIDsProperty = key === 'ClassBookIDs';
        const isStudentPersonIDsProperty = key === 'StudentPersonIDs';
        if (
          key !== 'Username' &&
          !key.toLowerCase().endsWith('id') &&
          !isLeadTeacherProperty &&
          !isClassBookIDsProperty &&
          !isStudentPersonIDsProperty
        ) {
          delete selected_role[key];
        }
      }
    } else if (use === 'userinfo') {
      profile = { ...this.profile };
    }

    return {
      sub: this.accountId, // it is essential to always return a sub claim
      ...profile,
      selected_role,
    };
  }

  validatePassword(password) {
    const passwordHash = (this.profile && this.profile.Password) || '';
    return new Promise((resolve, reject) =>
      bcrypt.compare(password, passwordHash, (err, res) => {
        if (err) {
          return reject(err);
        }
        return resolve(res);
      }),
    );
  }

  async changePassword(newPassword) {
    if (!this.accountId) {
      throw new Error('changePassword error: Falsy accountId');
    }

    const newHash = await bcrypt.hash(newPassword, +process.env.BCRYPT_SALT_ROUNDS);
    await knex('core.SysUser').where('Username', '=', this.accountId).update({ Password: newHash });

    // reload profile
    this.profile = await findLocalUser(this.accountId);
  }

  static async findAccount(ctx, id, token, impersonator = null, impersonatorSysUserID = null) {
    // token is a reference to the token used for which a given account is being loaded,
    //   it is undefined in scenarios where account claims are returned from authorization endpoint
    // ctx is the koa request context

    const profile = await findLocalUser(id);
    if (!profile) {
      return null;
    }
    let selected_role = ctx && ctx.oidc && ctx.oidc.session && ctx.oidc.session.getSelectedRole();

    if (!selected_role && ctx && token) {
      // During pkce, session is not previously loaded by the framework. Manually loading it here
      const session = await ctx.oidc.provider.Session.findByUid(token.sessionUid);
      selected_role = session.getSelectedRole();
      impersonator = session.authorizations[token.clientId]?.meta?.impersonator;
      impersonatorSysUserID = session.authorizations[token.clientId]?.meta?.impersonatorSysUserID;
    }
    return new Account(id, { ...profile, impersonator, impersonatorSysUserID }, selected_role);
  }

  static hasExternalRole(azureProfile) {
    return ExternalUserUtils.hasExternalRole(azureProfile);
  }

  static isStudentOrTeacher(azureProfile) {
    const allowedRoles = ['student', 'teacher'];
    return allowedRoles.find(
      (r) => !!azureProfile.primaryRole && r === azureProfile.primaryRole.toLowerCase(),
    );
  }

  static isSchool(azureProfile) {
    return (
      +azureProfile.extension_3fd591523047404483834154c3498778_NEISPUO_ROLE ===
      internalRolesEnum.INSTITUTION
    );
  }

  static async getSysUserMessages(role) {
    const todayDateTime2 = `${new Date().toISOString().slice(0, 19).replace('T', ' ')}.0000000`;
    return knex
      .queryBuilder()
      .select('*')
      .from('core.SystemUserMessage')
      .where('EndDate', '>', todayDateTime2)
      .andWhere('StartDate', '<=', todayDateTime2)
      .andWhere(knex.raw(`(',' + RTRIM(Roles) + ',') LIKE '%,${role.SysRoleID},%'`))
      .orderBy('StartDate', 'desc');
  }

  static async auditLogin(role, ip) {
    return knex.transaction(async (trx) => {
      // role = municipality
      if (role.MunicipalityID) {
        // get region and assign it to role.RegionID & role.RegionName
        const region = await trx('location.Region')
          .select('location.Region.RegionID', 'location.Region.Name as RegionName')
          .leftJoin(
            'location.Municipality',
            'location.Municipality.RegionID',
            'location.Region.RegionID',
          )
          .where('location.Municipality.MunicipalityID', role.MunicipalityID)
          .transacting(trx);

        role.RegionID = region[0].RegionID;
        role.RegionName = region[0].RegionName;
      } else if (role.InstitutionID) {
        // role = director, teacher, student
        // get municipality & region and assign it to role obj
        const municipalityAndRegion = await trx('location.Region')
          .select(
            'location.Municipality.MunicipalityID',
            'location.Municipality.Name as MunicipalityName',
            'location.Region.RegionID',
            'location.Region.Name as RegionName',
          )
          .leftJoin(
            'location.Municipality',
            'location.Municipality.RegionID',
            'location.Region.RegionID',
          )
          .leftJoin(
            'location.Town',
            'location.Town.MunicipalityID',
            'location.Municipality.MunicipalityID',
          )
          .leftJoin('core.Institution', 'core.Institution.TownID', 'location.Town.TownID')
          .where('core.Institution.InstitutionID', role.InstitutionID)
          .transacting(trx);

        role.MunicipalityID = municipalityAndRegion[0].MunicipalityID;
        role.MunicipalityName = municipalityAndRegion[0].MunicipalityName;
        role.RegionID = municipalityAndRegion[0].RegionID;
        role.RegionName = municipalityAndRegion[0].RegionName;
      }

      return trx('logs.LoginAudit')
        .insert(
          {
            SysUserID: role.SysUserID,
            Username: role.Username,
            SysRoleID: role.SysRoleID,
            SysRoleName: role.SysRoleName,
            InstitutionID: role.InstitutionID,
            InstitutionName: role.InstitutionName,
            RegionID: role.RegionID,
            RegionName: role.RegionName,
            MunicipalityID: role.MunicipalityID,
            MunicipalityName: role.MunicipalityName,
            BudgetingInstitutionID: role.BudgetingInstitutionID,
            BudgetingInstitutionName: role.BudgetingInstitutionName,
            PositionID: role.PositionID,
            IPSource: ip,
          },
          '*',
        )
        .transacting(trx);
    });
  }

  static async createAudit(auditEntity) {
    return knex('logs.Audit').insert({ ...auditEntity });
  }

  static async syncAccountWithAzure(account, azureProfile) {
    return ExternalUserUtils.syncAccount(account, azureProfile);
  }

  static async upsertAccount(azureProfile) {
    await ExternalUserUtils.createAccount(azureProfile);
    return this.findAccount(null, azureProfile.userPrincipalName, null);
  }

  // TODO: HOTFIX - This method randomly times out and prevents users from accessing the system.
  // The session check should be performed against Redis instead of the audit table in the database.
  // This is a temporary fix to prevent system access issues while the proper Redis-based solution is implemented.
  static async sessionExists(sessionID) {
    try {
      
      const session = await knex
      .queryBuilder()
      .select('AuditId')
      .from('logs.Audit')
      .where('LoginSessionId', sessionID)
      .limit(1);
      return session.length > 0;
    } catch (error) {
      console.error('Error checking if session exists', error);
      return false;
    }
  }
}

module.exports = Account;

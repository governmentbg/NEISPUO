const enquirer = new require('enquirer');
const knex = require('knex')({
  client: 'mssql',
  connection: {
    server: process.env.SB__Data__DbIP_MON,
    port: +process.env.SB__Data__DbPort_MON,
    database: 'neispuo-prod',
    user: process.env.SB__Data__DbUser_MON,
    password: process.env.SB__Data__DbPass_MON,
    options: {
      encrypt: false,
      enableArithAbort: true // silence tedious deprecation warning
    }
  },
});

const internalRolesEnum = Object.freeze({
  INSTITUTION: 0,
  TEACHER: 5,
  STUDENT: 6,
  PARENT: 7,
});

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

// The code above is from DSS oidc. Only the MSSQL env var names have been changed.
// https://github.com/Neispuo/openid-connect-server/blob/master/openid/openid-server/app/support/account.js

// Taken from Account.claims from DSS oidc
// https://github.com/Neispuo/openid-connect-server/blob/master/openid/openid-server/app/support/account.js#L230
function trimTokenSelectedRole(selected_role) {
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

  return { ...selected_role };
}

async function main() {
  try {
    let username = null;
    try {
      username = (await enquirer.prompt({
        type: 'input',
        name: 'username_prompt_result',
        message: 'Enter username:'
      })).username_prompt_result;
    } catch {
      return;
    }

    const user = await findLocalUser(username);

    if (user == null) {
      console.log("User not found!");
      return;
    }

    if (!user.roles?.length) {
      console.log("User has no roles!");
      return;
    }

    if (user.roles.length === 1) {
      console.log("Token selected_role:");
      console.log(JSON.stringify(trimTokenSelectedRole(user.roles[0]), null, 2));
      return;
    }

    const roles = user.roles.map((role, i) => {
      // adapted from DSS oidc
      // https://github.com/Neispuo/openid-connect-server/blob/master/openid/openid-server/app/views/select_role.ejs#L35
      let roleName = `#${i + 1} Роля: ${role.SysRoleName}`

      if (role.InstitutionName) {
        roleName += `, Институция: ${role.InstitutionName}`;
      } if (role.BudgetingInstitutionName) {
        roleName += `, Финансираща  ${role.BudgetingInstitutionName}`;
      } if (role.MunicipalityName) {
        roleName += `, Община: ${role.MunicipalityName}`;
      } if (role.RegionName) {
        roleName += `, Област: ${role.RegionName}`;
      }

      return roleName;
    });

    let role = null;
    try {
      role = (await enquirer.prompt({
        type: 'select',
        name: 'roles_prompt_result',
        message: 'Choose role',
        choices: roles
      })).roles_prompt_result;
    } catch {
      return;
    }

    const roleIndex = parseInt(role.match(/#(\d+) .+/)[1], 10) - 1;

    console.log("Token selected_role:");
    console.log(JSON.stringify(trimTokenSelectedRole(user.roles[roleIndex]), null, 2));
  } finally {
    knex.destroy();
  }
}

main();

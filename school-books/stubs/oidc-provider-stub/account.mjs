import low from 'lowdb';
import Memory from 'lowdb/adapters/Memory.js';
import assert from 'assert';
import { Connection, Request } from 'tedious';

const db = low(new Memory());

const schoolRole = 0 // School
const monRole = 1 // MON(sees all)
const ruoRole = 2 // RUO
const municipalityRole = 3 // Municipality
const otherRole = 4 // Other Budgeting institution
const teacherRole = 5 // Teacher
const studentRole = 6 // Student
const parentRole = 7 // Parent?

const SqlConnectionConfig = {
  server: process.env.SB__Data__DbIP,
  authentication: {
    type: 'default',
    options: {
      userName: process.env.SB__Data__DbUser,
      password: process.env.SB__Data__DbPass
    }
  },
  options: {
    database: process.env.SB__Data__DbName,
    port: parseInt(process.env.SB__Data__DbPort, 10),
    encrypt: false,
    rowCollectionOnRequestCompletion: true
  }
};

db.defaults({
  users: [
    {
      id: 'school@mon.bg', // директор - СОУ В.Благоева 4а
      "selected_role": {
        "Username": "school@mon.bg",
        "SysUserID": 1001,
        "SysRoleID": schoolRole,
        "InstitutionID": 300125,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'school2@mon.bg', // директор - ПГТЕ Х.Форд
      "selected_role": {
        "Username": "school2@mon.bg",
        "SysUserID": 1002,
        "SysRoleID": schoolRole,
        "InstitutionID": 2206409,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'school3@mon.bg', // директор - ОУ "Св.Иван Рилски", с. Балван
      "selected_role": {
        "Username": "school3@mon.bg",
        "SysUserID": 1018,
        "SysRoleID": schoolRole,
        "InstitutionID": 300110,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'dg@mon.bg', // директор - Детска градина "Ханс Кристиан Андерсен"
      "selected_role": {
        "Username": "dg@mon.bg",
        "SysUserID": 1014,
        "SysRoleID": schoolRole,
        "InstitutionID": 200277,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'dg2@mon.bg', // директор - Детска градина "Звездичка"
      "selected_role": {
        "Username": "dg2@mon.bg",
        "SysUserID": 1017,
        "SysRoleID": schoolRole,
        "InstitutionID": 100006,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'csop@mon.bg', // директор - Център за специална образователна подкрепа "Д-р Петър Берон"
      "selected_role": {
        "Username": "csop@mon.bg",
        "SysUserID": 1015,
        "SysRoleID": schoolRole,
        "InstitutionID": 607055,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'rcpppo@mon.bg', // директор - Регионален център за подкрепа на процеса на приобщаващо образование - Пловдив
      "selected_role": {
        "Username": "rcpppo@mon.bg",
        "SysUserID": 1016,
        "SysRoleID": schoolRole,
        "InstitutionID": 1690180,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'cplr@mon.bg', // директор - ЦПЛР Котел
      "selected_role": {
        "Username": "cplr@mon.bg",
        "SysUserID": 1019,
        "SysRoleID": schoolRole,
        "InstitutionID": 2000218,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    },
    {
      id: 'teacher@mon.bg',
      "selected_role": {
        "Username": "teacher@mon.bg",
        "SysUserID": 1003,
        "SysRoleID": teacherRole,
        "InstitutionID": 300125,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 84528404
      }
    },
    {
      id: 'teacher2@mon.bg',
      "selected_role": {
        "Username": "teacher2@mon.bg",
        "SysUserID": 1004,
        "SysRoleID": teacherRole,
        "InstitutionID": 300125,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 84613549
      }
    },
    {
      id: 'teacher3@mon.bg',
      "selected_role": {
        "Username": "teacher3@mon.bg",
        "SysUserID": 1005,
        "SysRoleID": teacherRole,
        "InstitutionID": 300125,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 84416511
      }
    },
    {
      id: 'teacher4@mon.bg', // класен - ПГТЕ Х.Форд 8к
      "selected_role": {
        "Username": "teacher4@mon.bg",
        "SysUserID": 1008,
        "SysRoleID": teacherRole,
        "InstitutionID": 2206409,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 85753100
      }
    },
    {
      id: 'teacher5@mon.bg', // учител БЕЛ - ПГТЕ Х.Форд 8к
      "selected_role": {
        "Username": "teacher5@mon.bg",
        "SysUserID": 1009,
        "SysRoleID": teacherRole,
        "InstitutionID": 2206409,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 85366174
      }
    },
    {
      id: 'teacher6@mon.bg', // класен - СОУ В.Благоева 4а
      "selected_role": {
        "Username": "teacher6@mon.bg",
        "SysUserID": 1010,
        "SysRoleID": teacherRole,
        "InstitutionID": 300125,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 85774665
      }
    },
    {
      id: 'teacher7@mon.bg', // учител Човекът и обществото, Родолюбие - СОУ В.Благоева 4а
      "selected_role": {
        "Username": "teacher7@mon.bg",
        "SysUserID": 1011,
        "SysRoleID": teacherRole,
        "InstitutionID": 300125,
        "PositionID": 2,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null,
        "PersonID": 84414911
      }
    },
    {
      id: 'parent1@mon.bg',
      "selected_role": {
        "Username": "parent1@mon.bg",
        "SysUserID": 1006,
        "SysRoleID": parentRole,
        "InstitutionID": 300125,
        "PersonID": 1,
        "StudentPersonIDs": [83748488, 83854217]
        // ClassBookIDs added by Account.findAccount
      }
    },
    {
      id: 'student1@mon.bg',
      "selected_role": {
        "Username": "student1@mon.bg",
        "SysUserID": 1007,
        "SysRoleID": studentRole,
        "InstitutionID": 300125,
        "PersonID": 83748488
        // ClassBookIDs added by Account.findAccount
      }
    },
    {
      id: 'mon@mon.bg',
      "selected_role": {
        "Username": "mon@mon.bg",
        "PersonID": 83071831,
        "SysUserID": 1012,
        "SysRoleID": monRole,
        "InstitutionID": null,
        "PositionID": null,
        "MunicipalityID": null,
        "RegionID": null,
        "BudgetingInstitutionID": null
      }
    }
  ],
}).write();

export default class Account {
  // This interface is required by oidc-provider
  static async findAccount(ctx, id) {
    // This would ideally be just a check whether the account is still in your storage
    const account = db.get('users').find({ id }).value();
    if (!account) {
      return undefined;
    }

    return {
      accountId: id,
      // and this claims() method would actually query to retrieve the account claims
      async claims(use) {
        if (use === 'userinfo') {
          return {
            sub: id,
            FirstName: 'иван',
            LastName: 'иванов'
          };
        }

        let studentPersonIds = null;

        if (account.selected_role.SysRoleID === studentRole) {
          studentPersonIds = [account.selected_role.PersonID];
        } else if (account.selected_role.SysRoleID === parentRole) {
          studentPersonIds = account.selected_role.StudentPersonIDs;
        }

        if (studentPersonIds && studentPersonIds.length) {
          try {
            const classBookIds = await getClassBooksForStudents(studentPersonIds);

            return {
              sub: id,
              sessionID: 'session1234567',
              selected_role: {
                ClassBookIDs: classBookIds,
                ...account.selected_role,
              }
            };
          }
          catch (e) {
            console.log(e);
          }
        } else {
          return {
            sub: id,
            sessionID: 'session1234567',
            selected_role: account.selected_role,
          };
        }
      },
    };
  }

  // This can be anything you need to authenticate a user
  static async authenticate(email, password) {
    try {
      assert(password, 'password must be provided');
      assert(email, 'email must be provided');
      const lowercased = String(email).toLowerCase();
      const account = db.get('users').find({ id: lowercased }).value();
      assert(account, 'invalid credentials provided');

      return account.id;
    } catch (err) {
      return undefined;
    }
  }
}

async function getClassBooksForStudents(studentPersonIds) {
  if (!studentPersonIds || !studentPersonIds.length) {
    return [];
  }

  const sql = `SELECT ClassBookId FROM [school_books].[vwStudentClassBooks] WHERE PersonId IN (${ studentPersonIds.join() })`

  const { rows } = await executeSql(sql);

  return rows.map(columns => columns[0].value);
}

function executeSql(sql) {
  return new Promise((resolve, reject) => {
    const connection = new Connection(SqlConnectionConfig);

    connection.on('connect', (err) => {
      if (err) {
        reject(err);
      }

      const request = new Request(sql, (err, rowCount, rows) => {
        if (err) {
          reject(err);
        } else {
          resolve({ rowCount, rows });
        }
      });

      connection.execSql(request);

    });

    connection.connect();
  });
}


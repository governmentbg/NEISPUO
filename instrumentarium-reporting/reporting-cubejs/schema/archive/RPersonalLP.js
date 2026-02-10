/* eslint no-undef: "off" */

cube('RPersonalLP', {
  sql: 'SELECT * FROM inst_basic.R_Personal_LP',
  title: 'Списъчен състав на персонала',
  description: 'Служи за извършване на справки свързани със списъчен състав на персонал.',


  joins: {},

  measures: {
    countPersonal: {
      type: 'count',
      title: 'Брой "Персонал"',
      drillMembers: [
        InstitutionID,
        FirstName,
        MiddleName,
        LastName,
        PositionCount,
        CurrentlyValid,
        StaffTypeName
      ],
    },
  },

  dimensions: {
    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },
    FirstName: {
      sql: 'FirstName',
      type: 'string',
      title: 'Име',
    },
    MiddleName: {
      sql: 'MiddleName',
      type: 'string',
      title: 'Презиме',
    },
    LastName: {
      sql: 'LastName',
      type: 'string',
      title: 'Фамилия',
    },
    PositionCount: {
      sql: 'PositionCount',
      type: 'number',
      title: 'Щат'
    },
    CurrentlyValid: {
      sql: 'CurrentlyValid',
      type: 'string',
      title: 'Активна за текущата учебна година',
    },
    StaffTypeName: {
      sql: 'StaffTypeName',
      type: 'string',
      title: 'Вид персонал',
    },

  },

  dataSource: 'default',
});

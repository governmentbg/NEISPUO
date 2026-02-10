/* eslint no-undef: "off" */

cube('RHostels', {
  sql: 'SELECT * FROM inst_basic.R_hostels',
  title: 'Общежития',
  description: 'Служи за извършване на справки свързани с данни по общежития.',


  preAggregations: {
    // Pre-Aggregations definitions go here
    // Learn more here: https://cube.dev/docs/caching/pre-aggregations/getting-started
  },

  joins: {},

  measures: {
    countInstitutionID: {
      sql: 'InstitutionID',
      title: 'Брой "Институции"',
      type: 'count',
      drillMembers: [InstitutionName, InstitutionID, ClassName, StudentsCount],
    },
    sumStudentsCount: {
      sql: 'StudentsCount',
      title: 'Сумирай по "Брой ученици"',
      type: 'sum',
      drillMembers: [InstitutionName, InstitutionID, ClassName, StudentsCount],
    },
  },

  dimensions: {
    InstitutionName: {
      sql: 'InstitutionName',
      type: 'string',
      title: 'Институция',
    },

    InstitutionID: {
      sql: 'InstitutionID',
      type: 'number',
      title: 'Код по НЕИСПУО',
    },

    ClassName: {
      sql: 'ClassName',
      type: 'string',
      title: 'Клас/група',
    },
    StudentsCount: {
      sql: 'StudentsCount',
      type: 'number',
      title: 'Брой ученици',
    }

  },

  dataSource: 'default',
});

/* eslint no-undef: "off" */

cube('RCouncils', {
    sql: 'SELECT * FROM inst_basic.R_councils',
    title: 'Съвети',
    description: 'Служи за извършване на справки свързани с данни по съвети.',

    preAggregations: {
        // Pre-Aggregations definitions go here
        // Learn more here: https://cube.dev/docs/caching/pre-aggregations/getting-started
    },

    joins: {},

    measures: {
        countInstitutionName: {
            type: 'count',
            title: 'Брой "Институции"',
            sql: "InstitutionName",
            drillMembers: [InstitutionName, FirstName, MiddleName, LastName, Phone, Email, Role, CouncilType],
        },
    },

    dimensions: {
        InstitutionName: {
            sql: 'InstitutionName',
            type: 'string',
            title: 'Институция',
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

        Phone: {
            sql: 'Phone',
            type: 'string',
            title: 'Телефонен номер',
        },

        Email: {
            sql: 'Email',
            type: 'string',
            title: 'Имейл',
        },

        Role: {
            sql: 'Role',
            type: 'string',
            title: 'Роля',
        },

        CouncilType: {
            sql: 'CouncilType',
            type: 'string',
            title: 'Вид съвет',
        },
    },

    dataSource: 'default',
});

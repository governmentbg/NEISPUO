/* eslint no-undef: "off" */

cube('RGraduatedStudents', {
  sql: 'SELECT TOP 100 PERCENT * FROM reporting.R_Graduated_Students ORDER BY Year DESC',
  title: 'OpenData - Дипломирани ученици',
  description: 'Служи за извършване на справки свързани с данни по Брой ученици по типове училища.',

  joins: {},

  measures: {
    sumAllDiplomasSUM: {
      type: 'sum',
      title: 'Сумирай по "Брой дипломи"',
      sql: 'AllDiplomasSUM',
      drillMembers: [Year, Region, SecondarySchools, SportSchools, SeminarySchools,ProfiledSchools, ProfessionalSchools,SpecialSchools, ArtSchools]
    }
  },

  dimensions: {
    Year: {
      sql: 'Year',
      type: 'number',
      title: 'Година',
    },
    Region: {
      sql: 'Region',
      type: 'string',
      title: 'Област',
    },
    AllDiplomasSUM: {
      sql: 'AllDiplomasSUM',
      type: 'number',
      title: 'Брой дипломи',
    },
    SecondarySchools: {
      sql: 'SecondarySchools',
      type: 'number',
      title: 'Средни училища',
    },
    SportSchools: {
      sql: 'SportSchools',
      type: 'number',
      title: 'Спортни училища',
    },
    SeminarySchools: {
      sql: 'SeminarySchools',
      type: 'number',
      title: 'Духовни училища',
    },
    ProfiledSchools: {
      sql: 'ProfiledSchools',
      type: 'number',
      title: 'Профилирани гимназии',
    },
    ProfessionalSchools: {
      sql: 'ProfessionalSchools',
      type: 'number',
      title: 'Професионални гимназии',
    },
    SpecialSchools: {
      sql: 'SpecialSchools',
      type: 'number',
      title: 'Специални училища/Училища към местата за лишаване от свобода',
    },
    ArtSchools: {
      sql: 'ArtSchools',
      type: 'number',
      title: 'Училища по изкуствата/по културата',
    },
  },

    dataSource: 'default',
});

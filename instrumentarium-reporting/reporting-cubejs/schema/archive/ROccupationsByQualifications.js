/* eslint no-undef: "off" */

cube('ROccupationsByQualifications', {
  sql: 'SELECT TOP 100 PERCENT * FROM reporting.R_OCCUPATIONS_BY_QUALIFICATIONS ORDER BY Year DESC',
  title: 'OpenData - Професии по СПК',
  description: 'Служи за извършване на справки свързани с данни по професии по СПК.',

  joins: {},

  measures: {},

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
    col2: {
      sql: 'col2',
      type: 'number',
      title: 'Физически науки, информатика, техника, здравеопазване, опазване на околната среда и Добив и обогатяване на полезни изкопаеми - I ПКС',
    },
    col3: {
      sql: 'col3',
      type: 'number',
      title: 'Физически науки, информатика, техника, здравеопазване, опазване на околната среда и Добив и обогатяване на полезни изкопаеми - II ПКС',
    },
    col4: {
      sql: 'col4',
      type: 'number',
      title: 'Физически науки, информатика, техника, здравеопазване, опазване на околната среда и Добив и обогатяване на полезни изкопаеми - III ПКС',
    },
    col5: {
      sql: 'col5',
      type: 'number',
      title: 'Физически науки, информатика, техника, здравеопазване, опазване на околната среда и Добив и обогатяване на полезни изкопаеми - IV ПКС',
    },
    col6: {
      sql: 'col6',
      type: 'number',
      title: 'Услуги за личността - I ПКС',
    },
    col7: {
      sql: 'col7',
      type: 'number',
      title: 'Услуги за личността - II ПКС',
    },
    col8: {
      sql: 'col8',
      type: 'number',
      title: 'Услуги за личността - III ПКС',
    },
    col9: {
      sql: 'col9',
      type: 'number',
      title: 'Услуги за личността - IV ПКС',
    },
    col10: {
      sql: 'col10',
      type: 'number',
      title: 'Стопанско управление и администрация, социални услуги - I ПКС',
    },
    col11: {
      sql: 'col11',
      type: 'number',
      title: 'Стопанско управление и администрация, социални услуги - II ПКС',
    },
    col12: {
      sql: 'col12',
      type: 'number',
      title: 'Стопанско управление и администрация, социални услуги - III ПКС',
    },
    col13: {
      sql: 'col13',
      type: 'number',
      title: 'Стопанско управление и администрация, социални услуги - IV ПКС',
    },
    col14: {
      sql: 'col14',
      type: 'number',
      title: 'Производство и преработка, архитектура и строителство - I ПКС',
    },
    col15: {
      sql: 'col15',
      type: 'number',
      title: 'Производство и преработка, архитектура и строителство - II ПКС',
    },
    col16: {
      sql: 'col16',
      type: 'number',
      title: 'Производство и преработка, архитектура и строителство - III ПКС',
    },
    col17: {
      sql: 'col17',
      type: 'number',
      title: 'Производство и преработка, архитектура и строителство - IV ПКС',
    },
    col18: {
      sql: 'col18',
      type: 'number',
      title: 'Транспорт I ПКС',
    },
    col19: {
      sql: 'col19',
      type: 'number',
      title: 'Транспорт II ПКС',
    },
    col20: {
      sql: 'col20',
      type: 'number',
      title: 'Транспорт III ПКС',
    },
    col21: {
      sql: 'col21',
      type: 'number',
      title: 'Транспорт IV ПКС',
    },
    col22: {
      sql: 'col22',
      type: 'number',
      title: 'Селско стопанство, горско стопанство и рибно стопанство, ветеринарна медицина - I ПКС',
    },
    col23: {
      sql: 'col23',
      type: 'number',
      title: 'Селско стопанство, горско стопанство и рибно стопанство, ветеринарна медицина - II ПКС',
    },
    col24: {
      sql: 'col24',
      type: 'number',
      title: 'Селско стопанство, горско стопанство и рибно стопанство, ветеринарна медицина - III ПКС',
    },
    col25: {
      sql: 'col25',
      type: 'number',
      title: 'Селско стопанство, горско стопанство и рибно стопанство, ветеринарна медицина - IV ПКС',
    },
    col26: {
      sql: 'col26',
      type: 'number',
      title: 'Изкуства - I ПКС',
    },
    col27: {
      sql: 'col27',
      type: 'number',
      title: 'Изкуства - II ПКС',
    },
    col28: {
      sql: 'col28',
      type: 'number',
      title: 'Изкуства - III ПКС',
    },
    col29: {
      sql: 'col29',
      type: 'number',
      title: 'Изкуства - IV ПКС',
    },
    col30: {
      sql: 'col30',
      type: 'number',
      title: 'Други',
    },
  },

    dataSource: 'default',
});

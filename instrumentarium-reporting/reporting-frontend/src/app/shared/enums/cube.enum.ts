export enum CubeFilterMatchMode {
  EQUALS = 'equals', // string, number, time
  NOT_EQUALS = 'notEquals', // string, number, time
  SET = 'set', // string, number, time - checks if value is not NULL
  NOT_SET = 'notSet', // string, number, time - checks if value is NULL

  CONTAINS = 'contains', // string
  NOT_CONTAINS = 'notContains', // string
  STARTS_WITH = 'startsWith', // string
  ENDS_WITH = 'endsWith', // string

  GREATER_THAN = 'gt', //number
  GREATER_THAN_OR_EQUAL_TO = 'gte', //number
  LESS_THAN = 'lt', //number
  LESS_THAN_OR_EQUAL_TO = 'lte', //number

  IN_DATE_RANGE = 'inDateRange', // time - [from, to]
  NOT_IN_DATE_RANGE = 'notInDateRange', // time - [from - to]
  BEFORE_DATE = 'beforeDate', // time - [date]
  AFTER_DATE = 'afterDate' // time = [date]
}

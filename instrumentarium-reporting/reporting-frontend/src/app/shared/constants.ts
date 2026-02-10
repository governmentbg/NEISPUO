import { FilterMatchMode } from 'primeng/api';
import { CubeFilterMatchMode } from './enums/cube.enum';

export class CONSTANTS {
  /**
   * The maximum allowed limit for Cube.js is 50000
   * https://cube.dev/docs/query-format#query-properties
   */
  static CUBEJS_MAX_RESULTS = 50000;
  static PIE_CHART_MAX_DISPLAYED_RESULTS = 10;
  static PIE_CHART_LAST_SLICE_INDEX = 10;

  static CUSTOM_MATCH_MODE = {
    NOT_BETWEEN: 'notBetween' // Custom PrimeNG filter that matches CubeJS notInDateRange
  };

  static DATE_RANGE_MATCH_MODES = [FilterMatchMode.BETWEEN, this.CUSTOM_MATCH_MODE.NOT_BETWEEN];

  static PRIMENG_CUBEJS_MATCH_MODE_MAPPING = {
    //string
    [FilterMatchMode.EQUALS]: CubeFilterMatchMode.EQUALS,
    [FilterMatchMode.NOT_EQUALS]: CubeFilterMatchMode.NOT_EQUALS,
    [FilterMatchMode.CONTAINS]: CubeFilterMatchMode.CONTAINS,
    [FilterMatchMode.NOT_CONTAINS]: CubeFilterMatchMode.NOT_CONTAINS,
    [FilterMatchMode.STARTS_WITH]: CubeFilterMatchMode.STARTS_WITH,
    [FilterMatchMode.ENDS_WITH]: CubeFilterMatchMode.ENDS_WITH,

    //number
    [FilterMatchMode.EQUALS]: CubeFilterMatchMode.EQUALS,
    [FilterMatchMode.NOT_EQUALS]: CubeFilterMatchMode.NOT_EQUALS,
    [FilterMatchMode.LESS_THAN]: CubeFilterMatchMode.LESS_THAN,
    [FilterMatchMode.LESS_THAN_OR_EQUAL_TO]: CubeFilterMatchMode.LESS_THAN_OR_EQUAL_TO,
    [FilterMatchMode.GREATER_THAN]: CubeFilterMatchMode.GREATER_THAN,
    [FilterMatchMode.GREATER_THAN_OR_EQUAL_TO]: CubeFilterMatchMode.GREATER_THAN_OR_EQUAL_TO,

    //time
    /*https://cube.dev/docs/query-format#filters-operators-equals 
    --> Due to Cube documentation we should be able to filter time with equals and notEquals operators
    --> But it seems that there is bug in Cube and this is not possible
    */
    //[FilterMatchMode.DATE_IS]: CubeFilterMatchMode.EQUALS,
    //[FilterMatchMode.DATE_IS_NOT]: CubeFilterMatchMode.NOT_EQUALS,
    [FilterMatchMode.DATE_IS]: CubeFilterMatchMode.IN_DATE_RANGE,
    [FilterMatchMode.DATE_IS_NOT]: CubeFilterMatchMode.NOT_IN_DATE_RANGE,
    [FilterMatchMode.BETWEEN]: CubeFilterMatchMode.IN_DATE_RANGE,
    [this.CUSTOM_MATCH_MODE.NOT_BETWEEN]: CubeFilterMatchMode.NOT_IN_DATE_RANGE, //custom filter
    [FilterMatchMode.DATE_BEFORE]: CubeFilterMatchMode.BEFORE_DATE,
    [FilterMatchMode.DATE_AFTER]: CubeFilterMatchMode.AFTER_DATE
  };
}

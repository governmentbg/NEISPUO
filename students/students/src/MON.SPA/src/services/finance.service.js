import http from './http.service';
import QueryString from "query-string";

class FinanceService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/finance";
  }

  getNaturalIndicators(schoolYear, period) {
    return this.$http.get(`${this.baseUrl}/getNaturalIndicators?schoolYear=${schoolYear}&period=${period}`);
  }

  listPeriod(schoolYear, period) {
    const userListInput = {
        year: schoolYear,
        period: period
    };

    return this.$http.get(`${this.baseUrl}/ListPeriod?${QueryString.stringify(userListInput)}`);
  }

  listPeriods(periods, showItemPrice, showItemValue) {
    const userListInput = {
        periods: periods,
        showItemPrice: showItemPrice,
        showItemValue: showItemValue
    };

    return this.$http.get(`${this.baseUrl}/ListPeriods?${QueryString.stringify(userListInput)}`);
  }  

  getGridHeaders(){
    return this.$http.get(`${this.baseUrl}/getGridHeaders`);
  }

  listResourceSupportDataPeriods(periods) {
    const userListInput = {
        periods: periods,
    };

    return this.$http.get(`${this.baseUrl}/ListResourceSupportDataPeriods?${QueryString.stringify(userListInput)}`);
  }    

  getResourceSupportDataGridHeaders(){
    return this.$http.get(`${this.baseUrl}/getResourceSupportDataGridHeaders`);
  }  

}

export default new FinanceService();

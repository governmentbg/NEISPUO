import QueryString from "query-string";
import http from './http.service';

class OresService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/Ores";
  }

  getById(id){
    if (!id) {
        throw new Error('Id is required');
    }

    return this.$http.get(`${this.baseUrl}/GetById?id=${id}`);
  }

  create(model) {
    if (!model) {
      throw new Error('Model is required');
    }

    return this.$http.post(`${this.baseUrl}/Create`, model);
  }

  update(model){
    if (!model) {
        throw new Error('Model is required');
    }

    return this.$http.put( `${this.baseUrl}/Update`, model);
  }

  delete(id) {
    if (!id) {
      throw new Error('Id is required');
    }

    return this.$http.delete(`${this.baseUrl}/Delete?id=${id}`);
  }

  getCalendarDetails(start, end, regionId, institutionId) {
    const params = {};
    if (start) params.start = start;
    if (end) params.end = end;
    if (regionId) params.regionId = regionId;
    if (institutionId) params.institutionId = institutionId;

    return this.$http.get(`${this.baseUrl}/GetCalendarDetails?${QueryString.stringify(params)}`);
  }

  getOresRangeDropdownOptions(institutionId, classId, personId, schoolYear) {
    const params = {};
    if (institutionId) params.institutionId = institutionId;
    if (classId) params.classId = classId;
    if (personId) params.personId = personId;
    if (schoolYear) params.schoolYear = schoolYear;

    return this.$http.get(`${this.baseUrl}/GetOresRangeDropdownOptions?${QueryString.stringify(params)}`);
  }
}

export default new OresService();

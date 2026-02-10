import { getDate } from 'date-fns';
import http from './http.service';

class RegixService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/regix";
  }

  getEmploymentContracts(pin) {
    if (!pin) {
      throw new Error('PIN is required');
    }

    return this.$http.get(`${this.baseUrl}/GetEmploymentContracts/?egn=${pin}`);
  }

  getRelations(pin) {
    if (!pin) {
      throw new Error('PIN is required');
    }

    return this.$http.get(`${this.baseUrl}/GetRelations/?egn=${pin}`);
  }

  getValidPerson(pin) {
    if (!pin) {
      throw new Error('PIN is required');
    }

    return this.$http.get(`${this.baseUrl}/GetValidPerson/?egn=${pin}`);
  }

  getStateOfPlay(eik) {
    if (!eik) {
      throw new Error('EIK is required');
    }

    return this.$http.get(`${this.baseUrl}/GetStateOfPlay/?eik=${eik}`);
  }

  getActualState(eik) {
    if (!eik) {
      throw new Error('EIK is required');
    }

    return this.$http.get(`${this.baseUrl}/GetActualState/?eik=${eik}`);
  }

  getCompanyDetails(eik) {
    if (!eik) {
      throw new Error('EIK is required');
    }

    return this.$http.get(`${this.baseUrl}/GetCompanyDetails/?eik=${eik}`);
  }

  getForeignIdentity(pin) {
    if (!pin) {
      throw new Error('PIN is required');
    }

    return this.$http.get(`${this.baseUrl}/GetForeignIdentityInfo?lnch=${pin}`);
  }

  getPersonSearch(pin) {
    if (!pin) {
      throw new Error('PIN is required');
    }

    return this.$http.get(`${this.baseUrl}/GetPersonData/?egn=${pin}`);
  }

  getTemporaryAddress(pin, searchDate = new Date()){
    if (!pin) {
      throw new Error('PIN is required');
    }
    let searchDateStr = searchDate.toISOString().split('T')[0];

    return this.$http.get(`${this.baseUrl}/GetTemporaryAddress/?egn=${pin}&searchDate=${searchDateStr}`);
  }

  getPermanentAddress(pin, searchDate = getDate()){
    if (!pin) {
      throw new Error('PIN is required');
    }
    let searchDateStr = searchDate.toISOString().split('T')[0];

    return this.$http.get(`${this.baseUrl}/GetPermanentAddress/?egn=${pin}&searchDate=${searchDateStr}`);
  }

  sign(data){
    if (!data) {
      throw new Error('data is required');
    }

    return this.$http.get(`${this.baseUrl}/Sign?data=${data}`);
  }
}

export default new RegixService();

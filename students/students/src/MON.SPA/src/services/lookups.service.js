import QueryString from "query-string";
import http from './http.service';

class LookupsService {
  constructor() {
    this.$http = http;
    this.baseUrl = "/api/lookups";
  }

  getPinTypes() {
    return this.$http.get(`${this.baseUrl}/GetPinTypes`);
  }

  getCountries(searchStr) {
    return this.$http.get(`${this.baseUrl}/GetCountriesBySearchString/?searchStr=${searchStr}`);
  }

  getGenders() {
    return this.$http.get(`${this.baseUrl}/GetGenders`);
  }

  getGuardianTypes() {
    return this.$http.get(`${this.baseUrl}/GetGuardianTypes`);
  }

  getAddresses(searchStr) {
    return this.$http.get(`${this.baseUrl}/GetAddressesBySearchString/?searchStr=${searchStr}`);
  }

  getCittes(searchStr, selectedValue) {
    return this.$http.get(`${this.baseUrl}/GetCities/?searchStr=${searchStr}&selectedValue=${selectedValue}`);
  }

  getCommuterOptions() {
    return this.$http.get(`${this.baseUrl}/GetCommuterOptions`);
  }

  getClassGroupsOptions(institutionId, schoolYear, basicClass, minBasicClass, pid, isInitialEnrollment, filterForTeacher) {
    let url = `${this.baseUrl}/GetClassGroups?institutionId=${institutionId}&schoolYear=${schoolYear}`;

    if (pid){
      url += `&personId=${pid}`;
    }

    if (basicClass){
      url += `&basicClass=${basicClass}`;
    }
    if (minBasicClass) {
      url += `&minBasicClass=${minBasicClass}`;
    }
    if (isInitialEnrollment) {
      url += `&isInitialEnrollment=${isInitialEnrollment}`;
    }
    if (filterForTeacher) {
      url += `&filterForTeacher=${filterForTeacher}`;
    }

    return this.$http.get(url);
  }

  getClassGroupsForLoggedUser(schoolYear, pid) {
    let url = `${this.baseUrl}/GetClassGroupsForLoggedUser?schoolYear=${schoolYear}`;

    if(pid) {
      url += `&personId=${pid}`;
    }

    return this.$http.get(url);
  }

  getBasicClassesLimitForInstitution(institutionId) {
    if (!institutionId) return [];

    return this.$http.get(`${this.baseUrl}/GetBasicClassesLimitForInstitution?institutionId=${institutionId}`);
  }

  getRepeaterReasons(){
    return this.$http.get(`${this.baseUrl}/GetRepeaterReasons`);
  }

  getLanguages(){
    return this.$http.get(`${this.baseUrl}/GetLanguages`);
  }

  getScholarshipTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetScholarshipTypeOptions`);
  }

  getScholarshipAmountOptions(){
    return this.$http.get(`${this.baseUrl}/GetScholarshipAmountOptions`);
  }

  getStudentRelativeTypeOptions() {
    return this.$http.get(`${this.baseUrl}/GetStudentRelativeTypeOptions`);
  }

  getStudentRelativeWorkStatusOptions() {
    return this.$http.get(`${this.baseUrl}/GetStudentRelativeWorkStatusOptions`);
  }

  getSpecialNeedsTypesOptions(){
    return this.$http.get(`${this.baseUrl}/GetSpecialNeedsTypesOptions`);
  }

  getSpecialNeedsSubTypesOptions(){
    return this.$http.get(`${this.baseUrl}/GetSpecialNeedsSubTypesOptions`);
  }

  getResourceSupportTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetResourceSupportTypeOptions`);
  }

  getResourceSupportSpecialistWorkPlaces(){
    return this.$http.get(`${this.baseUrl}/GetResourceSupportSpecialistWorkPlaces`);
  }

  getResourceSupportSpecialistTypesOptions(){
    return this.$http.get(`${this.baseUrl}/GetResourceSupportSpecialistTypesOptions`);
  }

  getInstitutionOptions(searchStr, selectedValue) {
    return this.$http.get(`${this.baseUrl}/GetInstitutionOptions?searchStr=${searchStr}&selectedValue=${selectedValue}`);
  }

  getStudentTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetStudentTypeOptions`);
  }

  getSupportPeriodOptions(){
    return this.$http.get(`${this.baseUrl}/GetSupportPeriodOptions`);
  }

  getEarlyEvaluationReasonOptions(){
    return this.$http.get(`${this.baseUrl}/GetEarlyEvaluationReasonOptions`);
  }

  getCommonSupportTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetCommonSupportTypeOptions`);
  }

  getAdditionalSupportTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetAdditionalSupportTypeOptions`);
  }

  getSubjects(searchStr){
    return this.$http.get(`${this.baseUrl}/GetSubjectOptions?searchStr=${searchStr}`);
  }

  getSubjectsForLoggedInstitution(params){
    if(!params) return this.$http.get(`${this.baseUrl}/GetSubjectsForLoggedInstitution`);

    return this.$http.get(`${this.baseUrl}/GetSubjectsForLoggedInstitution?${QueryString.stringify(params)}`);
  }

  getNomenclatureData(tableName){
    return this.$http.get(`${this.baseUrl}/GetNomenclatureData?tableName=${tableName}`);
  }

  updateNomenclatureData(model){
    if (!model) {
        throw new Error('Model is required');
      }
    return this.$http.put(`${this.baseUrl}/UpdateNomenclatureData`,model);
  }

  deleteNomenclature(tableName, id){
      return this.$http.delete(`${this.baseUrl}/DeleteNomenclature?tableName=${tableName}&id=${id}`);
  }

  addNomenclature(tableName, nomenclature){
      return  this.$http.post(`${this.baseUrl}/AddNomenclature?tableName=${tableName}`,nomenclature);
  }

  updateNomenclature(tableName, nomenclature){
      return  this.$http.put(`${this.baseUrl}/UpdateNomenclature?tableName=${tableName}`,nomenclature);
  }

  getEducationTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetEducationTypeOptions`);
  }

  getDocumentEducationTypeOptions() {
    return this.$http.get(`${this.baseUrl}/GetDocumentEducationTypeOptions`);
  }


  getAdmissionReasonTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetAdmissionReasonTypeOptions`);
  }

  getDischargeReasonTypeOptions({ isForDischarge, isForRelocation } = {}){
    return this.$http.get(`${this.baseUrl}/GetDischargeReasonTypeOptions?isForDischarge=${isForDischarge ?? false}&isForRelocation=${isForRelocation ?? false}`);
  }

  getEducationFormOptions(){
    return this.$http.get(`${this.baseUrl}/GetEducationFormOptions`);
  }

  getValidEducationFormsOptionsForPerson(isNotPresentForm){
    if (isNotPresentForm === undefined || isNotPresentForm === null) {
      return this.$http.get(`${this.baseUrl}/GetValidEducationFormsOptionsForPerson`);
    }

    return this.$http.get(`${this.baseUrl}/GetValidEducationFormsOptionsForPerson?isNotPresentForm=${isNotPresentForm}`);
  }

  getEduFormsForLoggedUser(isNotPresentForm) {
    if (isNotPresentForm === undefined || isNotPresentForm === null) {
      return this.$http.get(`${this.baseUrl}/GetEduFormsForLoggedUser`);
    }

    return this.$http.get(`${this.baseUrl}/GetEduFormsForLoggedUser?isNotPresentForm=${isNotPresentForm}`);
  }

  getBasicClassOptions(params){
    if(!params) return this.$http.get(`${this.baseUrl}/GetBasicClassOptions`);

    return this.$http.get(`${this.baseUrl}/GetBasicClassOptions?${QueryString.stringify(params)}`);
  }

  getSchoolYears(params){
    return this.$http.get(`${this.baseUrl}/GetSchoolYears?${QueryString.stringify(params)}`);
  }

  getRegionById(regionId) {
    return this.$http.get(`${this.baseUrl}/GetRegionById?regionId=${regionId}`);
  }

  getGradesByBasicClassId(params) {
    if(!params) return this.$http.get(`${this.baseUrl}/GetGradesByBasicClassId`);

    return this.$http.get(`${this.baseUrl}/GetGradesByBasicClassId?${QueryString.stringify(params)}`);
  }

  getBasicDocumentTypes() {
    return this.$http.get(`${this.baseUrl}/GetBasicDocumentTypes`);
  }

  getLodEvaluationResults() {
    return this.$http.get(`${this.baseUrl}/GetLodEvaluationResults`);
  }

  getBasicSubjectTypeOptions() {
    return this.$http.get(`${this.baseUrl}/GetBasicSubjectTypeOptions`);
  }

  getLodEvaluationsMajorCourses(personId, schoolYear) {
    return this.$http.get(`${this.baseUrl}/GetLodEvaluationsProfileClasses?personId=${personId}&schoolYear=${schoolYear}`);
  }

  getStudentAwardTypes() {
    return this.$http.get(`${this.baseUrl}/GetStudentAwardTypes`);
  }

  getStudentSanctionTypes() {
    return this.$http.get(`${this.baseUrl}/GetStudentSanctionTypes`);
  }

  getAwardCategories() {
    return this.$http.get(`${this.baseUrl}/GetAwardCategories`);
  }

  getFounders() {
    return this.$http.get(`${this.baseUrl}/GetFounders`);
  }

  getAwardReasons() {
    return this.$http.get(`${this.baseUrl}/GetAwardReasons`);
  }

  getScholarshipFinancingOrgans(){
    return this.$http.get(`${this.baseUrl}/GetScholarshipFinancingOrgans`);
  }

  getSupportingEquipment(){
    return this.$http.get(`${this.baseUrl}/GetSupportingEquipment`);
  }

  getStudentPositionOptions() {
    return this.$http.get(`${this.baseUrl}/GetStudentPositionOptions`);
  }

  getStudentPositionOptionsByCondition(params) {
    return this.$http.get(`${this.baseUrl}/GetStudentPositionOptionsByCondition?${QueryString.stringify(params)}`);
  }

  getStudentSelfGovernmentPositions() {
    return this.$http.get(`${this.baseUrl}/GetStudentSelfGovernmentPositions`);
  }

  getStudentParticipations() {
    return this.$http.get(`${this.baseUrl}/GetStudentParticipations`);
  }

  getSubjectTypeOptions(params){
    return this.$http.get(`${this.baseUrl}/GetSubjectTypeOptions?${QueryString.stringify(params)}`);
  }

  getExternalEvaluationTypeOptions(){
    return this.$http.get(`${this.baseUrl}/GetExternalEvaluationTypeOptions`);
  }

  getSchoolYearsForPerson(personId){
    return this.$http.get(`${this.baseUrl}/GetSchoolYearsForPerson?personId=${personId}`);
  }

  getAvailableArchitecture(){
    return this.$http.get(`${this.baseUrl}/GetAvailableArchitecture`);
  }

  getSubjectDetailsOptions() {
    return this.$http.get(`${this.baseUrl}/GetSubjecDetailsOptions`);
  }

  getORESTypesOptions() {
    return this.$http.get(`${this.baseUrl}/GetORESTypesOptions`);
  }

  getValidSpecialityOptions(){
    return this.$http.get(`${this.baseUrl}/GetValidSpecialityOptions`);
  }

  getProfessionOptions(){
    return this.$http.get(`${this.baseUrl}/GetSPPOOProfession`);
  }

  getClassTypeOptions(basicClassId) {
    return this.$http.get(`${this.baseUrl}/GetClassTypes?basicClassId=${basicClassId}`);
  }

  getSpecialNeedGradeOptions(){
    return this.$http.get(`${this.baseUrl}/GetSpecialNeedGradeOptions`);
  }

  getGradeOptions(){
    return this.$http.get(`${this.baseUrl}/GetGradeOptions`);
  }

  getOtherGradeOptions(){
    return this.$http.get(`${this.baseUrl}/GetOtherGradeOptions`);
  }

  getQualitativeGradeOptions(){
    return this.$http.get(`${this.baseUrl}/GetQualitativeGradeOptions`);
  }

  getCurriculumPartOptions(params) {
    if(!params) return this.$http.get(`${this.baseUrl}/GetCurriculumPartOptions`);

    return this.$http.get(`${this.baseUrl}/GetCurriculumPartOptions?${QueryString.stringify(params)}`);
  }

  getStudentSessionOptions(){
    return this.$http.get(`${this.baseUrl}/GetStudentSessionOptions`);
  }
}

export default new LookupsService();

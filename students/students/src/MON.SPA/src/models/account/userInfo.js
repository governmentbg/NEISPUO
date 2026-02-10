import {ClassGroupInfo} from './classGroupInfo';
export class UserInfo{
  constructor(obj = {})
  {
    this.institution = obj.institution || '';
    this.region = obj.region || '';
    this.municipality = obj.municipality || '';
    this.person = obj.person || '';
    this.budget = obj.budget;
    this.userId = obj.sysUserID;
    this.role = obj.sysRoleID;
    this.institutionId = obj.institutionID || 0;
    this.regionId = obj.regionId || 0;
    this.municipalityId = obj.municipalityID || 0;
    this.budgetId = obj.budgetingInstitutionID || 0;
    this.personId = obj.personId || 0;
    this.roleName = obj.roleName || '';
    this.baseSchoolTypeId = obj.baseSchoolTypeId || 0;
    this.address = obj.address || '';
    this.instType = obj.instType;
    this.instTypeId = obj.instTypeId;
    this.isLeadTeacher = obj.isLeadTeacher || false;
    this.currentSchoolYear = obj.currentSchoolYear || 0;
    this.leadTeacherClassGroups = obj.leadTeacherClassGroups ? obj.leadTeacherClassGroups.map(el => new ClassGroupInfo(el)) : [];
  }
}

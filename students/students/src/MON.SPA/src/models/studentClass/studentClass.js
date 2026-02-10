import Constants from '@/common/constants.js';
import { StudentClassEditModel } from "./studentClassEditModel";

export class StudentClass extends StudentClassEditModel {
  constructor(obj = {}, moment = {}) {
    super(obj);
    this.status = obj.status | 0;
    this.repeaterReason = obj.repeaterReason || '';
    this.classGroupId = obj.classGroupId || 0;
    this.commuterTypeName = obj.commuterTypeName || '';
    this.enrollmentDate = (obj.enrollmentDate && moment) ? moment(obj.enrollmentDate).format("DD.MM.YYYY") : '';
    this.admissionDocumentId = obj.admissionDocumentId || null;
    this.positionId = obj.positionId || null;
    this.position = obj.position || '';
    this.isCurrent = obj.isCurrent;
    this.statusName = obj.statusName || '';
    this.hasHistory = obj.hasHistory;
    this.createdBySysUser = obj.createdBySysUser;
    this.createDate = obj.createDate;
    this.modifiedBySysUser = obj.modifiedBySysUser;
    this.modifyDate = obj.modifyDate;
    this.studentEduFormName = obj.studentEduFormName || '';
    this.studentEduFormId = obj.studentEduFormId;
    this.studentSpeciality = obj.studentSpeciality || '';
    this.studentSpecialityId = obj.studentSpecialityId;
    this.studentProfessionId = obj.studentProfessionId;
    this.studentProfession = obj.studentProfession;
    this.basicClassId = obj.basicClassId;
    this.selectedClassTypeId = obj.selectedClassTypeId;
    this.classGroups = obj.classGroups;
    this.notPresentFormClassFilter = obj.notPresentFormClassFilter;
    this.basicClassName = obj.basicClassName;
    this.isNotPresentForm = obj.isNotPresentForm;
    this.isBasicClassForCurrentInstitution = obj.isBasicClassForCurrentInstitution;
    this.isAdditionalClass = obj.isAdditionalClass;
    this.canAddCurriculum = obj.canAddCurriculum;
    this.isHourlyOrganization = obj.isHourlyOrganization;
    this.enrollmentDate = obj.enrollmentDate;
    this.canBeDeleted = obj.canBeDeleted;
    this.entryDate = (obj.entryDate && moment) ? moment(obj.entryDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.dischargeDate = (obj.dischargeDate && moment) ? moment(obj.dischargeDate).format(Constants.DATEPICKER_FORMAT) : '';
    this.hasExternalSoProvider = obj.hasExternalSoProvider;
    this.hasExternalSoProviderClassTypeLimitation = obj.hasExternalSoProviderClassTypeLimitation;
    this.canChangePosition = obj.canChangePosition;
    this.changeTargetPositionId = obj.changeTargetPositionId;
  }
}

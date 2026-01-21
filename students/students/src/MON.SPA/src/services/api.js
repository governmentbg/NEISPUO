import absence from './absence.service';
import absenceCampaign from './absenceCampaign.service';
import admin from './admin.service';
import administration from './administration.service';
import admissionDocument from './admissionDocument.service';
import appConfiguration from './appConfiguration.service';
import asp from './asp.service';
import barcodeYear from './barcodeYear.service';
import basicDocument from './basicDocument.service';
import biss from './biss.service';
import certificate from './certificate.service';
import classGroup from './classGroup.service';
import diploma from './diploma.service';
import diplomaCreateRequest from './diplomaCreateRequest.service';
import diplomaTemplate  from './diplomaTemplate.service';
import diplomaValidationExclusion from './diplomaValidationExclusion.service';
import dischargeDocument from './dischargeDocument.service';
import document from './document.service';
import dynamicForm from './dynamicForm.service';
import environmentCharacteristics from'./environmentCharacteristics.service';
import equalization from './equalization.service';
import error from './error.service';
import externalEvaluation from './externalEvaluation.service';
import healthInsurance from './healthInsurance.service';
import finance from './finance.service';
import institution from './institution.service';
import localServer from './localServer.service';
import lodFinalization from './lodFinalization.service';
import lookups from './lookups.service';
import message from './message.service';
import note from './note.service';
import otherDocument from './otherDocument.service';
import otherInstitution from './otherInstitution.service';
import preSchool from './preSchool.service';
import printTemplate from './printTemplate.service';
import recognition from './recognition.service';
import refugee from './refugee.service';
import regix from './regix.service';
import relocationDocument from './relocationDocument.service';
import resourceSupport from  './resourceSupport.service';
import scanner from './scanner.service';
import scholarship from './scholarship.service';
import schoolTypeLodAccess from './schoolTypeLodAccess.service';
import selfGovernment from './selfGovernment.service';
import student from './student.service';
import studentAdmissionDocumentPermissionRequest from './studentAdmissionDocumentPermissionRequest.service';
import studentAwards from './studentAwards.service';
import studentClass from './studentClass.service';
import studentInternationalMobility from './studentInternationalMobility.service';
import studentLod from './studentLod.service';
import studentPDS from './studentPDS.service';
import studentSOP from './studentSOP.service';
import studentSanctions from './studentSanctions.service';
import curriculum from './curriculum.service';
import leadTeacher from './leadTeacher.service';
import trace from './trace.service';
import ores from './ores.service';
import earlyAssessment from './earlyAssessment.service';
import lodAssessment from './lodAssessment.service';
import lodAssessmentTemplate from './lodAssessmentTemplate.service';
import studentCommonPDS from './studentCommonPDS.service';
import studentAdditionalPDS from './studentAdditionalPDS.service';
import reassessment from './reassessment.service';
import docManagementCampaign from './docManagementCampaign.service';
import docManagementApplication from './docManagementApplication.service';
import docManagementAdditionalCampaign from './docManagementAdditionalCampaign.service';
import docManagementExchange from './docManagementExchange.service';


export default {
  student: student,
  lookups: lookups,
  document: document,
  studentClass: studentClass,
  otherInstitution: otherInstitution,
  scholarship: scholarship,
  studentLod: studentLod,
  studentSOP: studentSOP,
  resourceSupport,
  environmentCharacteristics,
  institution,
  absence,
  admin,
  diploma: diploma,
  preSchool: preSchool,
  externalEvaluation: externalEvaluation,
  studentAwards: studentAwards,
  dischargeDocument: dischargeDocument,
  administration: administration,
  studentSanctions: studentSanctions,
  admissionDocument: admissionDocument,
  dynamicForm: dynamicForm,
  relocationDocument: relocationDocument,
  barcodeYear: barcodeYear,
  schoolTypeLodAccess: schoolTypeLodAccess,
  basicDocument: basicDocument,
  otherDocument: otherDocument,
  recognition: recognition,
  equalization: equalization,
  scanner: scanner,
  selfGovernment: selfGovernment,
  studentInternationalMobility: studentInternationalMobility,
  appConfiguration: appConfiguration,
  classGroup: classGroup,
  printTemplate: printTemplate,
  studentPDS: studentPDS,
  studentAdmissionDocumentPermissionRequest: studentAdmissionDocumentPermissionRequest,
  asp: asp,
  lodFinalization: lodFinalization,
  message: message,
  certificate: certificate,
  error: error,
  localServer: localServer,
  absenceCampaign: absenceCampaign,
  note: note,
  biss: biss,
  trace: trace,
  regix: regix,
  refugee: refugee,
  diplomaValidationExclusion: diplomaValidationExclusion,
  diplomaTemplate: diplomaTemplate,
  healthInsurance: healthInsurance,
  finance: finance,
  diplomaCreateRequest: diplomaCreateRequest,
  curriculum: curriculum,
  leadTeacher: leadTeacher,
  ores: ores,
  earlyAssessment: earlyAssessment,
  lodAssessment: lodAssessment,
  lodAssessmentTemplate: lodAssessmentTemplate,
  studentCommonPDS: studentCommonPDS,
  studentAdditionalPDS: studentAdditionalPDS,
  reassessment,
  docManagementCampaign: docManagementCampaign,
  docManagementApplication: docManagementApplication,
  docManagementAdditionalCampaign: docManagementAdditionalCampaign,
  docManagementExchange: docManagementExchange
};

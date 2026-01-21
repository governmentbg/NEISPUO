export const NotificationSeverity = {
  Info: 'info',
  Success: 'success',
  Warn: 'warn',
  Error: 'error',
};

export const NotificationSeverityColor = {
  Info: 'info',
  Success: 'success',
  Warn: 'orange',
  Error: 'error',
};

export const NotificationPosition = {
  TopRight: 'top right',
  TopCenter: 'top center',
  BottomRight: 'bottom right',
  BottomCenter: 'bottom center',
  Default: 'bottom right',
};

export const Severity = {
  Debug: 0,
  Info: 1,
  Warning: 2,
  Error: 3
};

export const UserRole = {
  School: 0,
  Mon: 1,
  Ruo: 2,
  Municipality: 3,
  FinancingInstitution: 4,
  Teacher: 5,
  Student: 6,
  Parent: 7,
  RuoExpert: 9,
  Cioo: 10,
  ExternalExpert: 11,
  MonExpert: 12,
  InstitutionAssociate: 14, // като School
  MonOBGUM: 15,
  MonOBGUM_Finance: 16,
  MonHR: 17,
  Consortium: 18,
  Accountant: 20
};

export const AuditModule = {
  Students: 201,
  RegisterDiploma: 202,
  Mobile: 203,
  Helpdesk: 204
};

export const StudentPosition = {
  Staff: 2, // персонал
  Student: 3, // учащ (училище/ДГ)
  StudentOtherInstitution: 7, // учащ (друга институция)
  StudentPersDeveleopmentSupport: 8, // учащ (ПЛР)
  Discharged: 9, // отписан
  StudentSpecialNeeds: 10 // учащ (ЦСОП)
};

export const Permissions = {
  PermissionNameForStudentModuleUse: 'Permission_StudentModule_Use',
  PermissionNameForNomenclatureRead: 'Permission_Nomenclature_Read',
  PermissionNameForNomenclatureCreate: 'Permission_Nomenclature_Create',
  PermissionNameForNomenclatureUpdate: 'Permission_Nomenclature_Update',
  PermissionNameForNomenclatureDelete: 'Permission_Nomenclature_Delete',
  PermissionNameForNomenclatureManage: 'Permission_Nomenclature_Manage',
  PermissionNameForStudentSearch: 'Permission_Student_Search',
  PermissionNameForStudentCreate: 'Permission_Student_Create',
  PermissionNameForSettingsSectionShow: 'Permission_SettingsSection_Show',
  PermissionNameForDiplomasShow: 'Permission_DiplomasSection_Show',
  PermissionNameForDemonstrationSectionShow: 'Permission_DemonstrationSection_Show',
  PermissionNameForAdministrationSectionShow: 'Permission_AdministrationSection_Show',
  PermissionNameForStudentLodDocumentsShow: 'Permission_StudentLodDocuments_Show',
  PermissionNameForAuditLogsShow: 'Permission_AuditLogs_Show',
  PermissionNameForCacheServiceManage: 'Permission_CacheService_Manage',
  PermissionNameForDocumentationShow: 'Permission_Documentation_Show',
  PermissionNameForStudentClassDetailsShow: 'Permission_StudentClassDetails_Show',
  PermissionNameForDiplomaBarcodesShow: 'Permission_DiplomaBarcodes_Show',
  PermissionNameForDipomasBarcodesAdd: 'Permission_DiplomaBarcodes_Add',
  PermissionNameForDiplomasBarcodesEdit: 'Permission_DiplomaBarcodes_Edit',
  PermissionNameForBasicDocumentsListShow: 'Permission_BasicDocuments_Show',
  PermissionNameForBasicDocumentEdit: 'Permission_BasicDocument_Edit',
  PermissionNameForLodAccessConfigurationShow: 'Permission_LodAccessConfiguration_Show',
  PermissionNameForLodAccessConfigurationEdit: 'Permission_LodAccessConfiguration_Edit',
  PermissionNameForBasicDocumentTeplatesShow: 'Permission_BasicDocumentTeplates_Show',

  // Шаблони на дипломи/документи
  PermissionNameForDiplomaTemplatesRead: 'Permission_DiplomaTemplates_Read',
  PermissionNameForDiplomaTemplatesManage: 'Permission_DiplomaTemplates_Manage',

  // Определя правото за управление на настройките на приложението за дадена институция
  PermissionNameForTenantAppSettingsManage: 'Permission_TenantAppSettings_Manage',

  // Шаблони за печат
  PermissionNameForTemplatesSectionShow: 'Permission_TemplatesSection_Show',
  PermissionNameForPrintTemplatesShow: 'Permission_PrintTemplates_Show',


  PermissionNameForAdmissionPermissionRequestRead: 'Permission_AdmissionPermissionRequest_Read',
  PermissionNameForAdmissionPermissionRequestManage: 'Permission_AdmissionPermissionRequest_Manage',


  PermissionNameForContextualInformationManage: 'Permission_ContextualInformation_Manage',

  PermissionNameForAzureAccountManage: 'Permission_AzureAccount_Manage',


  // Меню 'Меню 'ЛОД/Данни за институцията'
  PermissionNameForStudentCurrentInstitutionDetailsRead: 'Permission_StudentCurrentInstitutionDetails_Read',
  // Край


  // Меню 'Меню 'ЛОД/Общи данни за обучението'
  PermissionNameForStudentGeneralTrainingDataRead: 'Permission_StudentGeneralTrainingData_Read',
  // Край

  // Меню 'ЛОД/Предучилищна подготовка'
  PermissionNameForStudentPreSchoolEvaluationRead: 'Permission_StudentPreSchoolEvaluation_Read',
  PermissionNameForStudentPreSchoolEvaluationManage: 'Permission_StudentPreSchoolEvaluation_Manage',
  // Край

  // Меню 'ЛОД/Оценки'
  PermissionNameForStudentEvaluationRead: 'Permission_StudentEvaluation_Read',
  PermissionNameForStudentEvaluationManage: 'Permission_StudentEvaluation_Manage',
  PermissionNameForStudentEvaluationFinalize: 'Permission_StudentEvaluation_Finalize',
  // Край

  // Меню 'ЛОД/НВО-ДЗИ'
  PermissionNameForStudentExternalEvaluationRead: 'Permission_StudentExternalEvaluation_Read',
  PermissionNameForStudentExternalEvaluationManage: 'Permission_StudentExternalEvaluation_Manage',
  // Край

  // Меню 'ЛОД/Награди'
  PermissionNameForStudentAwardRead: 'Permission_StudentAward_Read',
  PermissionNameForStudentAwardManage: 'Permission_StudentAward_Manage',
  // Край

  // Грид 'Ученици със СОП' в дашборда
  PermissionNameForSopEnrollmentDetailsRead: 'Permission_SopEnrollmentDetails_Read',

  // Меню 'ЛОД/Санкции'
  PermissionNameForStudentSanctionRead: 'Permission_StudentSanction_Read',
  PermissionNameForStudentSanctionManage: 'Permission_StudentSanction_Manage',
  // Край

  // Меню 'ЛОД/Подкрепа за личностно развитие'
  PermissionNameForStudentPersonalDevelopmentRead: 'Permission_StudentPersonalDevelopment_Read',
  PermissionNameForStudentPersonalDevelopmentManage: 'Permission_StudentPersonalDevelopment_Manage',
  // Край

  // Меню 'ЛОД/СОП'
  PermissionNameForStudentSopRead: 'Permission_StudentSop_Read',
  PermissionNameForStudentSopManage: 'Permission_StudentSop_Manage',
  // Край

  // Меню 'ЛОД/Ученическо самоуправление'
  PermissionNameForStudentSelfGovernmentRead: 'Permission_StudentSelfGovernment_Read',
  PermissionNameForStudentSelfGovernmentManage: 'Permission_StudentSelfGovernment_Manage',
  // Край

  // Меню 'ЛОД/Международна мобилност'
  PermissionNameForStudentInternationalMobilityRead: 'Permission_StudentInternationalMobility_Read',
  PermissionNameForStudentInternationalMobilityManage: 'Permission_StudentInternationalMobility_Manage',
  // Край

  // Меню 'Дипломи'
  PermissionNameForStudentDiplomaRead: 'Permission_StudentDiploma_Read',
  PermissionNameForStudentDiplomaManage: 'Permission_StudentDiploma_Manage',
  PermissionNameForStudentDiplomaFinalize: 'Permission_StudentDiploma_Finalize',
  PermissionNameForStudentDiplomaAnnulment: 'Permission_StudentDiploma_Annulment',
  PermissionNameForStudentDiplomaImportValidationExclusionsManage: 'Permission_StudentDiplomaImportValidationExclusions_Manage',
  PermissionNameForInstitutionDiplomaRead: 'Permission_InstitutionDiploma_Read',
  PermissionNameForInstitutionDiplomaManage: 'Permission_InstitutionDiploma_Manage',
  PermissionNameForAdminDiplomaRead: 'Permission_AdminDiploma_Read',
  PermissionNameForAdminDiplomaManage: 'Permission_AdminDiploma_Manage',
  PermissionNameForDiplomaCreateRequestRead: 'Permission_DiplomaCreateRequest_Read',
  PermissionNameForDiplomaCreateRequestManage: 'Permission_DiplomaCreateRequest_Manage',
  PermissionNameForMonHrDiplomaRead: 'Permission_MonHrDiploma_Read',
  PermissionNameForMonHrDiplomaManage: 'Permission_MonHrDiploma_Manage',
  PermissionNameForRuoHrDiplomaRead: 'Permission_RouHrDiploma_Read',
  PermissionNameForRuoHrDiplomaManage: 'Permission_RouHrDiploma_Manage',
  // Номерация за дипломи
  PermissionNameForBasicDocumentSequenceRead: 'Permission_BasicDocumentSequence_Read',
  PermissionNameForBasicDocumentSequenceManage: 'Permission_BasicDocumentSequence_Manage',

  // Меню Класни ръководители
  PermissionNameForLeadTeacherManage: 'Permission_LeadTeacher_Manage',

    // Права за четене/редакция на дипломи, получени чрез създаването на заявление за създаване на документ/диплома.
    // Различават се от PermissionNameForStudentDiplomaRead и PermissionNameForStudentDiplomaManage по това,
    // че трябва да скриваме дипломите на ученика от други институции.
  PermissionNameForStudentDiplomaByCreateRequestRead: 'Permission_StudentDiplomaByCreateRequest_Read',
  PermissionNameForStudentDiplomaByCreateRequestManage: 'Permission_StudentDiplomaByCreateRequest_Manage',
  // Край

  // Меню 'Приравняване'
  PermissionNameForStudentRecognitionRead: 'Permission_StudentRecognition_Read',
  PermissionNameForStudentRecognitionManage: 'Permission_StudentRecognition_Manage',
  // Край

  // Меню 'Приравняване'
  PermissionNameForStudentEqualizationRead: 'Permission_StudentEqualization_Read',
  PermissionNameForStudentEqualizationManage: 'Permission_StudentEqualization_Manage',
  // Край

  // Меню 'Изпити за промяна на оценка'
  PermissionNameForStudentReassessmentRead: 'Permission_StudentReassessment_Read',
  PermissionNameForStudentReassessmentManage: 'Permission_StudentReassessment_Manage',
  // Край

  PermissionNameForStudentRecognitionUpdate: 'Permission_StudentRecognition_Update',
  PermissionNameForStudentRecognitionDelete: 'Permission_StudentRecognition_Delete',
  // Край

  // Меню 'Характеристика на средата'
  PermissionNameForStudentEnvironmentCharacteristicRead: 'Permission_StudentEnvironmentCharacteristic_Read',
  PermissionNameForStudentEnvironmentCharacteristicManage: 'Permission_StudentEnvironmentCharacteristic_Manage',
  // Край

  // Меню 'Ресурсно подпомагане'
  PermissionNameForStudentResourceSupportRead: 'Permission_StudentResourceSupport_Read',
  PermissionNameForStudentResourceSupportManage: 'Permission_StudentResourceSupport_Manage',
  // Край

  // Меню 'Стипендии'
  PermissionNameForStudentScholarshipRead: 'Permission_StudentScholarship_Read',
  PermissionNameForStudentScholarshipManage: 'Permission_StudentScholarship_Manage',
  // Край

  // Меню 'Други документи'
  PermissionNameForStudentOtherDocumentRead: 'Permission_StudentOtherDocument_Read',
  PermissionNameForStudentOtherDocumentManage: 'Permission_StudentOtherDocument_Manage',
  // Край

  // Меню 'Бележки'
  PermissionNameForStudentNoteRead: 'Permission_StudentNote_Read',
  PermissionNameForStudentNoteManage: 'Permission_StudentNote_Manage',
  // Край

  // Меню 'Други институции'
  PermissionNameForStudentOtherInstitutionRead: 'Permission_StudentOtherInstitution_Read',
  PermissionNameForStudentOtherInstitutionManage: 'Permission_StudentOtherInstitution_Manage',
  // Край

  // StudentClass. Бутон класове в картата с профилните данни на ученик
  PermissionNameForStudentClassRead: 'Permission_StudentClasses_Read',
  PermissionNameForStudentClassUpdate: 'Permission_StudentClass_Update',
  PermissionNameForStudentClassHistoryRead: 'Permission_StudentClassHistory_Read',
  PermissionNameForStudentClassHistoryDelete: 'Permission_StudentClassHistory_Delete',
  PermissionNameForStudentClassMassEnrolmentManage: 'Permission_StudentClass_MassEnrollment_Manage', // Масово записване и отписване на ученици от паралелка в ЦПЛР
  // Край

  // Меню 'Лични данни за детето/ученика'
  PermissionNameForStudentPersonalDataRead: 'Permission_StudentPersonalData_Read',
  PermissionNameForStudentPartialPersonalDataRead: 'Permission_StudentPartialPersonalData_Read',
  PermissionNameForStudentPersonalDataManage: 'Permission_StudentPersonalData_Manage',
  PermissionNameForStudentEducationRead: 'Permission_StudentEducation_Read',
  PermissionNameForStudentInternationalProtectionRead: 'Permission_StudentInternationalProtection_Read',
  PermissionNameForStudentInternationalProtectionManage: 'Permission_StudentInternationalProtection_Manage',
  // Край

  // Меню 'Движение на ученика' => 'Документи за записване'
  PermissionNameForStudentAdmissionDocumentRead: 'Permission_StudentAdmissionDocument_Read',
  PermissionNameForStudentAdmissionDocumentCreate: 'Permission_StudentAdmissionDocument_Create',
  PermissionNameForStudentAdmissionDocumentUpdate: 'Permission_StudentAdmissionDocument_Update',
  PermissionNameForStudentAdmissionDocumentDelete: 'Permission_StudentAdmissionDocument_Delete',
  PermissionNameForStudentToClassEnrollment: 'Permission_StudentToClass_Enrollment',
  // Край

  // Меню 'Движение на ученика' => 'Документи за отписване'
  PermissionNameForStudentDischargeDocumentRead: 'Permission_StudentDischargeDocument_Read',
  PermissionNameForStudentDischargeDocumentCreate: 'Permission_StudentDischargeDocument_Create',
  PermissionNameForStudentDischargeDocumentUpdate: 'Permission_StudentDischargeDocument_Update',
  PermissionNameForStudentDischargeDocumentDelete: 'Permission_StudentDischargeDocument_Delete',
  // Край

  // Меню 'Движение на ученика' => 'Документи за преместване'
  PermissionNameForStudentRelocationDocumentRead: 'Permission_StudentRelocationDocument_Read',
  PermissionNameForStudentRelocationDocumentCreate: 'Permission_StudentRelocationDocument_Create',
  PermissionNameForStudentRelocationDocumentUpdate: 'Permission_StudentRelocationDocument_Update',
  PermissionNameForStudentRelocationDocumentDelete: 'Permission_StudentRelocationDocument_Delete',
  // Край

  // Подписване и прикачване на ЛОД в документ за преместване/ освобождаване
  PermissionNameForStudentRelocationDocumentSignLOD: "Permission_StudentRelocationDocument_Sign_LOD",
  PermissionNameForStudentDischargeDocumentSignLOD: "Permission_StudentDischargeDocument_Sign_LOD",
  // Край


  // Меню 'Списъци'(Образователни институции) => Детайли, Списък с ученици
  PermissionNameForInstitutionRead: 'Permission_Institution_Read',
  PermissionNameForInstitutionClassesRead: 'Permission_InstitutionClasses_Read',
  PermissionNameForInstitutionStudentsRead: 'Permission_InstitutionStudents_Read',
  // Край

  // Меню 'Списъци'(Списък паралeлки/групи) => Отсъствия, Автоматично номериране, Печат
  PermissionNameForClassManage: 'Permission_Class_Manage',
  PermissionNameForClassStudentsRead: 'Permission_ClassStudents_Read',
  PermissionNameForClassAbsenceRead: 'Permission_ClassAbsence_Read',
  PermissionNameForClassAbsenceManage: 'Permission_ClassAbsence_Manage',
  // Край

  // Отсъствия за ученик
  PermissionNameForStudentAbsenceRead: 'Permission_StudentAbsence_Read',
  PermissionNameForStudentAbsenceManage: 'Permission_StudentAbsence_Manage',
  PermissionNameForStudentAbsenceCampaignRead: 'Permission_StudentAbsenceCampaign_Read',
  PermissionNameForStudentAbsenceCampaignManage: 'Permission_StudentAbsenceCampaign_Manage',
  PermissionNameForAbsencesExportRead: 'Permission_AbsencesExport_Read',
  PermissionNameForAbsencesExportManage: 'Permission_AbsencesExport_Manage',

  // Заявления за търсещи закрила
  PermissionNameForRefugeeApplicationsRead: 'Permission_RefugeeApplications_Read',
  PermissionNameForRefugeeApplicationsManage: 'Permission_RefugeeApplications_Manage',
  PermissionNameForRefugeeApplicationsUnlock: 'Permission_RefugeeApplications_Unlock',

  // Меню ASP
  PermissionNameForASPImport: 'Permission_ASP_Import',
  PermissionNameForASPBenefitDetailsRead: 'Permission_ASP_BenefitDetails_Read',
  PermissionNameForASPManage: 'Permission_ASP_Manage',
  PermissionNameForASPImportDetailsShow: 'Permission_ASP_ImportDetailsShow',
  PermissionNameForASPBenefitUpdate: 'Permission_ASP_BenefitUpdate',
  PermissionNameForASPEnrolledStudentsExport: 'Permission_ASP_EnrolledStudentsExport',
  PermissionNameForASPAdministrationManage: 'Permission_ASP_Administration_Manage',

  // Меню Здравно осигуряване
  PermissionNameForHealthInsuranceManage: 'Permission_HealthInsurance_Manage',

  // Меню Финанси
  PermissionNameForNaturalIndicatorsManage: 'Permission_NaturalIndicators_Manage',
  PermissionNameForResourceSupportDataManage: 'Permission_ResourceSupportData_Manage',

  // Бутони за одобрение и финализиране на ЛОД
  PermissionNameForLodStateManage: 'PermissionName_LodState_Manage',

  //Меню Сертификати
  PermissionNameForCertificatesManage: 'Permission_Certificates_Manage',
  PermissionNameForCertificatesRead: 'Permission_Certificates_Read',

  //Бутон Информация за свързани записи
  PermissionNameDataReferencesRead: 'Permission_DataReferences_Read',
  PermissionNameDataReferencesReadManage: 'Permission_DataReferences_Manage',

  // Меню ОРЕС
  PermissionNameForOresRead: 'Permission_Ores_Read',
  PermissionNameForOresManage: 'Permission_Ores_Manage',

   // Импорт на оценки
   PermissionNameForLodAssessmentImport: 'Permission_LodAssessment_Import',

   // Меню "Приключване на ЛОД"
   PermissionNameForStudentLodFinalizationRead: 'Permission_StudentLodFinalization_Read',
   PermissionNameForLodFinalizationRead: 'Permission_LodFinalization_Read',
   PermissionNameForLodFinalizationAdministration: 'Permission_LodFinalization_Administration',

   // Меню "Меню Шаблони за СФО"
   PermissionNameForLodAssessmentTemplateRead: 'Permission_LodAssessmentTemplate_Read',
   PermissionNameForLodAssessmentTemplateManage: 'Permission_LodAssessmentTemplate_Manage',

   PermissionNameForDualFormRead: 'Permission_DualForm_Read',

   // Бутон(в timeline-а) за създаване/преглед на учебен план
   PermissionNameForStudentCurriculumRead: 'Permission_StudentCurriculum_Read',
   PermissionNameForStudentCurriculumManage: 'Permission_StudentCurriculum_Manage',

   //Управление на документи с фабрична номерация
    PermissionNameForDocManagementCampaignRead: 'Permission_DocManagementCampaign_Read',
    PermissionNameForDocManagementCampaignManage: 'Permission_DocManagementCampaign_Manage',
    PermissionNameForDocManagementAdditionalCampaignManage: 'Permission_DocManagementAdditionalCampaign_Manage',
    PermissionNameForDocManagementApplicationRead: 'Permission_DocManagementApplication_Read',
    PermissionNameForDocManagementApplicationManage: 'Permission_DocManagementApplication_Manage',
    PermissionNameForDocManagementReportCreate: 'Permission_DocManagementReport_Create',
};


export const DiplomaSubjectSections = {
  MandatoryPreparation: 2,
  MandatoryChoicePreparation: 3,
  NonProfileMandatoryChoicePreparation: 5,
  FreeChoicePreparation: 6
};

export const FirstGradeIconClasses = {
  2: 'far fa-frown-open',
  3: 'far fa-surprise',
  4: 'far fa-meh',
  5: 'far fa-smile',
  6: 'far fa-grin-beam'
};

export const RecognitionEducationLevel = {
  Primary: 1,
  Secondary: 2,
  ProfessionalQualification: 3,
  Class: 4
};

export const Months = [{ monthName: 'Януари', value: 1 }, { monthName: 'Февруари', value: 2 }, { monthName: 'Март', value: 3 }, { monthName: 'Април', value: 4 }, { monthName: 'Май', value: 5 }, { monthName: 'Юни', value: 6 }, { monthName: 'Юли', value: 7 }, { monthName: 'Август', value: 8 }, { monthName: 'Септември', value: 9 }, { monthName: 'Октомври', value: 10 }, { monthName: 'Ноември', value: 11 }, { monthName: 'Декември', value: 12 }];

export const Positions = {
  Student: 3,
  StudentOtherInstitution: 7,
  StudentPersonalSupport: 8,
  StudentSpecialEducationSupport: 10,
  ProfessionalEducation: 11
};

export const EnrolledStudentsExportFileTypes = {
  EnrolledStudents: 1,
  EnrolledStudentsCorrections: 2
};

export const ClassKind = {
  Basic: 1,
  Cdo: 2,
  Other: 3
};

export const AppSettingKeys = {
  AbsenceImportType: 'AbsenceImportType'
};

export const InstType = {
  School: 1,
  Kindergarten: 2,
  CPLR: 3,
  CSOP: 4,
  SOZ: 5
};

export const NEISPUOStatus =
{
    UnderReview: 0,
    Confirmed: 1,
    Rejected: 2
};

export const ASPStatus =
{
  Absence: 0,
  Discharged: 1,
  NonVisiting: 2
};

export const LodEvaluationType =
{
  Evaluation: 1,
  SelfEduFormEvaluation: 2,
  FirstGradeEvaluation: 3
};

export const DocumentStatusType =
{
  Draft: 1,
  Confirmed: 2
};

export const BasicDocumentPartCategory =
[
  { value: '1', text: 'Mandatory Part' },
  { value: '2', text: 'ZIPProf Part' },
  { value: '3', text: 'ZIPNoProf Part'},
  { value: '4', text: 'SIP Part'},
  { value: '5', text: 'Elective Part'},
  { value: '6', text: 'Optional Part'},
  { value: '7', text: 'MandatoryDZI Part'},
  { value: '8', text: 'AdditionalDZI Part'},
  { value: '9', text: 'Faculty Part'},
  { value: '10', text: 'NVO Part'}
];

export const PersonalDevelopmentSupportPeriodType = {
  ShortTerm: 1,
  LongTerm: 2
};

export const PersonalDevelopmentSupportStudentType = {
  Sop: 1,
  Risk: 2,
  Gifts: 3,
  ChronicDiseases: 4
};

export const AdditipnalPersonalDevelopmentSupportType = {
  StudentSupportPlan: 1,
  PsychosocialRehabilitation: 2,
  HearingAndSpeechRehabilitation: 3,
  VisualRehabilitation: 4,
  RehabilitationOfCommunicationDisorderAndPhysicalDisabilities: 5,
  GeneralAndSpecializedSupportiveEnvironment: 6,
  ProvisionOfSpecialEducationSubjects: 7,
  ResourceSupport: 8
};

export const RegBookType =
Object.freeze({
  RegBookQualification: 1,
  RegBookQualificationDuplicate: 2,
  RegBookCertificate: 3,
  RegBookCertificateDuplicate: 4
});

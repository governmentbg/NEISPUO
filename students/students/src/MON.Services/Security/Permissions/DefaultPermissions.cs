using MON.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MON.Services.Security.Permissions
{
    /// <summary>
    /// Контейнер на правата за достъп до ресурсите. Симулира DB.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DefaultPermissions
    {
        /// <summary>
        /// Връща всички права като списък от <see cref="Permission"/>. 
        /// Използва се за регистрация на всички права като policy-та в Authorization middleware-a. Виж AuthenticationUtil.cs
        /// </summary>
        /// <returns></returns>Permission_DocManagementReport_Create
        public static List<Permission> GetAll()
        {
            var list = new List<Permission>();
            Type t = typeof(DefaultPermissions);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public); // Взима всички права(константи) по-долу.
            foreach (FieldInfo fi in fields)
            {
                var permissionName = fi.GetValue(null) as string;
                if (permissionName.IsNullOrWhiteSpace()) continue;

                list.Add(new Permission
                {
                    Key = fi.Name,
                    Name = permissionName,
                    DisplayName = string.Join(' ', permissionName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries))
                });
            }

            return list;
        }

        // Списък с правата, които ще ползваме при ауторизацията.
        // Да се спазва тази конвенция за именуване и формат на стойностите!!!
        public const string PermissionNameForStudentModuleUse = "Permission_StudentModule_Use";

        public const string PermissionNameForNomenclatureRead = "Permission_Nomenclature_Read";
        public const string PermissionNameForNomenclatureCreate = "Permission_Nomenclature_Create";
        public const string PermissionNameForNomenclatureUpdate = "Permission_Nomenclature_Update";
        public const string PermissionNameForNomenclatureDelete = "Permission_Nomenclature_Delete";
        public const string PermissionNameForNomenclatureManage = "Permission_Nomenclature_Manage";


        #region Student
        public const string PermissionNameForStudentSearch = "Permission_Student_Search";
        public const string PermissionNameForStudentCreate = "Permission_Student_Create";
        public const string PermissionNameForStudentLodDocumentsShow = "Permission_StudentLodDocuments_Show";

        public const string PermissionNameForAdmissionPermissionRequestRead = "Permission_AdmissionPermissionRequest_Read";
        public const string PermissionNameForAdmissionPermissionRequestManage = "Permission_AdmissionPermissionRequest_Manage";

        public const string PermissionNameForAzureAccountManage = "Permission_AzureAccount_Manage";

        #endregion

        #region Меню "ЛОД/Данни за институцията"
        public const string PermissionNameForStudentCurrentInstitutionDetailsRead = "Permission_StudentCurrentInstitutionDetails_Read";
        #endregion

        #region Меню "ЛОД/Общи данни за обучението"
        public const string PermissionNameForStudentGeneralTrainingDataRead = "Permission_StudentGeneralTrainingData_Read";
        #endregion

        #region Меню "ЛОД/Предучилищна подготовка"
        public const string PermissionNameForStudentPreSchoolEvaluationRead = "Permission_StudentPreSchoolEvaluation_Read";
        public const string PermissionNameForStudentPreSchoolEvaluationManage = "Permission_StudentPreSchoolEvaluation_Manage";
        #endregion

        #region Меню "ЛОД/Оценки"
        public const string PermissionNameForStudentEvaluationRead = "Permission_StudentEvaluation_Read";
        public const string PermissionNameForStudentEvaluationManage = "Permission_StudentEvaluation_Manage";
        public const string PermissionNameForStudentEvaluationFinalize = "Permission_StudentEvaluation_Finalize";
        #endregion

        #region Меню "ЛОД/НВО-ДЗИ"
        public const string PermissionNameForStudentExternalEvaluationRead = "Permission_StudentExternalEvaluation_Read";
        public const string PermissionNameForStudentExternalEvaluationManage = "Permission_StudentExternalEvaluation_Manage";
        #endregion

        #region Меню "ЛОД/Награди"
        public const string PermissionNameForStudentAwardRead = "Permission_StudentAward_Read";
        public const string PermissionNameForStudentAwardManage = "Permission_StudentAward_Manage";
        #endregion

        #region Меню "ЛОД/ЛОД/Санкции"
        public const string PermissionNameForStudentSanctionRead = "Permission_StudentSanction_Read";
        public const string PermissionNameForStudentSanctionManage = "Permission_StudentSanction_Manage";
        #endregion

        #region Меню "ЛОД/Подкрепа за личностно развитие"
        public const string PermissionNameForStudentPersonalDevelopmentRead = "Permission_StudentPersonalDevelopment_Read";
        public const string PermissionNameForStudentPersonalDevelopmentManage = "Permission_StudentPersonalDevelopment_Manage";
        #endregion

        #region Меню "ЛОД/СОП"
        public const string PermissionNameForStudentSopRead = "Permission_StudentSop_Read";
        public const string PermissionNameForStudentSopManage = "Permission_StudentSop_Manage";
        #endregion

        #region Меню "ЛОД/Ученическо самоуправление"
        public const string PermissionNameForStudentSelfGovernmentRead = "Permission_StudentSelfGovernment_Read";
        public const string PermissionNameForStudentSelfGovernmentManage = "Permission_StudentSelfGovernment_Manage";
        #endregion

        #region Меню "ЛОД/Международна мобилност"
        public const string PermissionNameForStudentInternationalMobilityRead = "Permission_StudentInternationalMobility_Read";
        public const string PermissionNameForStudentInternationalMobilityManage = "Permission_StudentInternationalMobility_Manage";
        #endregion

        #region Меню "Дипломи"
        public const string PermissionNameForStudentDiplomaRead = "Permission_StudentDiploma_Read";
        public const string PermissionNameForStudentDiplomaManage = "Permission_StudentDiploma_Manage";
        public const string PermissionNameForStudentDiplomaFinalize = "Permission_StudentDiploma_Finalize";
        public const string PermissionNameForStudentDiplomaAnnulment = "Permission_StudentDiploma_Annulment";

        public const string PermissionNameForInstitutionDiplomaRead = "Permission_InstitutionDiploma_Read";
        public const string PermissionNameForInstitutionDiplomaManage = "Permission_InstitutionDiploma_Manage";

        // Достъп до всички дипломи от потребители, които трябва да могат да имат достъп
        public const string PermissionNameForAdminDiplomaRead = "Permission_AdminDiploma_Read";
        public const string PermissionNameForAdminDiplomaManage = "Permission_AdminDiploma_Manage";

        // Достъп до всички дипломи от потребители, които трябва да могат да имат достъп
        public const string PermissionNameForMonHrDiplomaRead = "Permission_MonHrDiploma_Read";
        public const string PermissionNameForMonHrDiplomaManage = "Permission_MonHrDiploma_Manage";


        // Достъп до всички дипломи от потребители, които трябва да могат да имат достъп
        public const string PermissionNameForRuoHrDiplomaRead = "Permission_RouHrDiploma_Read";
        public const string PermissionNameForRuoHrDiplomaManage = "Permission_RouHrDiploma_Manage";

        public const string PermissionNameForStudentDiplomaImportValidationExclusionsManage = "Permission_StudentDiplomaImportValidationExclusions_Manage";

        public const string PermissionNameForDiplomaCreateRequestRead = "Permission_DiplomaCreateRequest_Read";
        public const string PermissionNameForDiplomaCreateRequestManage = "Permission_DiplomaCreateRequest_Manage";

        // Права за четене/редакция на дипломи, получени чрез създаването на заявление за създаване на документ/диплома.
        // Различават се от PermissionNameForStudentDiplomaRead и PermissionNameForStudentDiplomaManage по това,
        // че трябва да скриваме дипломите на ученика от други институции.
        public const string PermissionNameForStudentDiplomaByCreateRequestRead = "Permission_StudentDiplomaByCreateRequest_Read";
        public const string PermissionNameForStudentDiplomaByCreateRequestManage = "Permission_StudentDiplomaByCreateRequest_Manage";
        #endregion

        #region Меню "Признаване"
        public const string PermissionNameForStudentRecognitionRead = "Permission_StudentRecognition_Read";
        public const string PermissionNameForStudentRecognitionManage = "Permission_StudentRecognition_Manage";
        #endregion

        #region Меню "Приравняване"
        public const string PermissionNameForStudentEqualizationRead = "Permission_StudentEqualization_Read";
        public const string PermissionNameForStudentEqualizationManage = "Permission_StudentEqualization_Manage";
        #endregion

        #region Меню "Изпити за промяна на оценка"
        public const string PermissionNameForStudentReassessmentRead = "Permission_StudentReassessment_Read";
        public const string PermissionNameForStudentReassessmentManage = "Permission_StudentReassessment_Manage";
        #endregion

        #region Меню "Характеристика на средата"
        public const string PermissionNameForStudentEnvironmentCharacteristicRead = "Permission_StudentEnvironmentCharacteristic_Read";
        public const string PermissionNameForStudentEnvironmentCharacteristicManage = "Permission_StudentEnvironmentCharacteristic_Manage";
        #endregion

        #region Меню "Ресурсно подпомагане"
        public const string PermissionNameForStudentResourceSupportRead = "Permission_StudentResourceSupport_Read";
        public const string PermissionNameForStudentResourceSupportManage = "Permission_StudentResourceSupport_Manage";
        #endregion

        #region Меню "Стипендии"
        public const string PermissionNameForStudentScholarshipRead = "Permission_StudentScholarship_Read";
        public const string PermissionNameForStudentScholarshipManage = "Permission_StudentScholarship_Manage";
        #endregion

        #region Меню "Други документи"
        public const string PermissionNameForStudentOtherDocumentRead = "Permission_StudentOtherDocument_Read";
        public const string PermissionNameForStudentOtherDocumentManage = "Permission_StudentOtherDocument_Manage";
        #endregion

        #region Меню "Бележки"
        public const string PermissionNameForStudentNoteRead = "Permission_StudentNote_Read";
        public const string PermissionNameForStudentNoteManage = "Permission_StudentNote_Manage";
        #endregion

        #region Меню "Други институции"
        public const string PermissionNameForStudentOtherInstitutionRead = "Permission_StudentOtherInstitution_Read";
        public const string PermissionNameForStudentOtherInstitutionManage = "Permission_StudentOtherInstitution_Manage";
        #endregion

        #region StudentClass. Бутон класове в картата с профилните данни на ученик
        public const string PermissionNameForStudentClassRead = "Permission_StudentClasses_Read";
        public const string PermissionNameForStudentClassUpdate = "Permission_StudentClass_Update";
        public const string PermissionNameForStudentClassHistoryRead = "Permission_StudentClassHistory_Read";
        public const string PermissionNameForStudentClassHistoryDelete = "Permission_StudentClassHistory_Delete";
        /// <summary>
        /// Масово записване и отписване на ученици от паралелка в ЦПЛР
        /// </summary>
        public const string PermissionNameForStudentClassMassEnrolmentManage = "Permission_StudentClass_MassEnrollment_Manage";
        #endregion

        #region Лични данни за детето / ученика
        public const string PermissionNameForStudentPersonalDataRead = "Permission_StudentPersonalData_Read";
        public const string PermissionNameForStudentPartialPersonalDataRead = "Permission_StudentPartialPersonalData_Read";
        public const string PermissionNameForStudentPersonalDataManage = "Permission_StudentPersonalData_Manage";
        public const string PermissionNameForStudentEducationRead = "Permission_StudentEducation_Read";
        public const string PermissionNameForStudentInternationalProtectionRead = "Permission_StudentInternationalProtection_Read";
        public const string PermissionNameForStudentInternationalProtectionManage = "Permission_StudentInternationalProtection_Manage";
        #endregion

        #region Меню "Движение на ученика" => "Документи за записване" 
        public const string PermissionNameForStudentAdmissionDocumentRead = "Permission_StudentAdmissionDocument_Read";
        public const string PermissionNameForStudentAdmissionDocumentCreate = "Permission_StudentAdmissionDocument_Create";
        public const string PermissionNameForStudentAdmissionDocumentUpdate = "Permission_StudentAdmissionDocument_Update";
        public const string PermissionNameForStudentAdmissionDocumentDelete = "Permission_StudentAdmissionDocument_Delete";
        public const string PermissionNameForStudentToClassEnrollment = "Permission_StudentToClass_Enrollment";
        #endregion

        #region Бутон(в timeline-а) за създаване/преглед на учебен план
        public const string PermissionNameForStudentCurriculumRead = "Permission_StudentCurriculum_Read";
        public const string PermissionNameForStudentCurriculumManage = "Permission_StudentCurriculum_Manage";
        #endregion

        #region Меню "Движение на ученика" => "Документи за отписване"
        public const string PermissionNameForStudentDischargeDocumentRead = "Permission_StudentDischargeDocument_Read";
        public const string PermissionNameForStudentDischargeDocumentCreate = "Permission_StudentDischargeDocument_Create";
        public const string PermissionNameForStudentDischargeDocumentUpdate = "Permission_StudentDischargeDocument_Update";
        public const string PermissionNameForStudentDischargeDocumentDelete = "Permission_StudentDischargeDocument_Delete";
        #endregion

        #region Меню "Движение на ученика" => "Документи за преместване"
        public const string PermissionNameForStudentRelocationDocumentRead = "Permission_StudentRelocationDocument_Read";
        public const string PermissionNameForStudentRelocationDocumentCreate = "Permission_StudentRelocationDocument_Create";
        public const string PermissionNameForStudentRelocationDocumentUpdate = "Permission_StudentRelocationDocument_Update";
        public const string PermissionNameForStudentRelocationDocumentDelete = "Permission_StudentRelocationDocument_Delete";
        #endregion

        #region Подписване и прикачване на ЛОД в документ за преместване/ освобождаване
        public const string PermissionNameForStudentRelocationDocumentSignLOD = "Permission_StudentRelocationDocument_Sign_LOD";
        public const string PermissionNameForStudentDischargeDocumentSignLOD = "Permission_StudentDischargeDocument_Sign_LOD";
        #endregion

        #region Меню "Списъци"(Образователни институции) => Детайли, Списък с ученици
        public const string PermissionNameForInstitutionRead = "Permission_Institution_Read";
        public const string PermissionNameForInstitutionClassesRead = "Permission_InstitutionClasses_Read";
        public const string PermissionNameForInstitutionStudentsRead = "Permission_InstitutionStudents_Read";
        #endregion

        #region Меню "Списъци"(Списък паралялки/групи) => Отсъствия, Автоматично номериране, Печат
        public const string PermissionNameForClassManage = "Permission_Class_Manage";
        public const string PermissionNameForClassStudentsRead = "Permission_ClassStudents_Read";
        public const string PermissionNameForClassAbsenceRead = "Permission_ClassAbsence_Read";
        public const string PermissionNameForClassAbsenceManage = "Permission_ClassAbsence_Manage";
        #endregion

        #region Меню "Приключване на ЛОД"
        public const string PermissionNameForStudentLodFinalizationRead = "Permission_StudentLodFinalization_Read";
        public const string PermissionNameForLodFinalizationRead = "Permission_LodFinalization_Read";
        public const string PermissionNameForLodFinalizationAdministration = "Permission_LodFinalization_Administration";
        #endregion


        #region Administration
        public const string PermissionNameForSettingsSectionShow = "Permission_SettingsSection_Show";
        public const string PermissionNameForDiplomasShow = "Permission_DiplomasSection_Show";
        public const string PermissionNameForAuditLogsShow = "Permission_AuditLogs_Show";
        public const string PermissionNameForDashboardShow = "Permission_Dashboard_Show";
        public const string PermissionNameForDocumentationShow = "Permission_Documentation_Show";
        public const string PermissionNameForTemplatesSectionShow = "Permission_TemplatesSection_Show";
        public const string PermissionNameForCacheServiceManage = "Permission_CacheService_Manage";
        
        public const string PermissionNameForDiplomaBarcodesShow = "Permission_DiplomaBarcodes_Show";
        public const string PermissionNameForDiplomaBarcodesAdd = "Permission_DiplomaBarcodes_Add";
        public const string PermissionNameForDiplomaBarcodesEdit = "Permission_DiplomaBarcodes_Edit";
        public const string PermissionNameForPrintTemplatesShow = "Permission_PrintTemplates_Show";
        public const string PermissionNameForLodAccessConfigurationShow = "Permission_LodAccessConfiguration_Show";
        public const string PermissionNameForBasicDocumentsListShow = "Permission_BasicDocuments_Show";
        public const string PermissionNameForBasicDocumentEdit = "Permission_BasicDocument_Edit";
        public const string PermissionNameForLodAccessConfigurationEdit = "Permission_LodAccessConfiguration_Edit";
        public const string PermissionNameForBasicDocumentTeplatesShow = "Permission_BasicDocumentTeplates_Show";
        public const string PermissionNameForSopEnrollmentDetailsRead = "Permission_SopEnrollmentDetails_Read";
        public const string PermissionNameForContextualInformationManage = "Permission_ContextualInformation_Manage";

        // Определя правото за управление на настройките на приложението за дадена институция
        public const string PermissionNameForTenantAppSettingsManage = "Permission_TenantAppSettings_Manage";

        #endregion

        public const string PermissionNameForAdministrationSectionShow = "Permission_AdministrationSection_Show";
        public const string PermissionNameForUnsignedStudentLodRead = "Permission_UnsignedStudentLod_Read";

        #region Шаблони и номерация на документи
        public const string PermissionNameForDiplomaTemplatesRead = "Permission_DiplomaTemplates_Read";
        public const string PermissionNameForDiplomaTemplatesManage = "Permission_DiplomaTemplates_Manage";

        public const string PermissionNameForBasicDocumentSequenceRead = "Permission_BasicDocumentSequence_Read";
        public const string PermissionNameForBasicDocumentSequenceManage = "Permission_BasicDocumentSequence_Manage";
        #endregion

        #region Класни ръководители
        public const string PermissionNameForLeadTeacherManage = "Permission_LeadTeacher_Manage";
        #endregion

        #region Development
        public const string PermissionNameForDemonstrationSectionShow = "Permission_DemonstrationSection_Show";
        #endregion

        #region Меню Отсъствия
        public const string PermissionNameForStudentAbsenceRead = "Permission_StudentAbsence_Read";
        public const string PermissionNameForStudentAbsenceManage = "Permission_StudentAbsence_Manage";
        /// <summary>
        /// Четене, листване на кампании за импорт на отсъствия
        /// </summary>
        public const string PermissionNameForStudentAbsenceCampaignRead = "Permission_StudentAbsenceCampaign_Read";
        /// <summary>
        /// Управление(CRUD) на кампании за импорт на отсъствия
        public const string PermissionNameForStudentAbsenceCampaignManage = "Permission_StudentAbsenceCampaign_Manage";
        public const string PermissionNameForAbsencesExportRead = "Permission_AbsencesExport_Read";
        public const string PermissionNameForAbsencesExportManage = "Permission_AbsencesExport_Manage";
        #endregion

        #region Търсещи закрила
        public const string PermissionNameForRefugeeApplicationsRead = "Permission_RefugeeApplications_Read";
        public const string PermissionNameForRefugeeApplicationsManage = "Permission_RefugeeApplications_Manage";
        public const string PermissionNameForRefugeeApplicationsUnlock = "Permission_RefugeeApplications_Unlock";
        #endregion

        #region Меню ASP
        /// <summary>
        /// Право за:
        /// - зареждане на данни за потвърждаване от АСП
        /// - показване на грид с незаредени данни от АСП
        /// - редакция на времената на кампанията за потвърждаване
        /// </summary>
        public const string PermissionNameForASPImport = "Permission_ASP_Import";
        public const string PermissionNameForASPManage = "Permission_ASP_Manage";
        public const string PermissionNameForASPBenefitDetailsRead = "Permission_ASP_BenefitDetails_Read";
        public const string PermissionNameForASPAdministrationManage = "Permission_ASP_Administration_Manage";
        public const string PermissionNameForASPImportDetailsShow = "Permission_ASP_ImportDetailsShow";
        public const string PermissionNameForASPBenefitUpdate = "Permission_ASP_BenefitUpdate";
        public const string PermissionNameForASPEnrolledStudentsExport = "Permission_ASP_EnrolledStudentsExport";
        public const string PermissionNameForASPGlobalAdmin = "Permission_ASP_GlobalAdmin";


        #endregion

        #region Меню здравно осигуряване
        public const string PermissionNameForHealthInsuranceManage = "Permission_HealthInsurance_Manage";
        #endregion

        #region Меню Финанси
        // Натурални показатели
        public const string PermissionNameForNaturalIndicatorsManage = "Permission_NaturalIndicators_Manage";
        public const string PermissionNameForResourceSupportDataManage = "Permission_ResourceSupportData_Manage";
        #endregion

        #region Бутони за одобрение и финализиране на ЛОД
        public const string PermissionNameForLodStateManage = "PermissionName_LodState_Manage";
        #endregion

        #region Меню Сертификати
        public const string PermissionNameForCertificatesManage = "Permission_Certificates_Manage";
        public const string PermissionNameForCertificatesRead = "Permission_Certificates_Read";
        #endregion

        #region "Бутон Информация за свързани записи"
        public const string PermissionNameDataReferencesRead = "Permission_DataReferences_Read";
        public const string PermissionNameDataReferencesReadManage = "Permission_DataReferences_Manage";
        #endregion

        #region Меню ОРЕС
        public const string PermissionNameForOresRead = "Permission_Ores_Read";
        public const string PermissionNameForOresManage = "Permission_Ores_Manage";
        #endregion

        #region Меню Импорт на оценки
        public const string PermissionNameForLodAssessmentImport = "Permission_LodAssessment_Import";
        #endregion


        #region Меню Шаблони за СФО
        public const string PermissionNameForLodAssessmentTemplateRead = "Permission_LodAssessmentTemplate_Read";
        public const string PermissionNameForLodAssessmentTemplateManage = "Permission_LodAssessmentTemplate_Manage";
        #endregion

        #region Управление на документи с фабрична номерация

        /// <summary>
        /// Четене, листване на кампании за заявяване на документи с фабрична номерация
        /// </summary>
        public const string PermissionNameForDocManagementCampaignRead = "Permission_DocManagementCampaign_Read";
        /// <summary>
        /// Управление(CRUD) на кампании за заявяване на документи с фабрична номерация
        /// </summary>
        public const string PermissionNameForDocManagementCampaignManage = "Permission_DocManagementCampaign_Manage";

        /// <summary>
        /// Управление(CRUD) на кампании за заявяване на документи с фабрична номерация
        /// </summary>
        public const string PermissionNameForDocManagementAdditionalCampaignManage = "Permission_DocManagementAdditionalCampaign_Manage";


        /// <summary>
        /// Четене, листване на заявки на документи с фабрична номерация
        /// </summary>
        public const string PermissionNameForDocManagementApplicationRead = "Permission_DocManagementApplication_Read";
        /// <summary>
        /// Управление(CRUD) на заявки на документи с фабрична номерация
        /// </summary>
        public const string PermissionNameForDocManagementApplicationManage = "Permission_DocManagementApplication_Manage";

        public const string PermissionNameForDocManagementReportCreate = "Permission_DocManagementReport_Create";

        #endregion

        /// <summary>
        /// Списък с ученици в обучение чрез работа (дуална система на обучение) и информация за месторабота.
        /// Възможност за въвеждане на ЕИК на работодател при записване в паралелка #1258.
        /// Списъкът ще е достъпен и от таблата на потребители с роля РУО, РУО експерт, МОН, МОН експерт, ЦИОО и Консорциум.
        /// </summary>
        public const string PermissionNameForDualFormRead = "Permission_DualForm_Read";

       
    }
}

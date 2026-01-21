using Microsoft.AspNetCore.Identity;
using MON.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Security.Permissions
{
    /// <summary>
    /// Описва коя роля, кои права има.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class RolesPermissions
    {
        // <roleName, permissions> Права за дадена роля.
        private static Dictionary<int, HashSet<string>> _rolesPermissions;

        private RolesPermissions()
        {

            _rolesPermissions = new Dictionary<int, HashSet<string>>
                    {
                        { (int)UserRoleEnum.Mon, new HashSet<string>(new List<string> {
                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                // меню Отсъствия
                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage,

                                // меню АСП
                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPAdministrationManage,
                                DefaultPermissions.PermissionNameForASPEnrolledStudentsExport,

                                //меню Шаблони и документи
                                DefaultPermissions.PermissionNameForTemplatesSectionShow,

                                //подменю Документи
                                DefaultPermissions.PermissionNameForBasicDocumentsListShow,
                                DefaultPermissions.PermissionNameForBasicDocumentEdit,

                                //баркодове на документи
                                DefaultPermissions.PermissionNameForDiplomaBarcodesShow,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesAdd,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesEdit,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,

                                //меню Приключване на ЛОД
                                DefaultPermissions.PermissionNameForLodFinalizationRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                //списък на Обучаващи се в ЦСОП
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък с лица под закрила, насочени от РУО за записване
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                //списък с отписани деца без подписано ЛОД
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,

                                // Финанси - натурални показатели
                                DefaultPermissions.PermissionNameForNaturalIndicatorsManage,
                                DefaultPermissions.PermissionNameForResourceSupportDataManage,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,

                                DefaultPermissions.PermissionNameForDualFormRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.MonExpert, new HashSet<string>(new List<string> {    

                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                //списък на Обучаващи се в ЦСОП
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък с лица под закрила, насочени от РУО за записване
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                //списък с отписани деца без подписано ЛОД
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,

                                DefaultPermissions.PermissionNameForDualFormRead,

                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.ExternalExpert, new HashSet<string>(new List<string> {
                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow
                            })
                        },

                        { (int)UserRoleEnum.Consortium, new HashSet<string>(new List<string> {
                                DefaultPermissions.PermissionNameForNomenclatureRead,
                                DefaultPermissions.PermissionNameForNomenclatureCreate,
                                DefaultPermissions.PermissionNameForNomenclatureUpdate,
                                DefaultPermissions.PermissionNameForNomenclatureDelete,
                                DefaultPermissions.PermissionNameForNomenclatureManage,

                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,

                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsManage,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsUnlock,

                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceManage,
                                DefaultPermissions.PermissionNameForAbsencesExportRead,
                                DefaultPermissions.PermissionNameForAbsencesExportManage,
                                DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage,

                                DefaultPermissions.PermissionNameForASPImport,
                                DefaultPermissions.PermissionNameForASPBenefitDetailsRead,
                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPBenefitUpdate,
                                DefaultPermissions.PermissionNameForASPGlobalAdmin,

                                DefaultPermissions.PermissionNameForSettingsSectionShow,
                                DefaultPermissions.PermissionNameForContextualInformationManage,

                                DefaultPermissions.PermissionNameForLodAccessConfigurationShow,
                                DefaultPermissions.PermissionNameForLodAccessConfigurationEdit,

                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                                DefaultPermissions.PermissionNameForBasicDocumentsListShow,
                                DefaultPermissions.PermissionNameForBasicDocumentEdit,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesShow,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesAdd,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesEdit,

                                DefaultPermissions.PermissionNameForLodAssessmentTemplateRead,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateManage,

                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForAdminDiplomaManage,
                                DefaultPermissions.PermissionNameForAdminDiplomaRead,

                                DefaultPermissions.PermissionNameForOresRead,
                                DefaultPermissions.PermissionNameForCertificatesManage,
                                DefaultPermissions.PermissionNameForCertificatesRead,

                                DefaultPermissions.PermissionNameForLodFinalizationRead,
                                DefaultPermissions.PermissionNameForLodFinalizationAdministration,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForAdministrationSectionShow,
                                DefaultPermissions.PermissionNameForAuditLogsShow,
                                DefaultPermissions.PermissionNameForStudentDiplomaImportValidationExclusionsManage,
                                DefaultPermissions.PermissionNameForCacheServiceManage,

                                DefaultPermissions.PermissionNameForDemonstrationSectionShow,

                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,

                                DefaultPermissions.PermissionNameForTenantAppSettingsManage,

                                DefaultPermissions.PermissionNameDataReferencesRead,
                                DefaultPermissions.PermissionNameDataReferencesReadManage,

                                DefaultPermissions.PermissionNameForDualFormRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,

                                DefaultPermissions.PermissionNameForStudentExternalEvaluationManage,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationManage,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },

                        { (int)UserRoleEnum.School, new HashSet<string>(new List<string>
                            {
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                DefaultPermissions.PermissionNameForInstitutionRead,

                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceManage,

                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPBenefitUpdate,

                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesManage,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceManage,
                                DefaultPermissions.PermissionNameForBasicDocumentTeplatesShow,
                                DefaultPermissions.PermissionNameForPrintTemplatesShow,

                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaRead,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaManage,
                                DefaultPermissions.PermissionNameForStudentDiplomaRead,
                                DefaultPermissions.PermissionNameForStudentDiplomaManage,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,

                                DefaultPermissions.PermissionNameForOresRead,
                                DefaultPermissions.PermissionNameForOresManage,

                                DefaultPermissions.PermissionNameForLodAssessmentImport,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForNomenclatureManage,

                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,

                                DefaultPermissions.PermissionNameForTenantAppSettingsManage,
                                DefaultPermissions.PermissionNameForLeadTeacherManage,
                                // не се използва
                                DefaultPermissions.PermissionNameForDiplomaCreateRequestRead,
                                DefaultPermissions.PermissionNameForDiplomaCreateRequestManage,

                                // Роля училище да може да управлява Други документи, независимо дали ученикът е записан в институцията
                                DefaultPermissions.PermissionNameForStudentOtherDocumentManage,

                                DefaultPermissions.PermissionNameForDualFormRead,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationManage,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.SchoolDirector, new HashSet<string>(new List<string>
                            {
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                DefaultPermissions.PermissionNameForInstitutionRead,

                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceManage,

                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPBenefitUpdate,

                                DefaultPermissions.PermissionNameForHealthInsuranceManage,

                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesManage,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceManage,
                                DefaultPermissions.PermissionNameForBasicDocumentTeplatesShow,
                                DefaultPermissions.PermissionNameForPrintTemplatesShow,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateRead,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateManage,

                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaRead,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaManage,
                                DefaultPermissions.PermissionNameForStudentDiplomaRead,
                                DefaultPermissions.PermissionNameForStudentDiplomaManage,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForLodFinalizationRead,

                                DefaultPermissions.PermissionNameForOresRead,
                                DefaultPermissions.PermissionNameForOresManage,

                                DefaultPermissions.PermissionNameForNaturalIndicatorsManage,
                                DefaultPermissions.PermissionNameForResourceSupportDataManage,

                                DefaultPermissions.PermissionNameForLodAssessmentImport,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForNomenclatureManage,

                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,

                                DefaultPermissions.PermissionNameForTenantAppSettingsManage,
                                DefaultPermissions.PermissionNameForLeadTeacherManage,
                                // не се използва
                                DefaultPermissions.PermissionNameForDiplomaCreateRequestRead,
                                DefaultPermissions.PermissionNameForDiplomaCreateRequestManage,

                                // Роля училище да може да управялева Други документи, независимо дали ученикът е записан в институцията
                                DefaultPermissions.PermissionNameForStudentOtherDocumentManage,

                                DefaultPermissions.PermissionNameForDualFormRead,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationManage,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.KindergartenDirector, new HashSet<string>(new List<string>
                            {
                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                
                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,
                                
                                // меню Отсъствия
                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceManage,

                                // меню АСП
                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPBenefitUpdate,

                                // меню Шаблони и документи
                                // подменю Шаблони на документи
                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesManage,
                                // подменю Настройка на печат на документи
                                DefaultPermissions.PermissionNameForPrintTemplatesShow,
                                // подменю Рег. номера на документи
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceManage,

                                // меню Дипломи и меню Регистрационни книги
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaRead,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaManage,

                                // меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,
                                DefaultPermissions.PermissionNameForOresManage,

                                // Натурални показатели
                                DefaultPermissions.PermissionNameForNaturalIndicatorsManage,
                                DefaultPermissions.PermissionNameForResourceSupportDataManage,

                                // меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,


                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForLeadTeacherManage,
                                                                
                                // Директорско табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                // списък Обучаващи се в ЦСОП   
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък Насочени от РУО за записване получили закрила
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,

                                // Настройки за начин на подаване на отсъствия
                                DefaultPermissions.PermissionNameForTenantAppSettingsManage,

                                // Роля детска градина да може да управлява Други документи, независимо дали ученикът е записан в институцията
                                DefaultPermissions.PermissionNameForStudentOtherDocumentManage,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementApplicationManage,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.CPLRDirector, new HashSet<string>(new List<string>
                            {
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForDocumentationShow,
                                DefaultPermissions.PermissionNameForInstitutionRead,
                            })
                        },
                        { (int)UserRoleEnum.CSOPDirector, new HashSet<string>(new List<string>
                            {
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                DefaultPermissions.PermissionNameForInstitutionRead,

                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceManage,

                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPBenefitUpdate,

                                DefaultPermissions.PermissionNameForHealthInsuranceManage,

                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateRead,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateManage,

                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForInstitutionDiplomaRead,
                                DefaultPermissions.PermissionNameForStudentDiplomaRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForLodFinalizationRead,

                                DefaultPermissions.PermissionNameForOresRead,
                                DefaultPermissions.PermissionNameForOresManage,

                                DefaultPermissions.PermissionNameForLodAssessmentImport,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForNomenclatureManage,

                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,

                                DefaultPermissions.PermissionNameForTenantAppSettingsManage,
                                DefaultPermissions.PermissionNameForLeadTeacherManage,
                            })
                        },
                        { (int)UserRoleEnum.SOZDirector, new HashSet<string>(new List<string>
                            {
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForDocumentationShow,
                                DefaultPermissions.PermissionNameForInstitutionRead,

                            })
                        },

                        { (int)UserRoleEnum.Teacher, new HashSet<string>(new List<string>
                            {
                                // Това е временно и трябва да се промени!
                                // По-долу на учител са дадени същите права, като на директор
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForDocumentationShow,
                                DefaultPermissions.PermissionNameForInstitutionRead,
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,

                                // Използва се за зареждане на данните от шаблон на документ при създаване на диплома    
                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,

                                // Използва се за управление на шаблоните с оценки за СФО
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateRead,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateManage,

                                DefaultPermissions.PermissionNameForDualFormRead,
                            })
                        },
                        { (int)UserRoleEnum.CIOO, new HashSet<string>(new List<string>
                            {
                                DefaultPermissions.PermissionNameForNomenclatureRead,
                                DefaultPermissions.PermissionNameForNomenclatureCreate,
                                DefaultPermissions.PermissionNameForNomenclatureUpdate,
                                DefaultPermissions.PermissionNameForNomenclatureDelete,
                                DefaultPermissions.PermissionNameForNomenclatureManage,

                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,

                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsManage,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsUnlock,

                                DefaultPermissions.PermissionNameForStudentAbsenceRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceManage,
                                DefaultPermissions.PermissionNameForAbsencesExportRead,
                                DefaultPermissions.PermissionNameForAbsencesExportManage,
                                DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead,
                                DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage,

                                DefaultPermissions.PermissionNameForASPImport,
                                DefaultPermissions.PermissionNameForASPBenefitDetailsRead,
                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPBenefitUpdate,
                                DefaultPermissions.PermissionNameForASPGlobalAdmin,

                                DefaultPermissions.PermissionNameForSettingsSectionShow,
                                DefaultPermissions.PermissionNameForContextualInformationManage,

                                DefaultPermissions.PermissionNameForLodAccessConfigurationShow,
                                DefaultPermissions.PermissionNameForLodAccessConfigurationEdit,

                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                                DefaultPermissions.PermissionNameForBasicDocumentsListShow,
                                DefaultPermissions.PermissionNameForBasicDocumentEdit,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesShow,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesAdd,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesEdit,

                                DefaultPermissions.PermissionNameForLodAssessmentTemplateRead,
                                DefaultPermissions.PermissionNameForLodAssessmentTemplateManage,

                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForAdminDiplomaManage,
                                DefaultPermissions.PermissionNameForAdminDiplomaRead,

                                DefaultPermissions.PermissionNameForOresRead,
                                DefaultPermissions.PermissionNameForCertificatesManage,
                                DefaultPermissions.PermissionNameForCertificatesRead,

                                DefaultPermissions.PermissionNameForLodFinalizationRead,
                                DefaultPermissions.PermissionNameForLodFinalizationAdministration,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForDashboardShow,
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,

                                DefaultPermissions.PermissionNameForTenantAppSettingsManage,

                                DefaultPermissions.PermissionNameDataReferencesRead,
                                DefaultPermissions.PermissionNameDataReferencesReadManage,

                                DefaultPermissions.PermissionNameForDualFormRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.Ruo, new HashSet<string>(new List<string>
                            {
                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                // меню Отсъствия
                                DefaultPermissions.PermissionNameForStudentAbsenceRead,

                                // меню АСП
                                DefaultPermissions.PermissionNameForASPManage,
                                DefaultPermissions.PermissionNameForASPBenefitDetailsRead,
                                DefaultPermissions.PermissionNameForASPImportDetailsShow,
                                DefaultPermissions.PermissionNameForASPAdministrationManage,

                                //меню Шаблони и документи
                                DefaultPermissions.PermissionNameForTemplatesSectionShow,

                                //подменю Документи
                                DefaultPermissions.PermissionNameForBasicDocumentsListShow,

                                //баркодове на документи
                                DefaultPermissions.PermissionNameForDiplomaBarcodesShow,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesAdd,
                                DefaultPermissions.PermissionNameForDiplomaBarcodesEdit,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,

                                //меню Приключване на ЛОД
                                DefaultPermissions.PermissionNameForLodFinalizationRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                //списък на Обучаващи се в ЦСОП
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък с лица под закрила, насочени от РУО за записване
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                //списък с отписани деца без подписано ЛОД
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,

                                // Финанси - натурални показатели
                                DefaultPermissions.PermissionNameForNaturalIndicatorsManage,
                                DefaultPermissions.PermissionNameForResourceSupportDataManage,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForLodFinalizationRead,

                                DefaultPermissions.PermissionNameForOresRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsManage,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsUnlock,

                                DefaultPermissions.PermissionNameForDualFormRead,

                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceManage,

                                DefaultPermissions.PermissionNameForRuoHrDiplomaRead,
                                DefaultPermissions.PermissionNameForRuoHrDiplomaManage,

                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesManage,

                                DefaultPermissions.PermissionNameForPrintTemplatesShow,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,
                            })
                        },
                        { (int)UserRoleEnum.RuoExpert, new HashSet<string>(new List<string>
                            {
                                //меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                //меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                //списък на Обучаващи се в ЦСОП
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък с лица под закрила, насочени от РУО за записване и работа със заявления
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsManage,
                                DefaultPermissions.PermissionNameForRefugeeApplicationsUnlock,

                                //списък с отписани деца без подписано ЛОД
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,
                                // Финанси - натурални показатели
                                DefaultPermissions.PermissionNameForNaturalIndicatorsManage,
                                DefaultPermissions.PermissionNameForResourceSupportDataManage,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,

                                DefaultPermissions.PermissionNameForOresRead,

                                DefaultPermissions.PermissionNameForDocumentationShow,

                                DefaultPermissions.PermissionNameForLodFinalizationRead,

                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,

                                DefaultPermissions.PermissionNameForDualFormRead,

                                DefaultPermissions.PermissionNameForBasicDocumentSequenceRead,
                                DefaultPermissions.PermissionNameForBasicDocumentSequenceManage,

                                DefaultPermissions.PermissionNameForRuoHrDiplomaRead,
                                DefaultPermissions.PermissionNameForRuoHrDiplomaManage,

                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesManage,

                                DefaultPermissions.PermissionNameForPrintTemplatesShow,

                                // меню Документи с фабрична номерация
                                DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                                DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage,
                                DefaultPermissions.PermissionNameForDocManagementApplicationRead,
                                DefaultPermissions.PermissionNameForDocManagementReportCreate,

                                //меню Шаблони и документи
                                DefaultPermissions.PermissionNameForTemplatesSectionShow,
                            })
                        },
                        { (int)UserRoleEnum.MonOBGUM, new HashSet<string>(new List<string> {
                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                //списък на Обучаващи се в ЦСОП
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък с лица под закрила, насочени от РУО за записване
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                //списък с отписани деца без подписано ЛОД
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow
                            })
                        },
                        { (int)UserRoleEnum.MonOBGUM_Finance, new HashSet<string>(new List<string> {
                                                                // меню Деца и ученици
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentCreate,

                                // меню Списъци
                                DefaultPermissions.PermissionNameForInstitutionRead,

                                //меню Дипломи
                                DefaultPermissions.PermissionNameForDiplomasShow,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,

                                //меню ОРЕС
                                DefaultPermissions.PermissionNameForOresRead,

                                //меню Помощ
                                DefaultPermissions.PermissionNameForDocumentationShow,

                                //табло
                                DefaultPermissions.PermissionNameForDashboardShow,
                                //списък на Обучаващи се в ЦСОП
                                DefaultPermissions.PermissionNameForSopEnrollmentDetailsRead,
                                //списък с лица под закрила, насочени от РУО за записване
                                DefaultPermissions.PermissionNameForRefugeeApplicationsRead,
                                //списък с отписани деца без подписано ЛОД
                                DefaultPermissions.PermissionNameForUnsignedStudentLodRead,

                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow
                            })
                        },
                        { (int)UserRoleEnum.MonHR, new HashSet<string>(new List<string> {
                                DefaultPermissions.PermissionNameForNomenclatureRead,
                                DefaultPermissions.PermissionNameForStudentSearch,
                                DefaultPermissions.PermissionNameForStudentModuleUse,
                                DefaultPermissions.PermissionNameForStudentLodDocumentsShow,
                                DefaultPermissions.PermissionNameForStudentCreate,
                                DefaultPermissions.PermissionNameForDocumentationShow,
                                DefaultPermissions.PermissionNameForBasicDocumentsListShow,
                                DefaultPermissions.PermissionNameForStudentDiplomaRead,
                                DefaultPermissions.PermissionNameForStudentDiplomaManage,
                                DefaultPermissions.PermissionNameForDiplomaTemplatesRead,
                                DefaultPermissions.PermissionNameForDiplomaCreateRequestRead,
                                DefaultPermissions.PermissionNameForMonHrDiplomaRead,
                                DefaultPermissions.PermissionNameForMonHrDiplomaManage,

                            })
                        },
                };
        }

        private static readonly Lazy<RolesPermissions> Instancelock =
                    new Lazy<RolesPermissions>(() => new RolesPermissions());
        public static RolesPermissions GetInstance
        {
            get
            {
                return Instancelock.Value;
            }
        }

        public Dictionary<int, HashSet<string>> All => _rolesPermissions;

        public static HashSet<string> GetRolePermissions(int userRole, int? institutionTypeId = null)
        {
            var selectedRole = userRole;
            if (userRole == (int)UserRoleEnum.School)
            {
                selectedRole = institutionTypeId switch
                {
                    (int)InstitutionTypeEnum.School => (int)UserRoleEnum.SchoolDirector,
                    (int)InstitutionTypeEnum.KinderGarden => (int)UserRoleEnum.KindergartenDirector,
                    (int)InstitutionTypeEnum.PersonalDevelopmentSupportCenter => (int)UserRoleEnum.CPLRDirector,
                    (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport => (int)UserRoleEnum.CSOPDirector,
                    (int)InstitutionTypeEnum.SpecializedServiceUnit => (int)UserRoleEnum.SOZDirector,
                    _ => selectedRole
                };
            }

            if (GetInstance.All.TryGetValue(selectedRole, out HashSet<string> permissonsHash))
            {
                return permissonsHash;
            }
            else
            {
                return null;
            }
        }
    }
}

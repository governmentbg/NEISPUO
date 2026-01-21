namespace MON.Services.Security.Permissions
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public sealed class StudentPermissionLevelGroups
    {
        // <gropName, permissions> Права за дадена група.
        private static Dictionary<int, HashSet<string>> _groupsPermissions;

        private StudentPermissionLevelGroups()
        {
            _groupsPermissions = new Dictionary<int, HashSet<string>>
            {
                {
                    (int)PermissionGroupEnum.Reader,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentEducationRead,
                        DefaultPermissions.PermissionNameForStudentInternationalProtectionRead,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentRead,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentRead,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForStudentOtherDocumentRead,
                        DefaultPermissions.PermissionNameForStudentScholarshipRead,
                        DefaultPermissions.PermissionNameForStudentResourceSupportRead,
                        DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead,
                        DefaultPermissions.PermissionNameForStudentAbsenceRead,
                        DefaultPermissions.PermissionNameForStudentRecognitionRead,
                        DefaultPermissions.PermissionNameForStudentEqualizationRead,
                        DefaultPermissions.PermissionNameForStudentReassessmentRead,
                        DefaultPermissions.PermissionNameForStudentNoteRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaRead,
                        DefaultPermissions.PermissionNameForStudentInternationalMobilityRead,
                        DefaultPermissions.PermissionNameForStudentSelfGovernmentRead,
                        DefaultPermissions.PermissionNameForStudentSopRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead,
                        DefaultPermissions.PermissionNameForStudentSanctionRead,
                        DefaultPermissions.PermissionNameForStudentAwardRead,
                        DefaultPermissions.PermissionNameForStudentExternalEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentGeneralTrainingDataRead,
                        DefaultPermissions.PermissionNameForStudentCurrentInstitutionDetailsRead,
                        DefaultPermissions.PermissionNameForOresRead
                    }
                },
                {
                    (int)PermissionGroupEnum.PersonalDataReader, new HashSet<string> {
                         DefaultPermissions.PermissionNameForStudentPersonalDataRead,
                    }
                },
                {
                    (int)PermissionGroupEnum.PartialPersonalDataReader, new HashSet<string> {
                         DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                    }
                },
                {
                    (int)PermissionGroupEnum.Owner,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentEducationRead,
                        DefaultPermissions.PermissionNameForStudentInternationalProtectionRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDataManage,
                        DefaultPermissions.PermissionNameForStudentInternationalProtectionManage,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentCreate,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentUpdate,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentDelete,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentRead,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentCreate,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentUpdate,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentDelete,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentRead,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentCreate,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentUpdate,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentDelete,
                        DefaultPermissions.PermissionNameForStudentToClassEnrollment,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForStudentOtherInstitutionRead,
                        DefaultPermissions.PermissionNameForStudentOtherInstitutionManage,
                        DefaultPermissions.PermissionNameForStudentOtherDocumentRead,
                        DefaultPermissions.PermissionNameForStudentOtherDocumentManage,
                        DefaultPermissions.PermissionNameForStudentScholarshipRead,
                        DefaultPermissions.PermissionNameForStudentScholarshipManage,
                        DefaultPermissions.PermissionNameForStudentResourceSupportRead,
                        DefaultPermissions.PermissionNameForStudentResourceSupportManage,
                        DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead,
                        DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage,
                        DefaultPermissions.PermissionNameForStudentAbsenceRead,
                        DefaultPermissions.PermissionNameForStudentAbsenceManage,
                        DefaultPermissions.PermissionNameForStudentRecognitionRead,
                        DefaultPermissions.PermissionNameForStudentRecognitionManage,
                        DefaultPermissions.PermissionNameForStudentEqualizationRead,
                        DefaultPermissions.PermissionNameForStudentEqualizationManage,
                        DefaultPermissions.PermissionNameForStudentReassessmentRead,
                        DefaultPermissions.PermissionNameForStudentReassessmentManage,
                        DefaultPermissions.PermissionNameForStudentNoteRead,
                        DefaultPermissions.PermissionNameForStudentNoteManage,
                        DefaultPermissions.PermissionNameForStudentDiplomaRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaManage,
                        DefaultPermissions.PermissionNameForStudentDiplomaFinalize,
                        DefaultPermissions.PermissionNameForStudentDiplomaAnnulment,
                        DefaultPermissions.PermissionNameForStudentInternationalMobilityRead,
                        DefaultPermissions.PermissionNameForStudentInternationalMobilityManage,
                        DefaultPermissions.PermissionNameForStudentSelfGovernmentRead,
                        DefaultPermissions.PermissionNameForStudentSelfGovernmentManage,
                        DefaultPermissions.PermissionNameForStudentSopRead,
                        DefaultPermissions.PermissionNameForStudentSopManage,
                        DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage,
                        DefaultPermissions.PermissionNameForStudentSanctionRead,
                        DefaultPermissions.PermissionNameForStudentSanctionManage,
                        DefaultPermissions.PermissionNameForStudentAwardRead,
                        DefaultPermissions.PermissionNameForStudentAwardManage,
                        DefaultPermissions.PermissionNameForStudentExternalEvaluationRead,
                        //DefaultPermissions.PermissionNameForStudentExternalEvaluationManage,
                        DefaultPermissions.PermissionNameForStudentEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentEvaluationManage,
                        DefaultPermissions.PermissionNameForStudentEvaluationFinalize,
                        DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage,
                        DefaultPermissions.PermissionNameForStudentGeneralTrainingDataRead,
                        DefaultPermissions.PermissionNameForStudentCurrentInstitutionDetailsRead,
                        DefaultPermissions.PermissionNameForAzureAccountManage,
                        DefaultPermissions.PermissionNameForLodStateManage,
                        DefaultPermissions.PermissionNameForOresRead,
                        DefaultPermissions.PermissionNameForOresManage,
                        DefaultPermissions.PermissionNameForStudentLodFinalizationRead,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentSignLOD,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentSignLOD,
                        DefaultPermissions.PermissionNameForStudentCurriculumRead,
                        DefaultPermissions.PermissionNameForStudentCurriculumManage
                    }
                },
                {
                    (int)PermissionGroupEnum.LeadTeacher,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentEducationRead,
                        DefaultPermissions.PermissionNameForStudentInternationalProtectionRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDataManage,
                        DefaultPermissions.PermissionNameForStudentInternationalProtectionManage,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentRead,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentRead,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForStudentOtherInstitutionRead,
                        DefaultPermissions.PermissionNameForStudentOtherDocumentRead,
                        DefaultPermissions.PermissionNameForStudentScholarshipRead,
                        DefaultPermissions.PermissionNameForStudentScholarshipManage,
                        DefaultPermissions.PermissionNameForStudentResourceSupportRead,
                        DefaultPermissions.PermissionNameForStudentResourceSupportManage,
                        DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead,
                        DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicManage,
                        DefaultPermissions.PermissionNameForStudentAbsenceRead,
                        DefaultPermissions.PermissionNameForStudentRecognitionRead,
                        DefaultPermissions.PermissionNameForStudentEqualizationRead,
                        DefaultPermissions.PermissionNameForStudentReassessmentRead,
                        DefaultPermissions.PermissionNameForStudentNoteRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaRead,
                        DefaultPermissions.PermissionNameForStudentInternationalMobilityRead,
                        DefaultPermissions.PermissionNameForStudentInternationalMobilityManage,
                        DefaultPermissions.PermissionNameForStudentSelfGovernmentRead,
                        DefaultPermissions.PermissionNameForStudentSelfGovernmentManage,
                        DefaultPermissions.PermissionNameForStudentSopRead,
                        DefaultPermissions.PermissionNameForStudentSopManage,
                        DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage,
                        DefaultPermissions.PermissionNameForStudentSanctionRead,
                        DefaultPermissions.PermissionNameForStudentSanctionManage,
                        DefaultPermissions.PermissionNameForStudentAwardRead,
                        DefaultPermissions.PermissionNameForStudentAwardManage,
                        DefaultPermissions.PermissionNameForStudentExternalEvaluationRead,
                        //DefaultPermissions.PermissionNameForStudentExternalEvaluationManage,
                        DefaultPermissions.PermissionNameForStudentEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentEvaluationManage,
                        DefaultPermissions.PermissionNameForStudentEvaluationFinalize,
                        DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationManage,
                        DefaultPermissions.PermissionNameForStudentGeneralTrainingDataRead,
                        DefaultPermissions.PermissionNameForStudentCurrentInstitutionDetailsRead,
                        DefaultPermissions.PermissionNameForStudentEqualizationRead,
                        DefaultPermissions.PermissionNameForStudentEqualizationManage,
                        DefaultPermissions.PermissionNameForOresRead,
                        DefaultPermissions.PermissionNameForOresManage,

                        DefaultPermissions.PermissionNameForStudentDiplomaRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaManage,
                        DefaultPermissions.PermissionNameForLodStateManage,
                    }
                },
                {
                    (int)PermissionGroupEnum.LodReader,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentEducationRead,
                        DefaultPermissions.PermissionNameForStudentInternationalProtectionRead,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead,
                        DefaultPermissions.PermissionNameForStudentDischargeDocumentRead,
                        DefaultPermissions.PermissionNameForStudentRelocationDocumentRead,
                        DefaultPermissions.PermissionNameForStudentClassRead,
                        DefaultPermissions.PermissionNameForStudentOtherDocumentRead,
                        DefaultPermissions.PermissionNameForStudentScholarshipRead,
                        DefaultPermissions.PermissionNameForStudentResourceSupportRead,
                        DefaultPermissions.PermissionNameForStudentEnvironmentCharacteristicRead,
                        DefaultPermissions.PermissionNameForStudentAbsenceRead,
                        DefaultPermissions.PermissionNameForStudentRecognitionRead,
                        DefaultPermissions.PermissionNameForStudentEqualizationRead,
                        DefaultPermissions.PermissionNameForStudentReassessmentRead,
                        DefaultPermissions.PermissionNameForStudentNoteRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaRead,
                        DefaultPermissions.PermissionNameForStudentInternationalMobilityRead,
                        DefaultPermissions.PermissionNameForStudentSelfGovernmentRead,
                        DefaultPermissions.PermissionNameForStudentSopRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead,
                        DefaultPermissions.PermissionNameForStudentSanctionRead,
                        DefaultPermissions.PermissionNameForStudentAwardRead,
                        DefaultPermissions.PermissionNameForStudentExternalEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentPreSchoolEvaluationRead,
                        DefaultPermissions.PermissionNameForStudentGeneralTrainingDataRead,
                        DefaultPermissions.PermissionNameForStudentCurrentInstitutionDetailsRead,
                        DefaultPermissions.PermissionNameForOresRead
                    }
                },
                {
                    (int)PermissionGroupEnum.DiplomaSpecial,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentDiplomaFinalize,
                        DefaultPermissions.PermissionNameForStudentPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaManage,
                    }
                },
                {
                    (int)PermissionGroupEnum.AdmissionDocCreator,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentEducationRead,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentRead,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentCreate,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentUpdate,
                        DefaultPermissions.PermissionNameForStudentAdmissionDocumentDelete,
                    }
                },
                {
                    (int)PermissionGroupEnum.DiplomaCreator,
                    new HashSet<string>
                    {
                        DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestRead,
                        DefaultPermissions.PermissionNameForStudentDiplomaByCreateRequestManage,
                        DefaultPermissions.PermissionNameForStudentPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentPartialPersonalDataRead,
                        DefaultPermissions.PermissionNameForStudentPersonalDataManage
                    }
                },
                {
                    (int)PermissionGroupEnum.StudentCurriculumReader, new HashSet<string> {
                         DefaultPermissions.PermissionNameForStudentCurriculumRead,
                    }
                },
            };
        }

        private static readonly Lazy<StudentPermissionLevelGroups> Instancelock =
                    new Lazy<StudentPermissionLevelGroups>(() => new StudentPermissionLevelGroups());
        public static StudentPermissionLevelGroups GetInstance
        {
            get
            {
                return Instancelock.Value;
            }
        }

        public Dictionary<int, HashSet<string>> All => _groupsPermissions;
    }
}

namespace SB.Domain;

public partial interface IInstitutionsQueryRepository
{
    public record GetVO(
        int InstId,
        string Name,
        InstType InstType,
        int[] SchoolYears,
        int? DirectorPersonId,
        bool ShowDefaultSettingsBanner,
        bool ShowSchoolYearSettingsBanner,
        bool ShowClassBooksBanner,
        bool ShowSpbsBook,
        bool ShowReqBookQualification,
        bool ShowProtocols,
        bool ShowSchoolYearSettings)
    {
        public bool HasCBExtProvider { get; set; } // should be mutable
        public bool SchoolYearIsFinalized { get; set; } // should be mutable

        public bool HasProtocolsReadAccess { get; set; } // should be mutable
        public bool HasProtocolsCreateAccess { get; set; } // should be mutable
        public bool HasProtocolsEditAccess { get; set; } // should be mutable
        public bool HasProtocolsRemoveAccess { get; set; } // should be mutable
        public bool HasCreateClassBooksAccess { get; set; } // should be mutable

        public bool HasBooksReadAccess { get; set; } // should be mutable
        public bool HasSpbsBookCreateAccess { get; set; } // should be mutable
        public bool HasSpbsBookEditAccess { get; set; } // should be mutable

        public bool HasSpbsBookRemoveAccess { get; set; } // should be mutable

        public bool HasMissingTopicsReportAdminCreateAccess { get; set; } // should be mutable
        public bool HasLectureSchedulesReportAdminCreateAccess { get; set; } // should be mutable
        public bool HasOffDayReadAccess { get; set; } // should be mutable
        public bool HasOffDayCreateAccess { get; set; } // should be mutable
        public bool HasOffDayEditAccess { get; set; } // should be mutable
        public bool HasOffDayRemoveAccess { get; set; } // should be mutable
        public bool HasSchoolYearSettingsReadAccess { get; set; } // should be mutable
        public bool HasSchoolYearSettingsCreateAccess { get; set; } // should be mutable
        public bool HasSchoolYearSettingsEditAccess { get; set; } // should be mutable
        public bool HasSchoolYearSettingsRemoveAccess { get; set; } // should be mutable
        public bool HasStudentsAtRiskOfDroppingOutReportAccess { get; set; } // should be mutable
        public bool HasGradelessStudentsReportAccess { get; set; } // should be mutable
        public bool HasSessionStudentsReportAccess { get; set; } // should be mutable
        public bool HasAbsencesByStudentsReportAccess { get; set; } // should be mutable
        public bool HasAbsencesByClassesReportAccess { get; set; } // should be mutable
        public bool HasRegularGradePointAverageByClassesReportAccess { get; set; } // should be mutable
        public bool HasRegularGradePointAverageByStudentsReportAccess { get; set; } // should be mutable
        public bool HasFinalGradePointAverageByStudentsReportAccess { get; set; } // should be mutable
        public bool HasFinalGradePointAverageByClassesReportAccess { get; set; } // should be mutable
        public bool HasDateAbsencesReportAccess { get; set; } // should be mutable
        public bool HasExamsReportAccess { get; set; } // should be mutable
        public bool HasScheduleAndAbsencesByTermReportAccess { get; set; } // should be mutable
        public bool HasScheduleAndAbsencesByMonthReportAccess { get; set; } // should be mutable
        public bool HasScheduleAndAbsencesByTermAllClassesReportAccess { get; set; } // should be mutable
        public bool HasShiftReadAccess { get; set; } // should be mutable
        public bool HasShiftCreateAccess { get; set; } // should be mutable
        public bool HasShiftEditAccess { get; set; } // should be mutable
        public bool HasShiftRemoveAccess { get; set; } // should be mutable
        public bool HasPublicationReadAccess { get; set; } // should be mutable
        public bool HasPublicationCreateAccess { get; set; } // should be mutable
        public bool HasPublicationEditAccess { get; set; } // should be mutable
        public bool HasPublicationRemoveAccess { get; set; } // should be mutable
        public bool HasTeacherAbsencesAllReadAccess { get; set; } // should be mutable
        public bool HasTeacherSchedulesReadAccess { get; set; } // should be mutable
        public bool HasTeacherAbsencesCreateAccess { get; set; } // should be mutable
        public bool HasTeacherAbsencesEditAccess { get; set; } // should be mutable
        public bool HasTeacherAbsencesRemoveAccess { get; set; } // should be mutable
        public bool HasLectureSchedulesCreateAccess { get; set; } // should be mutable
        public bool HasLectureSchedulesEditAccess { get; set; } // should be mutable
        public bool HasLectureSchedulesRemoveAccess { get; set; } // should be mutable
        public bool HasFinalizationAccess { get; set; } // should be mutable
        public bool HasVerificationReadAccess { get; set; } // should be mutable
        public bool HasVerificationWriteAccess { get; set; } // should be mutable
    }
}

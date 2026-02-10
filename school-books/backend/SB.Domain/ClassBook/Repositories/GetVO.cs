namespace SB.Domain;

using System;

public partial interface IClassBooksQueryRepository
{
    public record GetVO(
        int ClassBookId,
        int ClassId,
        ClassBookType BookType,
        string FullBookName,
        string? LeadTeacherName,
        int? BasicClassId,
        bool IsFinalized,
        bool IsValid,
        DateTime ModifyDate,
        DateTime SchoolYearStartDateLimit,
        DateTime SchoolYearStartDate,
        DateTime FirstTermEndDate,
        DateTime SecondTermStartDate,
        DateTime SchoolYearEndDate,
        DateTime SchoolYearEndDateLimit,
        bool HasFutureEntryLock,
        int? PastMonthLockDay)
    {
        public bool HasCBExtProvider { get; set; } // should be mutable
        public bool SchoolYearIsFinalized { get; set; } // should be mutable

        public bool HasCreateAttendanceAccess { get; set; } // should be mutable
        public bool HasRemoveAttendanceAccess { get; set; } // should be mutable
        public bool HasRemoveAbsenceAccess { get; set; } // should be mutable
        public bool HasRemoveDplrAbsenceAccess { get; set; } // should be mutable
        public bool HasEditFirstGradeResultsAccess { get; set; } // should be mutable
        public bool HasEditGradeResultsAccess { get; set; } // should be mutable
        public bool HasEditGradeResultSessionsAccess { get; set; } // should be mutable
        public bool HasCreateIndividualWorkAccess { get; set; } // should be mutable
        public bool HasEditIndividualWorkAccess { get; set; } // should be mutable
        public bool HasRemoveIndividualWorkAccess { get; set; } // should be mutable
        public bool HasCreateNoteAccess { get; set; } // should be mutable
        public bool HasEditNoteAccess { get; set; } // should be mutable
        public bool HasRemoveNoteAccess { get; set; } // should be mutable
        public bool HasCreateParentMeetingAccess { get; set; } // should be mutable
        public bool HasEditParentMeetingAccess { get; set; } // should be mutable
        public bool HasRemoveParentMeetingAccess { get; set; } // should be mutable
        public bool HasCreatePgResultAccess { get; set; } // should be mutable
        public bool HasEditPgResultAccess { get; set; } // should be mutable
        public bool HasRemovePgResultAccess { get; set; } // should be mutable
        public bool HasCreateSanctionAccess { get; set; } // should be mutable
        public bool HasEditSanctionAccess { get; set; } // should be mutable
        public bool HasRemoveSanctionAccess { get; set; } // should be mutable
        public bool HasCreateSupportAccess { get; set; } // should be mutable
        public bool HasCreateAdditionalActivityAccess { get; set; } // should be mutable
        public bool HasEditAdditionalActivityAccess { get; set; } // should be mutable
        public bool HasRemoveAdditionalActivityAccess { get; set; } // should be mutable
        public bool HasCreatePerformanceAccess { get; set; } // should be mutable
        public bool HasEditPerformanceAccess { get; set; } // should be mutable
        public bool HasRemovePerformanceAccess { get; set; } // should be mutable
        public bool HasExportForAllBooksPerformanceAccess { get; set; } // should be mutable
        public bool HasCreateReplrParticipationAccess { get; set; } // should be mutable
        public bool HasEditReplrParticipationAccess { get; set; } // should be mutable
        public bool HasRemoveReplrParticipationAccess { get; set; } // should be mutable
        public DateTime[]? CreateAttendanceReplTeacherAccessDates { get; set; } // should be mutable
    }
}

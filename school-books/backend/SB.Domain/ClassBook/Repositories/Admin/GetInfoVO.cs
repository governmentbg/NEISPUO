namespace SB.Domain;

using System;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetInfoVO(
        ClassBookType BookType,
        string FullBookName,
        int? BasicClassId,
        bool IsFinalized,
        bool IsValid,
        DateTime ModifyDate)
    {
        public bool HasCBExtProvider { get; set; } // should be mutable
        public bool SchoolYearIsFinalized { get; set; } // should be mutable

        public bool HasEditCurriculumAccess { get; set; } // should be mutable
        public bool HasCreateScheduleAccess { get; set; } // should be mutable
        public bool HasEditScheduleAccess { get; set; } // should be mutable
        public bool HasEditSchoolYearProgramAccess { get; set; } // should be mutable
        public bool HasEditStudentAccess { get; set; } // should be mutable
        public bool HasRemoveAccess { get; set; } // should be mutable
        public bool HasCreatePrintAccess { get; set; } // should be mutable
        public bool HasFinalizeAccess { get; set; } // should be mutable
        public bool HasUnfinalizeAccess { get; set; } // should be mutable
    }
}

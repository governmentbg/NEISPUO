namespace SB.Domain;

using System;

public partial interface ISupportsQueryRepository
{
    public record GetVO(
        int SupportId,
        string? Description,
        string? ExpectedResult,
        DateTime EndDate,
        bool IsForAllStudents,
        int[] StudentIds,
        int[] TeacherIds,
        int[] SupportDifficultyTypeIds)
    {
        public bool HasEditAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}

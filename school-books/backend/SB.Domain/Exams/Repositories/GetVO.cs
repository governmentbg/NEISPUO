namespace SB.Domain;

using System;

public partial interface IExamsQueryRepository
{
    public record GetVO(
        int ExamId,
        int CurriculumId,
        DateTime Date,
        string? Description)
    {
        public bool HasEditAccess { get; set; } // should be mutable

        public bool HasRemoveAccess { get; set; } // should be mutable
    }
}

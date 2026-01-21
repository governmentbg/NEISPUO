namespace SB.Domain;

using System;

public class CurriculumTeacher
{
    // EF constructor
    private CurriculumTeacher()
    {
    }

    // only used properties should be mapped

    public int CurriculumId { get; private set; }

    public int StaffPositionId { get; private set; }

    public bool IsValid { get; private set; }

    public DateTime? ValidFrom { get; private set; }

    public DateTime? StaffPositionStartDate { get; private set; }

    public DateTime? StaffPositionTerminationDate { get; private set; }

    public int? SchoolYear { get; private set; }

    public bool NoReplacement { get; private set; }
}

namespace SB.Domain;

using System;

public class AbsenceReason
{
    public const int MedicalAbsenceReasonId = 1;
    public const int FamilyAbsenceReasonId = 2;

    // EF constructor
    private AbsenceReason()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int Id { get; private set; }

    public int? SortOrd { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public bool IsValid { get; private set; }

    public DateTime? ValidFrom { get; private set; }

    public DateTime? ValidTo { get; private set; }
}

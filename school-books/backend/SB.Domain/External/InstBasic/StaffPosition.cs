namespace SB.Domain;

using System;

public class StaffPosition
{
    public const int DefaultPositionKindId = 1;
    public const int PedagogicalStaffTypeId = 1;

    // EF constructor
    private StaffPosition()
    {
    }

    // only used properties should be mapped

    public int InstitutionId { get; private set; }

    public int StaffPositionId { get; private set; }

    public int PersonId { get; private set; }

    public bool CurrentlyValid { get; private set; }

    public int NKPDPositionId { get; private set; }

    public int? PositionKindId { get; private set; }

    public bool IsValid { get; private set; }

    public int? StaffTypeId { get; private set; }

    public DateTime? TerminationDate { get; private set; }
}

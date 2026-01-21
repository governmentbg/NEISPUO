namespace SB.Domain;

using System;

public class SupportDifficultyType
{
    // EF constructor
    private SupportDifficultyType()
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

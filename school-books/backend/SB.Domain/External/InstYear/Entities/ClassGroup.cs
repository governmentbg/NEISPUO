namespace SB.Domain;

public class ClassGroup
{
    // EF constructor
    private ClassGroup()
    {
        this.ClassName = null!;
    }

    // only used properties should be mapped

    public int ClassId { get; private set; }

    public int SchoolYear { get; private set; }

    public int InstitutionId { get; private set; }

    public string ClassName { get; private set; }

    public string? ParalellClassName { get; private set; }

    public int? ParentClassId { get; private set; }

    public int? BasicClassId { get; private set; }

    public int? ClassTypeId { get; private set; }

    public bool? IsCombined { get; private set; }

    public bool? IsSpecNeed { get; private set; }

    public bool? IsNotPresentForm { get; private set; }

    public int? ClassSpecialityId { get; private set; }

    public bool IsValid { get; private set; }
}

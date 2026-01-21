namespace SB.Domain;

public class ClassType
{
    public static readonly int[] DormitoryClassTypes = new[] { 35, 39, 49 };
    public const int CsopPgClassTypeId = 88;

    // EF constructor
    private ClassType()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int ClassTypeId { get; private set; }

    public string Name { get; private set; }

    public ClassKind ClassKind { get; private set; }
}

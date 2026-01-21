namespace SB.Domain;

using System.ComponentModel.DataAnnotations.Schema;

public class CustomVarValue
{
    // EF constructor
    private CustomVarValue()
    {
    }

    public int CustomVarValueId { get; private set; }
    public int InstitutionId { get; private set; }
    public int CustomVarId { get; private set; }
    [Column(name: "CustomVarValue")]
    public int? CustomVarVal { get; private set; } //change the name of the property otherwise will get error "member names cannot be the same as their enclosing type"
    public int? CustomVarValueAdd1 { get; private set; }
    public int? CustomVarValueAdd2 { get; private set; }
    public int? CustomVarValueAdd3 { get; private set; }
    public bool? IsValid { get; private set; }
}

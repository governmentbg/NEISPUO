namespace SB.Domain;
public class AdmissionReasonType
{
    // EF constructor
    private AdmissionReasonType()
    {
        this.Name = null!;
    }

    // only used properties should be mapped

    public int Id { get; private set; }

    public string Name { get; private set; }
}

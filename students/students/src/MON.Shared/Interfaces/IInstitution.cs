namespace MON.Shared.Interfaces
{
    public interface IInstitution
    {
        int? InstitutionId { get; set; }
    }

    public interface IInstitutionNotNullable
    {
        int InstitutionId { get; set; }
    }
}

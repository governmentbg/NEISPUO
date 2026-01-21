namespace SB.Data;

public partial interface IInstTeacherNomsRepository
{
    public record InstTeacherNomVO(int Id, string Name, string FirstName, string LastName, string? badge);
}

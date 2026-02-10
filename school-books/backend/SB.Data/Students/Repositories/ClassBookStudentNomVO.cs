namespace SB.Data;

public partial interface IClassBookStudentNomsRepository
{
    public record ClassBookStudentNomVO(int Id, string Name, string? Badge);
}

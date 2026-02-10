namespace SB.Domain;
public partial interface IClassBooksQueryRepository
{
    public record GetDefaultCurriculumVO(int? CurriculumId);
}

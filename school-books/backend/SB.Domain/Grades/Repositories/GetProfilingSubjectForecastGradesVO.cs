namespace SB.Domain;

public partial interface IGradesQueryRepository
{
    public record GetProfilingSubjectForecastGradesVO(
        int PersonId,
        decimal ProfilingSubjectFinalGrade);
}

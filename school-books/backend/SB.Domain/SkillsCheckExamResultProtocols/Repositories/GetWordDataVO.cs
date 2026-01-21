namespace SB.Domain;
public partial interface ISkillsCheckExamResultProtocolsQueryRepository
{
    public record GetWordDataVO(
        string SchoolYear,
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string? ProtocolNumber,
        string SubjectName,
        string? Date,
        GetWordDataVOEvaluator[] Evaluators,
        int StudentsCapacity);

    public record GetWordDataVOEvaluator(
        string Name);
}

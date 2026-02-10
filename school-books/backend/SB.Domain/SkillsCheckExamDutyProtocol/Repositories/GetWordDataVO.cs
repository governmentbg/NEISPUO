namespace SB.Domain;
public partial interface ISkillsCheckExamDutyProtocolsQueryRepository
{
    public record GetWordDataVO(
        string SchoolYear,
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string? ProtocolNumber,
        string? ProtocolDate,
        string SubjectName,
        string Date,
        string DirectorName,
        int StudentsCapacity,
        GetWordDataVOSupervisor[] Supervisors);

    public record GetWordDataVOSupervisor(
       string SupervisorName);
}

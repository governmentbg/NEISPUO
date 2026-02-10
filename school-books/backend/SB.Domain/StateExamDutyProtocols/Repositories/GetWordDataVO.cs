namespace SB.Domain;
public partial interface IStateExamDutyProtocolsQueryRepository
{
    public record GetWordDataVO(
        string SchoolYear,
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string? ProtocolNumber,
        string? ProtocolDate,
        string? SessionType,
        string SubjectName,
        string? EduFormName,
        string OrderNumber,
        string OrderDate,
        string Date,
        int ModulesCount,
        string? RoomNumber,
        GetWordDataVOSupervisor[] Supervisors);

    public record GetWordDataVOSupervisor(
       string SupervisorName);
}

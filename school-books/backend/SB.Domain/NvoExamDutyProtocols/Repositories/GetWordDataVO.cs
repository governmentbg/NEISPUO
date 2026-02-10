namespace SB.Domain;
public partial interface INvoExamDutyProtocolsQueryRepository
{
    public record GetWordDataVO(
        string SchoolYear,
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string? ProtocolNumber,
        string? ProtocolDate,
        string BasicClassName,
        string SubjectName,
        string Date,
        string? RoomNumber,
        string DirectorName,
        GetWordDataVOSupervisor[] Supervisors,
        GetWordDataVOStudent[] Students);

    public record GetWordDataVOSupervisor(
       string SupervisorName);

    public record GetWordDataVOStudent(
       string StudentName,
       string ClassName
       );
}

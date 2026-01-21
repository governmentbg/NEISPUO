namespace SB.Domain;
public partial interface IExamDutyProtocolsQueryRepository
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
        string ExamType,
        string ExamSubType,
        string OrderNumber,
        string OrderDate,
        string Date,
        string? GroupNum,
        string[] ClassNames,
        GetWordDataVOSupervisor[] Supervisors,
        GetWordDataVOStudent[] Students);

    public record GetWordDataVOSupervisor(
       string SupervisorName);

    public record GetWordDataVOStudent(
       string StudentName,
       string ClassName
       );
}

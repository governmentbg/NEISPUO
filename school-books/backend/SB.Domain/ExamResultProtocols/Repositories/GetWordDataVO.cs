namespace SB.Domain;
public partial interface IExamResultProtocolsQueryRepository
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
        string? GroupNum,
        string[] ClassNames,
        string? EduFormName,
        string ProtocolType,
        string ExamType,
        string Date,
        string CommissionNominationOrderNumber,
        string CommissionNominationOrderDate,
        string ChairmanName,
        GetWordDataVOCommissioner[] CommissionMembers,
        GetWordDataVOCommissionerLeftRight[] CommissionMembersDivided,
        GetWordDataVOStudent[] Students);

    public record GetWordDataVOCommissioner(
        string CommissionerName);

    public record GetWordDataVOCommissionerLeftRight(
        string CommissionerNameLeft,
        string CommissionerNameRight);

    public record GetWordDataVOStudent(
       string StudentName,
       string ClassName
       );
}

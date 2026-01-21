namespace SB.Domain;

public partial interface IGraduationThesisDefenseProtocolQueryRepository
{
    public record GetWordDataVO(
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string SchoolYear,
        string? ProtocolNumber,
        string? ProtocolDate,
        string? SessionType,
        string? EduFormName,
        string CommissionMeetingDate,
        string DirectorOrderNumber,
        string DirectorOrderDate,
        string DirectorName,
        string DirectorNameInParentheses,
        string ChairmanName,
        GetWordDataVOCommissioner[] CommissionMembers,
        GetWordDataVOCommissionerLeftRight[] CommissionMembersDivided,
        int Section1StudentsCapacity,
        int Section2StudentsCapacity,
        int Section3StudentsCapacity,
        int Section4StudentsCapacity);

    public record GetWordDataVOCommissioner(
        string CommissionerName);

    public record GetWordDataVOCommissionerLeftRight(
        string CommissionerNameLeft,
        string CommissionerNameRight);
}

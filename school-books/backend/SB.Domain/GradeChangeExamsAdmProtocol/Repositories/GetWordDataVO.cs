namespace SB.Domain;

public partial interface IGradeChangeExamsAdmProtocolQueryRepository
{
    public record GetWordDataVO(
        string InstName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        string SchoolYear,
        string? ExamSession,
        string? ProtocolNum,
        string? ProtocolDate,
        string CommissionMeetingDate,
        string CommissionNominationOrderNumber,
        string CommissionNominationOrderDate,
        string DirectorName,
        string DirectorNameInParentheses,
        string ChairmanName,
        GetWordDataVOCommissioner[] CommissionMembers,
        GetWordDataVOCommissionerLeftRight[] CommissionMembersDivided,
        GetWordDataVOStudent[] Students);

    public record GetWordDataVOStudent(
        string ClassName,
        string StudentName,
        string Subjects);

    public record GetWordDataVOCommissioner(
        string CommissionerName);

    public record GetWordDataVOCommissionerLeftRight(
        string CommissionerNameLeft,
        string CommissionerNameRight);
}

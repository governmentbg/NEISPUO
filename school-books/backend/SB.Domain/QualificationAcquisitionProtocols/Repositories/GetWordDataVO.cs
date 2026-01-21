namespace SB.Domain;
public partial interface IQualificationAcquisitionProtocolsQueryRepository
{
    public record GetWordDataVO(
        string SchoolYear,
        string InstitutionName,
        string InstitutionTownName,
        string InstitutionMunicipalityName,
        string InstitutionRegionName,
        QualificationAcquisitionProtocolType ProtocolType,
        string? ProtocolNumber,
        string? ProtocolDate,
        string Profession,
        string Speciality,
        string QualificationDegree,
        string Date,
        string CommissionNominationOrderNumber,
        string CommissionNominationOrderDate,
        string DirectorName,
        string DirectorNameInParentheses,
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
        string ClassName,
        string StudentName,
        bool ExamsPassed,
        string TheoryPoints,
        string PracticePoints,
        string TotalPoints,
        string AverageDecimalGrade,
        string AverageDecimalGradeText);
}

namespace SB.Domain;

using System;

public partial interface IQualificationExamResultProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int QualificationExamResultProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string? SessionType,
        string Profession,
        string Speciality,
        int QualificationDegreeId,
        string? GroupNum,
        int[] ClassIds,
        int? EduFormId,
        int QualificationExamTypeId,
        DateTime Date,
        string CommissionNominationOrderNumber,
        DateTime CommissionNominationOrderDate,
        int CommissionChairman,
        int[] CommissionMembers);
}

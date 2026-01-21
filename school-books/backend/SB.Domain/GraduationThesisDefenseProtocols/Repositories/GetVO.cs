namespace SB.Domain;

using System;

public partial interface IGraduationThesisDefenseProtocolQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int GraduationThesisDefenseProtocolId,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string? SessionType,
        int? EduFormId,
        DateTime CommissionMeetingDate,
        string DirectorOrderNumber,
        DateTime DirectorOrderDate,
        int DirectorPersonId,
        int CommissionChairman,
        int[] CommissionMembers,
        int Section1StudentsCapacity,
        int Section2StudentsCapacity,
        int Section3StudentsCapacity,
        int Section4StudentsCapacity);
}

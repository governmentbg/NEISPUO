namespace SB.Domain;

using System;

public partial interface IQualificationAcquisitionProtocolsQueryRepository
{
    public record GetVO(
        int SchoolYear,
        int QualificationAcquisitionProtocolId,
        QualificationAcquisitionProtocolType ProtocolType,
        string? ProtocolNumber,
        DateTime? ProtocolDate,
        string Profession,
        string Speciality,
        int QualificationDegreeId,
        DateTime Date,
        string CommissionNominationOrderNumber,
        DateTime CommissionNominationOrderDate,
        int DirectorPersonId,
        int CommissionChairman,
        int[] CommissionMembers);
}

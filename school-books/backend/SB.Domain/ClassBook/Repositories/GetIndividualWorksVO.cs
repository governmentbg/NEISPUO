namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetIndividualWorksVO(
        int? ClassNumber,
        string FullName,
        bool IsTransferred,
        DateTime Date,
        string IndividualWorkActivity);
}

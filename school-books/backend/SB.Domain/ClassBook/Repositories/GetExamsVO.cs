namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetExamsVO(
        string SubjectName,
        string SubjectTypeName,
        DateTime[] FirstTermDates,
        DateTime[] SecondTermDates
    );
}

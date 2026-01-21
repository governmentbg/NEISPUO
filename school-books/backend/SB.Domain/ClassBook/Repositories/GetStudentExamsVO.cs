namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentExamsVO(
        string SubjectName,
        string SubjectTypeName,
        DateTime[] FirstTermDates,
        DateTime[] SecondTermDates
    );
}

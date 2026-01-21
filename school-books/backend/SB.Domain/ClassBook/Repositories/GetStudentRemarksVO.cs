namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentRemarksVO(
        SchoolTerm Term,
        DateTime Date,
        string SubjectName,
        string SubjectTypeName,
        string Description
    );
}

namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetRemarksVO(
        int? ClassNumber,
        string FullName,
        bool IsTransferred,
        DateTime Date,
        string RemarkType,
        string SubjectName,
        string SubjectTypeName,
        string Description);
}

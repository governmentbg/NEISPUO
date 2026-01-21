namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentNotesVO(
        DateTime DateCreated,
        string CreatedBySysUserName,
        string? Description
    );
}

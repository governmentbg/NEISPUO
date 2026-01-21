namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetNotesVO(
        DateTime DateCreated,
        string CreatedBySysUserName,
        string? Description);
}

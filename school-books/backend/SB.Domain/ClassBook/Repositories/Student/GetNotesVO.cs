namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetNotesVO(
        DateTime DateCreated,
        string CreatedBySysUserName,
        string? Description);
}

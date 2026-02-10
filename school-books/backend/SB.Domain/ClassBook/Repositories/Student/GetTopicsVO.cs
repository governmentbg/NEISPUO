namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetTopicsVO(
        int Hour,
        DateTime Date,
        bool IsOffDay,
        string[] Titles);
}

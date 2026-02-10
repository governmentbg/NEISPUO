namespace SB.Domain;

using Microsoft.EntityFrameworkCore;
using System;

public partial interface IClassBookStudentPrintRepository
{
    [Keyless]
    public record GetStudentAbsencesByDateVO(
        DateTime Date,
        int LateCount,
        int UnexcusedCount,
        int ExcusedCount
    );
}

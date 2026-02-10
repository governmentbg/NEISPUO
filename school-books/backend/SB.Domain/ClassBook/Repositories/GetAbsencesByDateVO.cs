namespace SB.Domain;

using Microsoft.EntityFrameworkCore;
using System;

public partial interface IClassBookPrintRepository
{
    [Keyless]
    public record GetAbsencesByDateVO(
        int PersonId,
        DateTime Date,
        int LateCount,
        int UnexcusedCount,
        int ExcusedCount
    );
}

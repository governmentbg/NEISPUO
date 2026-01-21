namespace SB.Domain;

using Microsoft.EntityFrameworkCore;

public partial interface IAbsencesDplrQueryRepository
{
    [Keyless]
    public record GetAllDplrForClassBookVO(
        int PersonId,
        int Count);
}

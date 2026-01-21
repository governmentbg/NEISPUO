namespace SB.Domain;

using Microsoft.EntityFrameworkCore;

public partial interface IAbsencesQueryRepository
{
    [Keyless]
    public record GetAllForClassBookVO(
        int PersonId,
        int LateAbsencesCount,
        int UnexcusedAbsencesCount,
        int ExcusedAbsencesCount,
        int CarriedLateAbsencesCount,
        int CarriedUnexcusedAbsencesCount,
        int CarriedExcusedAbsencesCount);
}

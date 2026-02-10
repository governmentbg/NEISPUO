namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookOffDayDatesRepository : IRepository
{
    Task<bool> ExistsClassBookOffDayDateAsync(
        int schoolYear,
        int offDayId,
        CancellationToken ct);

    Task UpdateClassBookOffDayDatesAsync(
        int schoolYear,
        int offDayId,
        CancellationToken ct);
}

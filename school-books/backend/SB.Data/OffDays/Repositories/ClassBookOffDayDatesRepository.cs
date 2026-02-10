namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class ClassBookOffDayDatesRepository : Repository, IClassBookOffDayDatesRepository
{
    public ClassBookOffDayDatesRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<bool> ExistsClassBookOffDayDateAsync(
        int schoolYear,
        int offDayId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ClassBookOffDayDate>()
            .AnyAsync(
                cbod => cbod.SchoolYear == schoolYear &&
                    cbod.OffDayId == offDayId,
                ct);
    }

    public async Task UpdateClassBookOffDayDatesAsync(
        int schoolYear,
        int offDayId,
        CancellationToken ct)
    {
        string sql = """
            UPDATE cbod
            SET
                IsPgOffProgramDay = od.IsPgOffProgramDay
            FROM [school_books].[ClassBookOffDayDate] cbod
            JOIN [school_books].[OffDay] od
                ON cbod.SchoolYear = od.SchoolYear
                AND cbod.OffDayId = od.OffDayId
            WHERE od.SchoolYear = @schoolYear
                AND od.OffDayId = @offDayId
            """;

        await this.DbContext.Database.ExecuteSqlRawAsync(
            sql,
            new[]
            {
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("offDayId", offDayId),
            },
            ct);
    }
}

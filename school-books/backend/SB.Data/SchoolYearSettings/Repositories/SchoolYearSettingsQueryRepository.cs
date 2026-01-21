namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.ISchoolYearSettingsQueryRepository;

internal class SchoolYearSettingsQueryRepository : Repository, ISchoolYearSettingsQueryRepository
{
    private const int CommonSportDetailedSchoolTypeId = 8;
    private const int SpecialSportDetailedSchoolTypeId = 114;

    public SchoolYearSettingsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var res = await (
            from sys in this.DbContext.Set<SchoolYearSettings>()

            where sys.SchoolYear == schoolYear &&
                sys.InstId == instId

            orderby sys.CreateDate

            select new GetAllVO(
                sys.SchoolYearSettingsId,
                sys.SchoolYearStartDate,
                sys.FirstTermEndDate,
                sys.SecondTermStartDate,
                sys.SchoolYearEndDate,
                sys.Description,
                sys.IsForAllClasses,
                Array.Empty<string>(),
                Array.Empty<string>())
        ).ToTableResultAsync(offset, limit, ct);

        var ids = res.Result.Select(syd => syd.SchoolYearSettingsId).ToArray();

        var basicClassNames = (await (
            from sysc in this.DbContext.Set<SchoolYearSettingsClass>()
            join bc in this.DbContext.Set<BasicClass>()
            on sysc.BasicClassId equals bc.BasicClassId

            where sysc.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => sysc.SchoolYearSettingsId == id.Id)
            select new
            {
                sysc.SchoolYearSettingsId,
                bc.Name
            }).ToArrayAsync(ct))
            .ToLookup(r => r.SchoolYearSettingsId, r => r.Name);

        var classBookNames = (await (
            from syscb in this.DbContext.Set<SchoolYearSettingsClassBook>()
            join cb in this.DbContext.Set<ClassBook>()
            on new { syscb.SchoolYear, syscb.ClassBookId }
            equals new { cb.SchoolYear, cb.ClassBookId }

            where syscb.SchoolYear == schoolYear &&
                cb.IsValid &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => syscb.SchoolYearSettingsId == id.Id)
            select new
            {
                syscb.SchoolYearSettingsId,
                cb.FullBookName
            }).ToArrayAsync(ct))
            .ToLookup(r => r.SchoolYearSettingsId, r => r.FullBookName);

        var resFixup = res with
            {
                Result = res.Result
                    .Select(r =>
                        r with
                        {
                            BasicClassNames = basicClassNames[r.SchoolYearSettingsId].ToArray(),
                            ClassBookNames = classBookNames[r.SchoolYearSettingsId].ToArray(),
                        })
                    .ToArray()
            };

        return resFixup;
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int schoolYearSettingsId,
        CancellationToken ct)
    {
        return await (
            from sys in this.DbContext.Set<SchoolYearSettings>()

            where sys.SchoolYear == schoolYear &&
                sys.SchoolYearSettingsId == schoolYearSettingsId

            select new GetVO(
                sys.SchoolYearSettingsId,
                sys.SchoolYearStartDate,
                sys.FirstTermEndDate,
                sys.SecondTermStartDate,
                sys.SchoolYearEndDate,
                sys.Description,
                sys.HasFutureEntryLock,
                sys.PastMonthLockDay,
                sys.IsForAllClasses,
                (from sysc in this.DbContext.Set<SchoolYearSettingsClass>()
                    where sysc.SchoolYear == sys.SchoolYear &&
                        sysc.SchoolYearSettingsId == sys.SchoolYearSettingsId
                    select sysc.BasicClassId
                    ).ToArray(),
                (from syscb in this.DbContext.Set<SchoolYearSettingsClassBook>()
                    where syscb.SchoolYear == sys.SchoolYear &&
                        syscb.SchoolYearSettingsId == sys.SchoolYearSettingsId
                    select syscb.ClassBookId
                    ).ToArray())
        ).SingleAsync(ct);
    }

    public async Task<GetAllForRebuildVO[]> GetAllForRebuildAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from sys in this.DbContext.Set<SchoolYearSettings>()

            where sys.SchoolYear == schoolYear &&
                sys.InstId == instId

            select new GetAllForRebuildVO(
                sys.SchoolYearSettingsId,
                sys.SchoolYearStartDate,
                sys.FirstTermEndDate,
                sys.SecondTermStartDate,
                sys.SchoolYearEndDate,
                sys.HasFutureEntryLock,
                sys.PastMonthLockDay,
                sys.IsForAllClasses,
                (from sysc in this.DbContext.Set<SchoolYearSettingsClass>()
                    where sysc.SchoolYear == sys.SchoolYear &&
                        sysc.SchoolYearSettingsId == sys.SchoolYearSettingsId
                    select sysc.BasicClassId
                    ).ToArray(),
                (from syscb in this.DbContext.Set<SchoolYearSettingsClassBook>()
                    where syscb.SchoolYear == sys.SchoolYear &&
                        syscb.SchoolYearSettingsId == sys.SchoolYearSettingsId
                    select syscb.ClassBookId
                    ).ToArray())
            )
            .AsSplitQuery()
            .ToArrayAsync(ct);
    }

    public async Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.IsValid

            select new GetAllClassBooksVO(
                cb.SchoolYear,
                cb.ClassBookId,
                cb.BasicClassId,
                (from cg in this.DbContext.Set<ClassGroup>()
                where cg.ParentClassId == cb.ClassId
                select cg.BasicClassId
                ).ToArray())
        ).ToArrayAsync(ct);
    }

    public async Task<bool> ExistsIsForAllClassesAsync(
        int schoolYear,
        int instId,
        int? exceptSchoolYearSettingsId,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<SchoolYearSettings>();
        if (exceptSchoolYearSettingsId != null)
        {
            predicate = predicate.And(syd => syd.SchoolYearSettingsId != exceptSchoolYearSettingsId);
        }

        return await this.DbContext.Set<SchoolYearSettings>()
            .Where(predicate)
            .AnyAsync(
                sys =>
                    sys.SchoolYear == schoolYear &&
                    sys.InstId == instId &&
                    sys.IsForAllClasses,
                ct);
    }

    public async Task<GetDefaultVO> GetDefaultAsync(
        int schoolYear,
        CancellationToken ct)
    {
        return await this.DbContext.Set<SchoolYearSettingsDefault>()
            .Where(sysd => sysd.SchoolYear == schoolYear)
            .Select(sysd => new GetDefaultVO(
                sysd.PgSchoolYearStartDateLimit,
                sysd.PgSchoolYearStartDate,
                sysd.PgFirstTermEndDate,
                sysd.PgSecondTermStartDate,
                sysd.PgSchoolYearEndDate,
                sysd.PgSchoolYearEndDateLimit,
                sysd.SportSchoolYearStartDateLimit,
                sysd.SportSchoolYearStartDate,
                sysd.SportFirstTermEndDate,
                sysd.SportSecondTermStartDate,
                sysd.SportSchoolYearEndDate,
                sysd.SportSchoolYearEndDateLimit,
                sysd.CplrSchoolYearStartDateLimit,
                sysd.CplrSchoolYearStartDate,
                sysd.CplrFirstTermEndDate,
                sysd.CplrSecondTermStartDate,
                sysd.CplrSchoolYearEndDate,
                sysd.CplrSchoolYearEndDateLimit,
                sysd.OtherSchoolYearStartDateLimit,
                sysd.OtherSchoolYearStartDate,
                sysd.OtherFirstTermEndDate,
                sysd.OtherSecondTermStartDate,
                sysd.OtherSchoolYearEndDate,
                sysd.OtherSchoolYearEndDateLimit))
            .SingleAsync(ct);
    }

    public async Task<bool> ExistsSchoolYearSettingsDefaultAsync(
        int schoolYear,
        CancellationToken ct)
    {
        return await this.DbContext.Set<SchoolYearSettingsDefault>()
            .AnyAsync(sysd => sysd.SchoolYear == schoolYear, ct);
    }

    public async Task<bool> IsSportSchoolAsync(int schoolYear, int instId, CancellationToken ct)
    {
        return await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select (d.DetailedSchoolTypeId == CommonSportDetailedSchoolTypeId ||
                d.DetailedSchoolTypeId == SpecialSportDetailedSchoolTypeId)
        ).SingleAsync(ct);
    }

    public async Task<bool> IsCplrAsync(int schoolYear, int instId, CancellationToken ct)
    {
        return await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select d.InstType == InstType.CPLR
        ).SingleAsync(ct);
    }

    public async Task RemoveSchoolYearSettingsLinkAsync(
        int schoolYear,
        int instId,
        int schoolYearSettingsId,
        CancellationToken ct)
    {
        string sql = """
            UPDATE cbsys
            SET
                [SchoolYearSettingsId] = NULL
            FROM [school_books].[ClassBookSchoolYearSettings] cbsys
            JOIN [school_books].[ClassBook] cb
                ON cbsys.[SchoolYear] = cb.[SchoolYear]
                AND cbsys.[ClassBookId] = cb.[ClassBookId]
            WHERE
                cb.[SchoolYear] = @schoolYear
                AND cb.[InstId] = @instId
                AND cbsys.[SchoolYearSettingsId] = @schoolYearSettingsId
            """;

        await this.DbContext.Database
            .ExecuteSqlRawAsync(
                sql,
                new[]
                {
                    new SqlParameter("schoolYear", schoolYear),
                    new SqlParameter("instId", instId),
                    new SqlParameter("schoolYearSettingsId", schoolYearSettingsId),
                },
                ct);
    }
}

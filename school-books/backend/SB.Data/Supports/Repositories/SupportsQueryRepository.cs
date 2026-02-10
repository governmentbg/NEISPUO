namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ISupportsQueryRepository;

internal class SupportsQueryRepository : SupportsAggregateRepository, ISupportsQueryRepository
{
    public SupportsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var activities = (await (
            from s in this.DbContext.Set<Support>()

            join a in this.DbContext.Set<SupportActivity>()
                on new { s.SchoolYear, s.SupportId }
                equals new { a.SchoolYear, a.SupportId }

            join sat in this.DbContext.Set<SupportActivityType>() on a.SupportActivityTypeId equals sat.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby a.SupportActivityId

            select new
            {
                s.SupportId,
                sat.Name
            })
            .ToArrayAsync(ct))
            .ToLookup(
                a => a.SupportId,
                a => a.Name);

        var difficulties = (await (
            from s in this.DbContext.Set<Support>()

            join d in this.DbContext.Set<SupportDifficulty>()
                on new { s.SchoolYear, s.SupportId }
                equals new { d.SchoolYear, d.SupportId }

            join sdt in this.DbContext.Set<SupportDifficultyType>() on d.SupportDifficultyTypeId equals sdt.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new
            {
                s.SupportId,
                sdt.Name
            })
            .ToArrayAsync(ct))
            .ToLookup(
                d => d.SupportId,
                d => d.Name);

        var students = (await (
            from s in this.DbContext.Set<Support>()

            join st in this.DbContext.Set<SupportStudent>()
                on new { s.SchoolYear, s.SupportId }
                equals new { st.SchoolYear, st.SupportId }
            join p in this.DbContext.Set<Person>()
                on st.PersonId
                equals p.PersonId

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new
            {
                s.SupportId,
                p.FirstName,
                p.MiddleName,
                p.LastName
            })
            .ToArrayAsync(ct))
            .ToLookup(
                ss => ss.SupportId,
                ss => StringUtils.JoinNames(ss.FirstName, ss.MiddleName, ss.LastName));

        return await (
           from s in this.DbContext.Set<Support>()

           where s.SchoolYear == schoolYear &&
               s.ClassBookId == classBookId

           orderby s.SupportId

           select new GetAllVO(
               s.SupportId,
               s.IsForAllStudents,
               string.Join(", ", difficulties[s.SupportId]),
               string.Join(", ", students[s.SupportId]),
               string.Join(", ", activities[s.SupportId]),
               s.CreateDate,
               s.EndDate))
           .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int supportId,
        CancellationToken ct)
    {
        var support = await this.FindEntityAsync(
            s =>
                s.SchoolYear == schoolYear &&
                s.SupportId == supportId,
            ct);

        return new GetVO(
            support.SupportId,
            support.Description,
            support.ExpectedResult,
            support.EndDate,
            support.IsForAllStudents,
            support.Students.Select(s => s.PersonId).ToArray(),
            support.Teachers.Select(t => t.PersonId).ToArray(),
            support.DifficultyTypes.Select(d => d.SupportDifficultyTypeId).ToArray());
    }

    public async Task<TableResultVO<GetActivityAllVO>> GetActivityAllAsync(
        int schoolYear,
        int supportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from sa in this.DbContext.Set<SupportActivity>()
            join sat in this.DbContext.Set<SupportActivityType>() on sa.SupportActivityTypeId equals sat.Id

            where sa.SchoolYear == schoolYear &&
               sa.SupportId == supportId

            orderby sa.SupportActivityId

            select new GetActivityAllVO(
                sa.SupportActivityId,
                sat.Name,
                sa.Date,
                sa.Target,
                sa.Result))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetActivityVO> GetActivityAsync(
       int schoolYear,
       int supportId,
       int supportActivityId,
       CancellationToken ct)
    {
        return await this.DbContext.Set<SupportActivity>()
            .Where(sa =>
                sa.SchoolYear == schoolYear &&
                sa.SupportId == supportId &&
                sa.SupportActivityId == supportActivityId)
            .Select(sa => new GetActivityVO(
                sa.SupportActivityTypeId,
                sa.Target,
                sa.Result,
                sa.Date))
            .SingleAsync(ct);
    }
}

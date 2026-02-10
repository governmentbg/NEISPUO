namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;

internal class SchedulesAggregateRepository : ScopedAggregateRepository<Schedule>, ISchedulesAggregateRepository
{
    public SchedulesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Schedule>, IQueryable<Schedule>>[] Includes =>
        new Func<IQueryable<Schedule>, IQueryable<Schedule>>[]
        {
            (q) => q.Include(e => e.Dates),
            (q) => q.Include(e => e.Hours),
            (q) => q.Include(e => e.Lessons)
        };

    public async Task<Schedule[]> FindAllByClassBookAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            s =>
                s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId,
            ct)
            ).ToArray();
    }

    public async Task<Schedule[]> FindSchedulesByDateAsync(int schoolYear, int classBookId, DateTime date, CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            s =>
                s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.Dates.Any(s => s.Date == date),
            ct)
            ).ToArray();
    }

    public async Task<ScheduleLesson[]> FindScheduleLessonsAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        CancellationToken ct)
    {
        return await (
            from sl in this.DbContext.Set<ScheduleLesson>()

            join s in this.DbContext.Set<Schedule>()
            on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>()
            on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.IsValid &&
                this.DbContext
                    .MakeIdsQuery(scheduleLessonIds)
                    .Any(id => sl.ScheduleLessonId == id.Id)

            select sl
        ).ToArrayAsync(ct);
    }

    public async Task RemoveScheduleLessonsForDateAsync(
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct)
    {
        await (
            from sl in this.DbContext.Set<ScheduleLesson>()

            join s in this.DbContext.Set<Schedule>()
            on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>()
            on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId &&
                cb.IsValid &&
                sl.Date == date
            select sl
        ).ExecuteDeleteAsync(ct);
    }
}

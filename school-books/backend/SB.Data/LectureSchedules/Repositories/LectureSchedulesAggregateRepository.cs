namespace SB.Data;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class LectureSchedulesAggregateRepository : ScopedAggregateRepository<LectureSchedule>
{
    public LectureSchedulesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<LectureSchedule>, IQueryable<LectureSchedule>>[] Includes =>
        new Func<IQueryable<LectureSchedule>, IQueryable<LectureSchedule>>[]
        {
            (q) => q.Include(e => e.Hours)
        };
}

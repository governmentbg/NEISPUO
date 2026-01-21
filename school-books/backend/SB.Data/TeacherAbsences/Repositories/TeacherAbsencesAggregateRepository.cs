namespace SB.Data;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class TeacherAbsencesAggregateRepository : ScopedAggregateRepository<TeacherAbsence>
{
    public TeacherAbsencesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<TeacherAbsence>, IQueryable<TeacherAbsence>>[] Includes =>
        new Func<IQueryable<TeacherAbsence>, IQueryable<TeacherAbsence>>[]
        {
            (q) => q.Include(e => e.Hours)
        };
}

namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ClassBooksAggregateRepository : ScopedAggregateRepository<ClassBook>, IClassBooksAggregateRepository
{
    public ClassBooksAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<ClassBook>, IQueryable<ClassBook>>[] Includes =>
        new Func<IQueryable<ClassBook>, IQueryable<ClassBook>>[]
        {
            (q) => q.Include(r => r.GradelessCurriculums),
            (q) => q.Include(r => r.GradelessStudents),
            (q) => q.Include(r => r.SpecialNeedsStudents),
            (q) => q.Include(r => r.FirstGradeResultSpecialNeedsStudents),
            (q) => q.Include(r => r.StudentActivities),
            (q) => q.Include(r => r.StudentCarriedAbsences),
            (q) => q.Include(r => r.Prints).ThenInclude(r => r.Signatures),
            (q) => q.Include(r => r.StudentPrints),
            (q) => q.Include(r => r.StatusChanges),
        };

    public Task<ClassBook?> FindOrDefaultAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return this.FindEntityOrDefaultAsync(
            this.DbContext.Set<ClassBook>(),
            new object[] { schoolYear, classBookId },
            this.Includes,
            ct);
    }

    public async Task<ClassBook[]> FindAllByIdsAsync(
        int schoolYear,
        int[] classBookIds,
        CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            cb =>
                cb.SchoolYear == schoolYear
                && this.DbContext.MakeIdsQuery(classBookIds)
                    .Any(id => cb.ClassBookId == id.Id),
            ct)
        ).ToArray();
    }

    public async Task<ClassBook[]> FindAllByClassIdsAsync(
        int schoolYear,
        int[] classIds,
        CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            cb =>
                cb.SchoolYear == schoolYear
                && this.DbContext.MakeIdsQuery(classIds)
                    .Any(id => cb.ClassId == id.Id),
            ct)
        ).ToArray();
    }

    public async Task AddAsync(ClassBookPrint entity, CancellationToken ct)
    {
        await this.DbContext.Set<ClassBookPrint>().AddAsync(entity, ct);
    }

    public async Task AddAsync(ClassBookStudentPrint entity, CancellationToken ct)
    {
        await this.DbContext.Set<ClassBookStudentPrint>().AddAsync(entity, ct);
    }
}

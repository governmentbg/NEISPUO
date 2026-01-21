namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IStudentClassQueryRepository;

internal class StudentClassQueryRepository : Repository, IStudentClassQueryRepository
{
    public StudentClassQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<StudentClass[]> FindAllByClassBookAndPersonIdAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId &&
                cb.IsValid
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        return await (
            from sc in this.StudentClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            where sc.PersonId == personId

            select sc
        ).ToArrayAsync(ct);
    }

    public async Task<FindAllByClassBookVO[]> FindAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId &&
                cb.IsValid
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        return await this.FindAllByClassBookAsync(schoolYear, classBook.ClassId, classBook.ClassIsLvl2, ct);
    }

    public async Task<FindAllByClassBookVO[]> FindAllByClassBookAsync(
        int schoolYear,
        int classId,
        bool classIsLvl2,
        CancellationToken ct)
    {
        return await (
            from sc in this.StudentClassForClassBook(schoolYear, classId, classIsLvl2)

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            select new FindAllByClassBookVO(
                sc,
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName)
        ).ToArrayAsync(ct);
    }

    public async Task<int[]> GetPersonIdsWithAbsencesAttendancesAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Absence>()
            .Where(a => a.SchoolYear == schoolYear && a.ClassBookId == classBookId)
            .Select(a => a.PersonId)
            .Union(
                this.DbContext.Set<Attendance>()
                .Where(a => a.SchoolYear == schoolYear && a.ClassBookId == classBookId)
                .Select(a => a.PersonId)
            )
            .Distinct()
            .ToArrayAsync(ct);
    }

    private IQueryable<StudentClass> StudentClassForClassBook(int schoolYear, int classId, bool classIsLvl2)
    {
        if (classIsLvl2)
        {
            return (
                from sc in this.DbContext.Set<StudentClass>().Where(sc => sc.IsNotPresentForm == false)

                where sc.SchoolYear == schoolYear && sc.ClassId == classId

                select sc
            );
        }
        else
        {
            return (
                from cg in this.DbContext.Set<ClassGroup>()

                join sc in this.DbContext.Set<StudentClass>().Where(sc => sc.IsNotPresentForm == false)
                on new { cg.SchoolYear, cg.ClassId } equals new { sc.SchoolYear, sc.ClassId }

                where cg.SchoolYear == schoolYear && cg.ParentClassId == classId

                select sc
            );
        }
    }
}

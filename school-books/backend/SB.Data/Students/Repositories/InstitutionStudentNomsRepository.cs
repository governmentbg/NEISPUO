namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Data.IInstitutionStudentNomsRepository;

internal class InstitutionStudentNomsRepository : Repository, IInstitutionStudentNomsRepository
{
    private const int LastGradeBasicClassId = 12;

    public InstitutionStudentNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<InstitutionStudentNomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        InstitutionStudentNomVOStudent[] ids,
        CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        var personIds = ids.Select(id => id.PersonId).ToArray();

        return await (
            from sc in this.DbContext.Set<StudentClass>()

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            join cg in this.DbContext.Set<ClassGroup>() on sc.ClassId equals cg.ClassId

            join cgp in this.DbContext.Set<ClassGroup>() on cg.ParentClassId equals cgp.ClassId
            into g1 from cgp in g1.DefaultIfEmpty()

            where sc.SchoolYear == schoolYear &&
                sc.InstitutionId == instId &&
                this.DbContext.MakeIdsQuery(personIds).Any(id => p.PersonId == id.Id)

            orderby p.FirstName, p.MiddleName, p.LastName

            select new InstitutionStudentNomVO(
                new InstitutionStudentNomVOStudent(
                    cgp != null ? cgp.ClassId : cg.ClassId,
                    sc.PersonId,
                    p.FirstName,
                    p.LastName),
                $"{StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)} {cgp.ClassName ?? cg.ClassName}"
            ))
            .ToArrayAsync(ct);
    }

    public async Task<InstitutionStudentNomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        string? term,
        int? offset,
        int? limit,
        bool? showOnlyLastGrade,
        CancellationToken ct)
    {
        var studentClassPredicate = PredicateBuilder.True<StudentClass>();

        var personPredicate = PredicateBuilder.True<Person>();

        var classGroupPredicate = PredicateBuilder.True<ClassGroup>();

        if (showOnlyLastGrade == true)
        {
            studentClassPredicate = studentClassPredicate.And(sc => sc.BasicClassId == LastGradeBasicClassId);
            classGroupPredicate = classGroupPredicate.And(c => c.BasicClassId == LastGradeBasicClassId || c.IsNotPresentForm == true);
        }

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                personPredicate = personPredicate.AndAnyStringContains(p => p.FirstName, p => p.MiddleName, p => p.LastName, word);
            }
        }

        return await (
            from sc in this.DbContext.Set<StudentClass>().Where(studentClassPredicate)

            join p in this.DbContext.Set<Person>().Where(personPredicate) on sc.PersonId equals p.PersonId

            join cg in this.DbContext.Set<ClassGroup>().Where(classGroupPredicate) on sc.ClassId equals cg.ClassId

            join cgp in this.DbContext.Set<ClassGroup>() on cg.ParentClassId equals cgp.ClassId
            into g1 from cgp in g1.DefaultIfEmpty()

            where sc.SchoolYear == schoolYear &&
                sc.InstitutionId == instId &&
                sc.Status == StudentClassStatus.Enrolled

            orderby p.FirstName, p.MiddleName, p.LastName

            select new InstitutionStudentNomVO(
                new InstitutionStudentNomVOStudent(
                    cgp != null ? cgp.ClassId : cg.ClassId,
                    sc.PersonId,
                    p.FirstName,
                    p.LastName),
                $"{StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName)} {cgp.ClassName ?? cg.ClassName}"
            ))
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);
    }
}

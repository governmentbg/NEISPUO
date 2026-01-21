namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Data.ClassBooksQueryHelper;
using static SB.Data.IClassBookStudentNomsRepository;

internal class ClassBookStudentNomsRepository : Repository, IClassBookStudentNomsRepository
{
    public ClassBookStudentNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<ClassBookStudentNomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        int classBookId,
        bool showOnlyWithIndividualCurriculum,
        bool showOnlyWithIndividualCurriculumSchedule,
        int[] ids,
        CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.IsValid &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        return await (
            from p in this.DbContext.Set<StudentsWithClassBookDataView>()

            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            on p.PersonId equals sc.PersonId
            into j1 from sc in j1.DefaultIfEmpty()

            where
                p.SchoolYear == schoolYear &&
                p.InstId == instId &&
                p.ClassBookId == classBookId &&
                this.DbContext.MakeIdsQuery(ids).Any(id => p.PersonId == id.Id)

            orderby p.FirstName, p.MiddleName, p.LastName

            select new ClassBookStudentNomVO(
                p.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                #pragma warning disable CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
                sc.PersonId == null ? "ИЗТРИТ" :
                #pragma warning restore CS0472
                sc.IsTransferred ? "ОТПИСАН" :
                null
            ))
            .ToArrayAsync(ct);
    }

    public async Task<ClassBookStudentNomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        string? term,
        int classBookId,
        bool showOnlyWithIndividualCurriculum,
        bool showOnlyWithIndividualCurriculumSchedule,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Person>();

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndAnyStringContains(p => p.FirstName, p => p.MiddleName, p => p.LastName, word);
            }
        }

        var studentClassPredicate = PredicateBuilder.True<StudentsForClassBookVO>();

        if (showOnlyWithIndividualCurriculum)
        {
            studentClassPredicate = studentClassPredicate.And(sc => sc.IsIndividualCurriculum == true);
        }

        if (showOnlyWithIndividualCurriculumSchedule)
        {
            studentClassPredicate = studentClassPredicate.And(sc =>
                this.DbContext
                    .Set<Schedule>()
                    .Any(s => s.SchoolYear == schoolYear &&
                              s.ClassBookId == classBookId &&
                              s.IsIndividualSchedule &&
                              s.PersonId == sc.PersonId));
        }

        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var students =
            this.DbContext.StudentsForClassBook(
                schoolYear,
                classBook.ClassId,
                classBook.ClassIsLvl2);

        return await (
            from sc in students.Where(studentClassPredicate)

            join p in this.DbContext.Set<Person>().Where(predicate) on sc.PersonId equals p.PersonId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new ClassBookStudentNomVO(
                sc.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                sc.IsTransferred ? "ОТПИСАН" : null
            ))
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);
    }
}

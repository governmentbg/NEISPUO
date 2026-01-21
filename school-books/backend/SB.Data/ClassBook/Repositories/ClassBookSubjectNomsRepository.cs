namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ClassBookSubjectNomsRepository : Repository, IClassBookSubjectNomsRepository
{
    public ClassBookSubjectNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        int classBookId,
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
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var curriculums = await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            where this.DbContext.MakeIdsQuery(ids).Any(id => s.SubjectId == id.Id)

            select new
            {
                s.SubjectId,
                s.SubjectName,
                IsRemoved = !cc.IsValid || !c.IsValid
            }
        ).ToArrayAsync(ct);

        return curriculums
            .GroupBy(c => new { c.SubjectId, c.SubjectName })
            .Select(s =>
            {
                var badge = s.All(c => c.IsRemoved) ? "ИЗТРИТ" : null;

                return new NomVO(
                    s.Key.SubjectId,
                    s.Key.SubjectName,
                    badge);
            })
            .OrderBy(s => s.Name)
            .ToArray();
    }

    public async Task<NomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? writeAccessCurriculumTeacherPersonId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Subject>();

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(s => s.SubjectName, word);
            }
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

        var curriculums =
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>()
            on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>().Where(predicate)
            on c.SubjectId equals s.SubjectId

            where cc.IsValid &&
                c.IsValid

            select new
            {
                s.SubjectId,
                s.SubjectName,
                c.CurriculumId,
                c.CurriculumPartID,
                c.TotalTermHours,
            };

        if (writeAccessCurriculumTeacherPersonId != null)
        {
            curriculums =
                from c in curriculums

                join t in this.DbContext.Set<CurriculumTeacher>()
                on c.CurriculumId equals t.CurriculumId

                join sp in this.DbContext.Set<StaffPosition>()
                on t.StaffPositionId equals sp.StaffPositionId

                where sp.PersonId == writeAccessCurriculumTeacherPersonId &&
                    t.IsValid

                select c;
        }

        return
            curriculums
            .GroupBy(c => new { c.SubjectId, c.SubjectName })
            .Select(s => new { s.Key.SubjectId, s.Key.SubjectName })
            .OrderBy(c => c.SubjectName)
            .WithOffsetAndLimit(offset, limit)
            .Select(s => new NomVO(s.SubjectId, s.SubjectName))
            .ToArray();
    }
}

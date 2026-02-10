namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Data.IInstitutionCurriculumNomsRepository;

internal class InstitutionCurriculumNomsRepository : Repository, IInstitutionCurriculumNomsRepository
{
    public InstitutionCurriculumNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<InstitutionCurriculumNomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        InstitutionCurriculumNomVOCurriculum[] ids,
        CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        var aloneSubjectIds = ids.Where(id => id.SubjectTypeId == SubjectType.DefaultSubjectTypeId).Select(id => id.SubjectId).ToArray();
        var curriculumTupleIds = ids.Where(id => id.SubjectTypeId != SubjectType.DefaultSubjectTypeId).Select(s => (s.SubjectId, s.SubjectTypeId)).ToArray();

        var aloneSubjects =
            from s in this.DbContext.Set<Subject>()

            where this.DbContext.MakeIdsQuery(aloneSubjectIds).Any(id => s.SubjectId == id.Id)

            orderby s.SubjectName

            select new
            {
                s.SubjectId,
                SubjectTypeId = SubjectType.DefaultSubjectTypeId,
                s.SubjectName
            };

        var institutionCurriculums = (
            from c in this.DbContext.Set<Curriculum>()

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where c.SchoolYear == schoolYear &&
                c.InstitutionId == instId &&
                this.DbContext.MakeIdsQuery(curriculumTupleIds).Any(id => s.SubjectId == id.Id1 && st.SubjectTypeId == id.Id2)

            orderby s.SubjectName, st.Name

            select new
            {
                s.SubjectId,
                st.SubjectTypeId,
                SubjectName = s.SubjectName + " / " + st.Name
            }).Distinct();

        return await aloneSubjects
            .Concat(institutionCurriculums)
            .Select(s => new InstitutionCurriculumNomVO(
                new InstitutionCurriculumNomVOCurriculum(
                    s.SubjectId,
                    s.SubjectTypeId),
                s.SubjectName))
            .ToArrayAsync(ct);
    }

    public async Task<InstitutionCurriculumNomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True(new
        {
            SubjectId = default(int),
            SubjectTypeId = default(int),
            SubjectName = default(string)!
        });

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(s => s.SubjectName, word);
            }
        }

        var institutionCurriculums = (
            from c in this.DbContext.Set<Curriculum>()

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where c.SchoolYear == schoolYear &&
                c.InstitutionId == instId

            select new
            {
                s.SubjectId,
                st.SubjectTypeId,
                SubjectName = s.SubjectName + " / " + st.Name
            }).Distinct();

        var customCurriculums = (
            from c in this.DbContext.Set<CustomVarValue>()

            join s in this.DbContext.Set<Subject>().Where(s => s.IsValid == true) on c.CustomVarVal equals s.SubjectId


            where c.InstitutionId == instId && s.IsValid == true && c.IsValid == true

            select new
            {
                s.SubjectId,
                SubjectTypeId = SubjectType.DefaultSubjectTypeId,
                s.SubjectName
            }).Distinct();

        var institutionCurriculumSubjectIds = institutionCurriculums.Select(c => c.SubjectId).Distinct().ToArray();

        var aloneSubjects =
            from s in this.DbContext.Set<Subject>()
            where s.SubjectId <= Subject.LastInternalSubjectId && this.DbContext.MakeIdsQuery(institutionCurriculumSubjectIds).Any(id => s.SubjectId == id.Id)
            select new
            {
                s.SubjectId,
                SubjectTypeId = SubjectType.DefaultSubjectTypeId,
                s.SubjectName
            };

        var allSubjects =
            await aloneSubjects
                .Union(institutionCurriculums)
                .Union(customCurriculums)
                .Where(predicate)
                .OrderBy(s => s.SubjectName)
                .ToArrayAsync(ct);

        return allSubjects
            .Select(s => new InstitutionCurriculumNomVO(
                new InstitutionCurriculumNomVOCurriculum(
                    s.SubjectId,
                    s.SubjectTypeId),
                s.SubjectName))
            .WithOffsetAndLimit(offset, limit)
            .ToArray();
    }
}

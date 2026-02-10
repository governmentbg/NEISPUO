namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IPgResultsQueryRepository;

internal class PgResultsQueryRepository : Repository, IPgResultsQueryRepository
{
    public PgResultsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllForClassBookVO[]> GetAllForClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await
            (from pgr in this.DbContext.Set<PgResult>()
             where pgr.SchoolYear == schoolYear && pgr.ClassBookId == classBookId
             select pgr.PersonId)
             .GroupBy(g => g)
             .Select(r => new GetAllForClassBookVO(
                 r.Key,
                 r.Count()))
             .ToArrayAsync(ct);
    }

    public async Task<GetAllForStudentVO[]> GetAllForStudentAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        CancellationToken ct)
    {
        return await (
            from pgr in this.DbContext.Set<PgResult>()

            join csu in this.DbContext.Set<SysUser>() on pgr.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join msu in this.DbContext.Set<SysUser>() on pgr.ModifiedBySysUserId equals msu.SysUserId

            join mp in this.DbContext.Set<Person>() on msu.PersonId equals mp.PersonId

            join s in this.DbContext.Set<Subject>()
            on pgr.SubjectId equals s.SubjectId
            into j2 from s in j2.DefaultIfEmpty()

            where pgr.SchoolYear == schoolYear &&
                pgr.ClassBookId == classBookId &&
                pgr.PersonId == studentPersonId

            orderby pgr.CreateDate

            select new GetAllForStudentVO(
                pgr.PgResultId,
                pgr.SubjectId,
                pgr.PersonId,
                s.SubjectName,
                s.SubjectNameShort,
                pgr.StartSchoolYearResult,
                pgr.EndSchoolYearResult,
                pgr.CreateDate,
                pgr.CreatedBySysUserId,
                cp.FirstName,
                cp.MiddleName,
                cp.LastName,
                pgr.ModifyDate,
                pgr.ModifiedBySysUserId,
                mp.FirstName,
                mp.MiddleName,
                mp.LastName
        )).ToArrayAsync(ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int pgResultId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<PgResult>()
            .Where(pgr =>
                pgr.SchoolYear == schoolYear &&
                pgr.ClassBookId == classBookId &&
                pgr.PgResultId == pgResultId)
            .Select(pgr => new GetVO(
                pgr.PgResultId,
                pgr.PersonId,
                pgr.SubjectId,
                pgr.StartSchoolYearResult,
                pgr.EndSchoolYearResult))
            .SingleAsync(ct);
    }

    public async Task<bool> ExistsForPersonAndSubjectAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? subjectId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<PgResult>()
            .Where(pgr =>
                pgr.SchoolYear == schoolYear &&
                pgr.ClassBookId == classBookId &&
                pgr.PersonId == personId &&
                pgr.SubjectId == subjectId)
            .AnyAsync(ct);
    }

    public async Task<int> GetSubjectIdForCurriculumAsync(int curriculumId, CancellationToken ct)
    {
        return await this.DbContext.Set<Curriculum>()
            .Where(s => s.CurriculumId == curriculumId)
            .Select(s => s.SubjectId)
            .SingleAsync(ct) ?? throw new Exception($"The curriculum {curriculumId} does not have a subject.");
    }
}

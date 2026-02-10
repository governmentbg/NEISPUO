namespace SB.Data;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IGradeResultsQueryRepository;

internal class GradeResultsQueryRepository : Repository, IGradeResultsQueryRepository
{
    public GradeResultsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllVO[]> GetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var studentRetakeExams = (await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
                on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }

            join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            select new
            {
                gr.PersonId,
                RetakeExamSubject = string.Join(" / ", s.SubjectName, st.Name)
            })
            .ToArrayAsync(ct))
            .ToLookup(grs => grs.PersonId, grs => grs.RetakeExamSubject);

        return await (
            from gr in this.DbContext.Set<GradeResult>()

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            select new GetAllVO(
                gr.PersonId,
                gr.InitialResultType,
                string.Join(", ", studentRetakeExams[gr.PersonId]),
                gr.FinalResultType
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetAllEditVO[]> GetAllEditAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var studentRetakeExams = (await (
           from gr in this.DbContext.Set<GradeResult>()

           join grs in this.DbContext.Set<GradeResultSubject>()
               on new { gr.SchoolYear, gr.GradeResultId }
               equals new { grs.SchoolYear, grs.GradeResultId }

           where gr.SchoolYear == schoolYear &&
               gr.ClassBookId == classBookId

           select new
           {
               gr.PersonId,
               grs.CurriculumId,
           })
           .ToArrayAsync(ct))
           .ToLookup(re => re.PersonId, re => re.CurriculumId);

        return await (
            from gr in this.DbContext.Set<GradeResult>()

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            select new GetAllEditVO(
                gr.PersonId,
                gr.InitialResultType,
                studentRetakeExams[gr.PersonId].ToArray(),
                gr.FinalResultType
            ))
            .ToArrayAsync(ct);
    }

    public async Task<bool> HasRemovedFilledSessionAsync(
       int schoolYear,
       int classBookId,
       (int personId, int[] retakeExamCurriculumIds)[] classGradeResults,
       CancellationToken ct)
    {
        var gradeResultCurriculums =
            classGradeResults
            .SelectMany(gr =>
                gr.retakeExamCurriculumIds
                .Select(curriculumId => (gr.personId, curriculumId)))
            .ToArray();

        var removedSessions = await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
                on new { gr.SchoolYear, gr.GradeResultId }
                equals new { grs.SchoolYear, grs.GradeResultId }

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId &&
                !(this.DbContext.MakeIdsQuery(gradeResultCurriculums).Any(
                    id =>
                        gr.PersonId == id.Id1 &&
                        grs.CurriculumId == id.Id2))

            select grs
        ).ToArrayAsync(ct);

        return removedSessions.Any(s => s.IsFilled);
    }

    public async Task<GetSessionAllVO[]> GetSessionAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return (await this.GetSessionAllEditAsync(schoolYear, classBookId, ct))
            .Select(s => new GetSessionAllVO(
                s.PersonId,
                s.ClassNumber,
                s.FirstName,
                s.MiddleName,
                s.LastName,
                s.IsTransferred,
                s.IsRemoved,
                s.Curriculum,
                s.Session1NoShow,
                GradeUtils.GetDecimalGradeText(s.Session1Grade),
                s.Session2NoShow,
                GradeUtils.GetDecimalGradeText(s.Session2Grade),
                s.Session3NoShow,
                GradeUtils.GetDecimalGradeText(s.Session3Grade)
                ))
            .ToArray();
    }

    public async Task<GetSessionAllEditVO[]> GetSessionAllEditAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        return await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
                on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }

            join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join p in this.DbContext.Set<Person>() on gr.PersonId equals p.PersonId

            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            on gr.PersonId equals sc.PersonId
            into j1 from sc in j1.DefaultIfEmpty()

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            orderby sc.ClassNumber

            select new GetSessionAllEditVO(
                gr.PersonId,
                grs.CurriculumId,
                sc.ClassNumber,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                ((bool?)sc.IsTransferred) ?? false,
                #pragma warning disable CS0472 // The result of the expression is always 'false' since a value of type 'int' is never equal to 'null' of type 'int?'
                sc.PersonId == null,
                #pragma warning restore CS0472
                string.Join(" / ", s.SubjectName, st.Name),
                grs.Session1NoShow,
                grs.Session1Grade,
                grs.Session2NoShow,
                grs.Session2Grade,
                grs.Session3NoShow,
                grs.Session3Grade
            ))
            .ToArrayAsync(ct);
    }
}

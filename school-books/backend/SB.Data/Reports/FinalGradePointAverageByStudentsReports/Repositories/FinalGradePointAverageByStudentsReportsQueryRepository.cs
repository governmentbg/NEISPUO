namespace SB.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IFinalGradePointAverageByStudentsReportsQueryRepository;

class FinalGradePointAverageByStudentsReportsQueryRepository : Repository, IFinalGradePointAverageByStudentsReportsQueryRepository
{
    public FinalGradePointAverageByStudentsReportsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<FinalGradePointAverageByStudentsReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SchoolYear,
                r.FinalGradePointAverageByStudentsReportId,
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.MinimumGradePointAverage,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<FinalGradePointAverageByStudentsReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsReportId

            select new GetVO(
                r.SchoolYear,
                r.FinalGradePointAverageByStudentsReportId,
                r.Period,
                r.ClassBookNames,
                r.MinimumGradePointAverage,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<FinalGradePointAverageByStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsReportId

            orderby ri.FinalGradePointAverageByStudentsReportItemId

            select new GetItemsVO(
                ri.ClassBookName,
                ri.StudentNames,
                ri.IsTransferred,
                ri.FinalGradePointAverage)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        ReportPeriod period,
        int[] classBookIds,
        decimal? minimalGradePointAverage,
        CancellationToken ct)
    {
        var sql = $"""
SELECT
    o.FullBookName AS ClassBookName,
    [school_books].fn_join_names2(o.FirstName, o.LastName) AS StudentName,
    o.IsTransferred,
    AVG(o.Grade) AS FinalGradePointAverage
FROM (
    SELECT
        p.PersonID,
        p.FirstName,
        p.LastName,
        IIF(sc.Status = {(int)StudentClassStatus.Transferred}, CAST(1 AS BIT), CAST(0 AS BIT)) AS IsTransferred,
        cb.ClassBookId,
        cb.BasicClassId,
        cb.FullBookName,
        COALESCE(g.QualitativeGrade, CASE WHEN g.DecimalGrade < 3.00 THEN 2.00 ELSE ROUND(g.DecimalGrade, 0) END) AS Grade
    FROM
        [school_books].[Grade] g
        INNER JOIN [school_books].[ClassBook] cb ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId
		INNER JOIN [inst_year].[ClassGroup] cg ON (CASE WHEN cg.IsNotPresentForm = 1 THEN cg.ClassId ELSE cg.ParentClassId END) = cb.classId AND cg.SchoolYear = cb.SchoolYear AND cg.InstitutionID = cb.InstId
        INNER JOIN [student].[StudentClass] sc ON sc.SchoolYear = cb.SchoolYear AND sc.InstitutionID = cb.InstId AND sc.ClassId = cg.ClassID AND sc.PersonId = g.PersonId
        INNER JOIN [core].[Person] p ON p.PersonID = g.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON c.SchoolYear = g.SchoolYear AND c.CurriculumID = g.CurriculumId
    WHERE
        c.IsValid = 1 AND
        (((@period = {(int)ReportPeriod.TermOne} AND g.Term = 1 AND g.Type = {(int)GradeType.Term}) OR
         (@period = {(int)ReportPeriod.TermTwo} AND g.Term = 2 AND g.Type = {(int)GradeType.Term}) OR
         (@period = {(int)ReportPeriod.WholeYear} AND g.Type = {(int)GradeType.Final}))) AND
        cb.InstId = @InstId AND
        cb.SchoolYear = @SchoolYear AND
        {(classBookIds.Length > 0 ? "g.ClassBookId IN (" + string.Join(',', classBookIds) + ")" : "1 = 1")}
) o
GROUP BY
    o.ClassBookId,
    o.BasicClassId,
    o.FullBookName,
    o.PersonID,
    o.FirstName,
    o.LastName,
    o.IsTransferred
HAVING
    {(minimalGradePointAverage != null ? "AVG(Grade) >= " + minimalGradePointAverage.Value.ToString(CultureInfo.InvariantCulture) : "1 = 1")}
ORDER BY
    ClassBookId,
    BasicClassId,
    FullBookName,
    FirstName,
    LastName
""";

        var results = await this.DbContext.Set<FinalGradesByStudentsQO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("InstId", instId),
                new SqlParameter("period", (int)period))
            .ToArrayAsync(ct);

        if (results.Length == 0)
        {
            return Array.Empty<GetItemsForAddVO>();
        }

        return results.Select(r =>
                new GetItemsForAddVO(
                    r.ClassBookName,
                    r.StudentName,
                    r.IsTransferred,
                    r.FinalGradePointAverage))

        .ToArray();
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from abc in this.DbContext.Set<FinalGradePointAverageByStudentsReport>()
            where abc.SchoolYear == schoolYear &&
                abc.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsReportId
            select abc.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct)
    {
        var items = await (
            from ri in this.DbContext.Set<FinalGradePointAverageByStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsReportId

            orderby ri.FinalGradePointAverageByStudentsReportItemId

            select new GetExcelDataVOItem
            (
                ri.ClassBookName,
                ri.StudentNames,
                ri.IsTransferred,
                ri.FinalGradePointAverage
            )
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<FinalGradePointAverageByStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.FinalGradePointAverageByStudentsReportId == finalGradePointAverageByStudentsReportId

            select new GetExcelDataVO
            (
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.MinimumGradePointAverage,
                r.CreateDate,
                items
            )
        ).SingleAsync(ct);
    }
}

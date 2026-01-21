namespace SB.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ITopicPlansQueryRepository;

internal class TopicPlansQueryRepository : Repository, ITopicPlansQueryRepository
{
    public TopicPlansQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int sysUserId,
        string? name,
        string? basicClassName,
        string? subjectName,
        string? subjectTypeName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var query = (
            from tp in this.DbContext.Set<TopicPlan>()

            join bc in this.DbContext.Set<BasicClass>() on tp.BasicClassId equals bc.BasicClassId
            into j1
            from bc in j1.DefaultIfEmpty()

            join s in this.DbContext.Set<Subject>() on tp.SubjectId equals s.SubjectId
            into j2
            from s in j2.DefaultIfEmpty()

            join st in this.DbContext.Set<SubjectType>() on tp.SubjectTypeId equals st.SubjectTypeId
            into j3
            from st in j3.DefaultIfEmpty()

            where tp.CreatedBySysUserId == sysUserId

            orderby tp.BasicClassId, tp.CreateDate

            select new
            {
                tp.TopicPlanId,
                tp.Name,
                BasicClassName = bc.Name,
                SubjectName = s.SubjectName,
                SubjectTypeName = st.Name
            });

        var predicate = PredicateBuilder.True(query);
        if (!string.IsNullOrWhiteSpace(name))
        {
            string[] words = name.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(tp => tp.Name, word);
            }
        }

        if (!string.IsNullOrWhiteSpace(basicClassName))
        {
            string[] words = basicClassName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(bc => bc.BasicClassName, word);
            }
        }

        if (!string.IsNullOrWhiteSpace(subjectName))
        {
            string[] words = subjectName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(s => s.SubjectName, word);
            }
        }

        if (!string.IsNullOrWhiteSpace(subjectTypeName))
        {
            string[] words = subjectTypeName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(st => st.SubjectTypeName, word);
            }
        }

        return await
            query
            .Where(predicate)
            .Select(q => new GetAllVO(
                q.TopicPlanId,
                q.Name,
                q.BasicClassName,
                q.SubjectName,
                q.SubjectTypeName))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int sysUserId,
        int topicPlanId,
        CancellationToken ct)
    {
        return await (
            from tp in this.DbContext.Set<TopicPlan>()

            where tp.TopicPlanId == topicPlanId && tp.CreatedBySysUserId == sysUserId

            select new GetVO(
                tp.TopicPlanId,
                tp.Name,
                tp.BasicClassId,
                tp.SubjectId,
                tp.SubjectTypeId,
                tp.TopicPlanPublisherId,
                tp.TopicPlanPublisherOther))
            .SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int sysUserId,
        int topicPlanId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<TopicPlanItem>()
            join tp in this.DbContext.Set<TopicPlan>() on tpi.TopicPlanId equals tp.TopicPlanId

            where tpi.TopicPlanId == topicPlanId && tp.CreatedBySysUserId == sysUserId

            orderby tpi.Number, tpi.CreateDate

            select new GetItemsVO(
                tpi.TopicPlanItemId,
                tpi.Number,
                tpi.Title,
                tpi.Note))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemVO> GetItemAsync(
        int sysUserId,
        int topicPlanItemId,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<TopicPlanItem>()
            join tp in this.DbContext.Set<TopicPlan>() on tpi.TopicPlanId equals tp.TopicPlanId

            where tpi.TopicPlanItemId == topicPlanItemId && tp.CreatedBySysUserId == sysUserId

            select new GetItemVO(
                tpi.Number,
                tpi.Title,
                tpi.Note))
            .SingleAsync(ct);
    }

    public async Task<GetExcelDataVO[]> GetExcelDataAsync(
        int sysUserId,
        int topicPlanId,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<TopicPlanItem>()
            join tp in this.DbContext.Set<TopicPlan>() on tpi.TopicPlanId equals tp.TopicPlanId

            where tpi.TopicPlanId == topicPlanId && tp.CreatedBySysUserId == sysUserId

            orderby tpi.Number, tpi.CreateDate

            select new GetExcelDataVO(
                tpi.Number,
                tpi.Title,
                tpi.Note))
            .ToArrayAsync(ct);
    }

    public async Task RemoveTopicPlanAsync(
        int topicPlanId,
        CancellationToken ct)
    {
        string sql = """
            DELETE FROM [school_books].[TopicPlanItem]
            WHERE
                [TopicPlanId] = @topicPlanId

            DELETE FROM [school_books].[TopicPlan]
            WHERE
                [TopicPlanId] = @topicPlanId
            """;

        await this.DbContext.Database
            .ExecuteSqlRawAsync(
                sql,
                new[]
                {
                    new SqlParameter("topicPlanId", topicPlanId)
                },
                ct);
    }
}

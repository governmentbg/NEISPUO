namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.ISkillsCheckExamResultProtocolsQueryRepository;
using SB.Common;
using System.Threading;

internal class SkillsCheckExamResultProtocolsQueryRepository : Repository, ISkillsCheckExamResultProtocolsQueryRepository
{
    public SkillsCheckExamResultProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int skillsCheckExamResultProtocolId,
        CancellationToken ct)
    {
        return await (
            from sdp in this.DbContext.Set<SkillsCheckExamResultProtocol>()

            where sdp.SchoolYear == schoolYear && sdp.SkillsCheckExamResultProtocolId == skillsCheckExamResultProtocolId

            select new GetVO(
                sdp.SchoolYear,
                sdp.SkillsCheckExamResultProtocolId,
                sdp.ProtocolNumber,
                sdp.SubjectId,
                sdp.Date,
                sdp.StudentsCapacity)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetEvaluatorAllVO>> GetEvaluatorAllAsync(
        int schoolYear,
        int skillsCheckExamResultProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<SkillsCheckExamResultProtocolEvaluator>()

            where p.SchoolYear == schoolYear &&
                p.SkillsCheckExamResultProtocolId == skillsCheckExamResultProtocolId

            orderby p.Name

            select new GetEvaluatorAllVO(
                p.SkillsCheckExamResultProtocolEvaluatorId,
                p.Name
            ))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetEvaluatorVO> GetEvaluatorAsync(
        int schoolYear,
        int skillsCheckExamResultProtocolId,
        int skillsCheckExamResultProtocolEvaluatorId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<SkillsCheckExamResultProtocolEvaluator>()
            .Where(p => p.SchoolYear == schoolYear &&
                p.SkillsCheckExamResultProtocolId == skillsCheckExamResultProtocolId &&
                p.SkillsCheckExamResultProtocolEvaluatorId == skillsCheckExamResultProtocolEvaluatorId)
            .Select(p => new GetEvaluatorVO(
                p.SkillsCheckExamResultProtocolEvaluatorId,
                p.Name
            ))
            .SingleAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int skillsCheckExamResultProtocolId,
       CancellationToken ct)
    {
        var evaluators = await (
            from pe in this.DbContext.Set<SkillsCheckExamResultProtocolEvaluator>()

            where pe.SchoolYear == schoolYear &&
                pe.SkillsCheckExamResultProtocolId == skillsCheckExamResultProtocolId
            orderby pe.Name

            select new GetWordDataVOEvaluator(pe.Name)
        ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<SkillsCheckExamResultProtocol>()
            join s in this.DbContext.Set<Subject>() on edp.SubjectId equals s.SubjectId
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { edp.InstId, edp.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }

            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId
            into g2
            from t in g2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into g3
            from m in g3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into g4
            from r in g4.DefaultIfEmpty()

            where edp.SchoolYear == schoolYear && edp.SkillsCheckExamResultProtocolId == skillsCheckExamResultProtocolId

            select new GetWordDataVO(
                edp.SchoolYear + " / " + (edp.SchoolYear + 1),
                i.Name,
                !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                edp.ProtocolNumber,
                s.SubjectName,
                edp.Date != null ? edp.Date!.Value.ToString("dd.MM.yyyy") : null,
                evaluators,
                edp.StudentsCapacity)
         ).SingleAsync(ct);
    }
}

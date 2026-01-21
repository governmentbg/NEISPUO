namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.ISkillsCheckExamDutyProtocolsQueryRepository;
using SB.Common;
using System.Threading;

internal class SkillsCheckExamDutyProtocolsQueryRepository : Repository, ISkillsCheckExamDutyProtocolsQueryRepository
{
    public SkillsCheckExamDutyProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int skillsCheckExamDutyProtocolId,
        CancellationToken ct)
    {
        var supervisorIds = await (
            from aps in this.DbContext.Set<SkillsCheckExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on aps.PersonId equals p.PersonId
            where aps.SchoolYear == schoolYear && aps.SkillsCheckExamDutyProtocolId == skillsCheckExamDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select aps.PersonId
         ).ToArrayAsync(ct);

        return await (
            from sdp in this.DbContext.Set<SkillsCheckExamDutyProtocol>()

            where sdp.SchoolYear == schoolYear && sdp.SkillsCheckExamDutyProtocolId == skillsCheckExamDutyProtocolId

            select new GetVO(
                sdp.SchoolYear,
                sdp.SkillsCheckExamDutyProtocolId,
                sdp.ProtocolNumber,
                sdp.ProtocolDate,
                sdp.SubjectId,
                sdp.SubjectTypeId,
                sdp.Date,
                sdp.DirectorPersonId,
                sdp.StudentsCapacity,
                supervisorIds)
         ).SingleAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int skillsCheckExamDutyProtocolId,
       CancellationToken ct)
    {
        var supervisors = await (
            from edps in this.DbContext.Set<SkillsCheckExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            where edps.SchoolYear == schoolYear && edps.SkillsCheckExamDutyProtocolId == skillsCheckExamDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select new GetWordDataVOSupervisor(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName))
        ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<SkillsCheckExamDutyProtocol>()
            join s in this.DbContext.Set<Subject>() on edp.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on edp.SubjectTypeId equals st.SubjectTypeId
            join p in this.DbContext.Set<Person>() on edp.DirectorPersonId equals p.PersonId
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

            where edp.SchoolYear == schoolYear && edp.SkillsCheckExamDutyProtocolId == skillsCheckExamDutyProtocolId

            select new GetWordDataVO(
                edp.SchoolYear + " / " + (edp.SchoolYear + 1),
                i.Name,
                !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                edp.ProtocolNumber,
                edp.ProtocolDate != null ? edp.ProtocolDate!.Value.ToString("dd.MM.yyyy") : null,
                s.SubjectName + ' ' + st.Name,
                edp.Date.ToString("dd.MM.yyyy"),
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                edp.StudentsCapacity,
                supervisors)
         ).SingleAsync(ct);
    }
}

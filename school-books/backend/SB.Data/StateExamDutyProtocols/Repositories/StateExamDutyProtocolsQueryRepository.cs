namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IStateExamDutyProtocolsQueryRepository;
using SB.Common;
using System.Threading;

internal class StateExamDutyProtocolsQueryRepository : Repository, IStateExamDutyProtocolsQueryRepository
{
    public StateExamDutyProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int stateExamDutyProtocolId,
        CancellationToken ct)
    {
        var supervisorIds = await (
            from aps in this.DbContext.Set<StateExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on aps.PersonId equals p.PersonId
            where aps.SchoolYear == schoolYear && aps.StateExamDutyProtocolId == stateExamDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select aps.PersonId
         ).ToArrayAsync(ct);

        return await (
            from sdp in this.DbContext.Set<StateExamDutyProtocol>()

            where sdp.SchoolYear == schoolYear && sdp.StateExamDutyProtocolId == stateExamDutyProtocolId

            select new GetVO(
                sdp.SchoolYear,
                sdp.StateExamDutyProtocolId,
                sdp.ProtocolNumber,
                sdp.ProtocolDate,
                sdp.SessionType,
                sdp.SubjectId,
                sdp.SubjectTypeId,
                sdp.EduFormId,
                sdp.OrderNumber,
                sdp.OrderDate,
                sdp.Date,
                sdp.ModulesCount,
                sdp.RoomNumber,
                supervisorIds)
         ).SingleAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int stateExamDutyProtocolId,
       CancellationToken ct)
    {
        var supervisors = await (
            from edps in this.DbContext.Set<StateExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            where edps.SchoolYear == schoolYear && edps.StateExamDutyProtocolId == stateExamDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select new GetWordDataVOSupervisor(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName))
        ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<StateExamDutyProtocol>()
            join s in this.DbContext.Set<Subject>() on edp.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on edp.SubjectTypeId equals st.SubjectTypeId
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { edp.InstId, edp.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }

            join ef in this.DbContext.Set<EduForm>() on edp.EduFormId equals ef.ClassEduFormId
            into g0
            from ef in g0.DefaultIfEmpty()

            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId
            into g2
            from t in g2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into g3
            from m in g3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into g4
            from r in g4.DefaultIfEmpty()

            where edp.SchoolYear == schoolYear && edp.StateExamDutyProtocolId == stateExamDutyProtocolId

            select new GetWordDataVO(
                edp.SchoolYear + " / " + (edp.SchoolYear + 1),
                i.Name,
                !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                edp.ProtocolNumber,
                edp.ProtocolDate != null ? edp.ProtocolDate!.Value.ToString("dd.MM.yyyy") : null,
                edp.SessionType,
                s.SubjectName + ' ' + st.Name,
                ef.Name,
                edp.OrderNumber,
                edp.OrderDate.ToString("dd.MM.yyyy"),
                edp.Date.ToString("dd.MM.yyyy"),
                edp.ModulesCount,
                edp.RoomNumber,
                supervisors)
         ).SingleAsync(ct);
    }
}

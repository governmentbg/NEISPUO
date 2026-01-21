namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.INvoExamDutyProtocolsQueryRepository;
using SB.Common;
using System.Threading;

internal class NvoExamDutyProtocolsQueryRepository : Repository, INvoExamDutyProtocolsQueryRepository
{
    public NvoExamDutyProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int nvoExamDutyProtocolId,
        CancellationToken ct)
    {
        var supervisorIds = await (
            from edps in this.DbContext.Set<NvoExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            where edps.SchoolYear == schoolYear && edps.NvoExamDutyProtocolId == nvoExamDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select edps.PersonId
         ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<NvoExamDutyProtocol>()

            where edp.SchoolYear == schoolYear && edp.NvoExamDutyProtocolId == nvoExamDutyProtocolId

            select new GetVO(
                edp.SchoolYear,
                edp.NvoExamDutyProtocolId,
                edp.ProtocolNumber,
                edp.ProtocolDate,
                edp.BasicClassId,
                edp.SubjectId,
                edp.SubjectTypeId,
                edp.Date,
                edp.RoomNumber,
                edp.DirectorPersonId,
                supervisorIds)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int nvoExamDutyProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<NvoExamDutyProtocolStudent>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edps.SchoolYear, edps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1
            from cgp in j1.DefaultIfEmpty()

            where edps.SchoolYear == schoolYear && edps.NvoExamDutyProtocolId == nvoExamDutyProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetStudentAllVO(
                cg.ClassId,
                p.PersonId,
                cgp.ClassName ?? cg.ClassName,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName))
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<bool> HasDuplicatedStudentsAsync(
        int schoolYear,
        int examDutyProtocolId,
        int[] personIds,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<NvoExamDutyProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.NvoExamDutyProtocolId == examDutyProtocolId &&
                this.DbContext.MakeIdsQuery(personIds).Any(id => edps.PersonId == id.Id)

            select edps.PersonId
        ).AnyAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int nvoExamDutyProtocolId,
       CancellationToken ct)
    {
        var supervisors = await (
            from edps in this.DbContext.Set<NvoExamDutyProtocolSupervisor>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            where edps.SchoolYear == schoolYear && edps.NvoExamDutyProtocolId == nvoExamDutyProtocolId
            orderby p.FirstName, p.MiddleName, p.LastName
            select new GetWordDataVOSupervisor(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName))
        ).ToArrayAsync(ct);

        var students = await (
            from edps in this.DbContext.Set<NvoExamDutyProtocolStudent>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edps.SchoolYear, edps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
            into j1
            from cgp in j1.DefaultIfEmpty()

            where edps.SchoolYear == schoolYear && edps.NvoExamDutyProtocolId == nvoExamDutyProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetWordDataVOStudent(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName), cgp.ClassName ?? cg.ClassName)
        ).ToArrayAsync(ct);

        return await (
            from edp in this.DbContext.Set<NvoExamDutyProtocol>()
            join s in this.DbContext.Set<Subject>() on edp.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on edp.SubjectTypeId equals st.SubjectTypeId
            join p in this.DbContext.Set<Person>() on edp.DirectorPersonId equals p.PersonId
            join bc in this.DbContext.Set<BasicClass>() on edp.BasicClassId equals bc.BasicClassId
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

            where edp.SchoolYear == schoolYear && edp.NvoExamDutyProtocolId == nvoExamDutyProtocolId

            select new GetWordDataVO(
                edp.SchoolYear + " / " + (edp.SchoolYear + 1),
                i.Name,
                !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                edp.ProtocolNumber,
                edp.ProtocolDate != null ? edp.ProtocolDate!.Value.ToString("dd.MM.yyyy") : null,
                bc.Name,
                s.SubjectName + ' ' + st.Name,
                edp.Date.ToString("dd.MM.yyyy"),
                edp.RoomNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                supervisors,
                students)
         ).SingleAsync(ct);
    }
}

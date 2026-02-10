namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IGradeChangeExamsAdmProtocolQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class GradeChangeExamsAdmProtocolQueryRepository : Repository, IGradeChangeExamsAdmProtocolQueryRepository
{
    public GradeChangeExamsAdmProtocolQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        CancellationToken ct)
    {
        return (await (
            from ap in this.DbContext.Set<GradeChangeExamsAdmProtocol>()
            join apc in this.DbContext.Set<GradeChangeExamsAdmProtocolCommissioner>()
                on new { ap.SchoolYear, ap.GradeChangeExamsAdmProtocolId }
                equals new { apc.SchoolYear, apc.GradeChangeExamsAdmProtocolId }

            where ap.SchoolYear == schoolYear &&
                ap.GradeChangeExamsAdmProtocolId == gradeChangeExamsAdmProtocolId

            select new
            {
                ap.SchoolYear,
                ap.GradeChangeExamsAdmProtocolId,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.CommissionMeetingDate,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.ExamSession,
                ap.DirectorPersonId,
                apc.PersonId,
                apc.IsChairman,
                apc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(ap => new
            {
                ap.SchoolYear,
                ap.GradeChangeExamsAdmProtocolId,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.CommissionMeetingDate,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.ExamSession,
                ap.DirectorPersonId,
            })
            .Select(g => new GetVO(
                g.Key.GradeChangeExamsAdmProtocolId,
                g.Key.ProtocolNum,
                g.Key.ProtocolDate,
                g.Key.CommissionMeetingDate,
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate,
                g.Key.ExamSession,
                g.Key.DirectorPersonId,
                g.Where(c => c.IsChairman == true).Select(c => c.PersonId).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.PersonId).ToArray()
            ))
            .Single();
    }

    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var subjects = (await (
            from aps in this.DbContext.Set<GradeChangeExamsAdmProtocolStudent>()
            join apss in this.DbContext.Set<GradeChangeExamsAdmProtocolStudentSubject>()
                on new { aps.SchoolYear, aps.GradeChangeExamsAdmProtocolId, aps.ClassId, aps.PersonId }
                equals new { apss.SchoolYear, apss.GradeChangeExamsAdmProtocolId, apss.ClassId, apss.PersonId }
            join s in this.DbContext.Set<Subject>() on apss.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on apss.SubjectTypeId equals st.SubjectTypeId

            where aps.SchoolYear == schoolYear &&
                aps.GradeChangeExamsAdmProtocolId == gradeChangeExamsAdmProtocolId

            select new
            {
                aps.ClassId,
                aps.PersonId,
                SubjectName = st.SubjectTypeId == SubjectType.DefaultSubjectTypeId ? s.SubjectName : $"{s.SubjectName} / {st.Name}",
            })
            .ToArrayAsync(ct))
            .ToLookup(s => new { s.ClassId, s.PersonId }, s => s.SubjectName);

        return await (
            from aps in this.DbContext.Set<GradeChangeExamsAdmProtocolStudent>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { aps.SchoolYear, aps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1 from cgp in j1.DefaultIfEmpty()
            join p in this.DbContext.Set<Person>() on aps.PersonId equals p.PersonId

            where aps.SchoolYear == schoolYear &&
                aps.GradeChangeExamsAdmProtocolId == gradeChangeExamsAdmProtocolId

            orderby cg.BasicClassId, cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetStudentAllVO(
                cgp.ClassName ?? cg.ClassName,
                aps.ClassId,
                aps.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                subjects[new { aps.ClassId, aps.PersonId }].ToArray()
            ))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct)
    {
        return (await (
            from aps in this.DbContext.Set<GradeChangeExamsAdmProtocolStudent>()
            join apss in this.DbContext.Set<GradeChangeExamsAdmProtocolStudentSubject>()
                on new { aps.SchoolYear, aps.GradeChangeExamsAdmProtocolId, aps.ClassId, aps.PersonId }
                equals new { apss.SchoolYear, apss.GradeChangeExamsAdmProtocolId, apss.ClassId, apss.PersonId }

            where aps.SchoolYear == schoolYear &&
                aps.GradeChangeExamsAdmProtocolId == gradeChangeExamsAdmProtocolId &&
                aps.ClassId == classId &&
                aps.PersonId == personId

            select new
            {
                 aps.ClassId,
                 aps.PersonId,
                 apss.SubjectId,
                 apss.SubjectTypeId,
            })
            .ToArrayAsync(ct))
            .GroupBy(s => new { s.ClassId, s.PersonId })
            .Select(g => new GetStudentVO(
                g.Key.ClassId,
                g.Key.PersonId,
                g.Select(s => new GetStudentVOSubject(s.SubjectId, s.SubjectTypeId)).ToArray()
            ))
            .Single();
    }

    public async Task<bool> IsStudentDuplicatedAsync(
        int schoolYear,
        int gradeChangeExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<GradeChangeExamsAdmProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.GradeChangeExamsAdmProtocolId == gradeChangeExamsAdmProtocolId &&
                edps.ClassId == classId &&
                edps.PersonId == personId

            select edps.PersonId
        ).AnyAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
        int schoolYear,
        int admProtocolId,
        CancellationToken ct)
    {
        var subjects = (await (
            from apss in this.DbContext.Set<GradeChangeExamsAdmProtocolStudentSubject>()
            join s in this.DbContext.Set<Subject>() on apss.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on apss.SubjectTypeId equals st.SubjectTypeId

            where apss.SchoolYear == schoolYear &&
                apss.GradeChangeExamsAdmProtocolId == admProtocolId

            select new
            {
                apss.ClassId,
                apss.PersonId,
                SubjectName = st.SubjectTypeId == SubjectType.DefaultSubjectTypeId ? s.SubjectName : $"{s.SubjectName} / {st.Name}",
            })
            .ToArrayAsync(ct))
            .ToLookup(s => (s.ClassId, s.PersonId), s => s.SubjectName);

        var students = (await (
            from aps in this.DbContext.Set<GradeChangeExamsAdmProtocolStudent>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { aps.SchoolYear, aps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1 from cgp in j1.DefaultIfEmpty()
            join p in this.DbContext.Set<Person>() on aps.PersonId equals p.PersonId

            where aps.SchoolYear == schoolYear &&
                aps.GradeChangeExamsAdmProtocolId == admProtocolId

            orderby cg.BasicClassId, cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new
            {
                aps.ClassId,
                aps.PersonId,
                ClassName = cgp.ClassName ?? cg.ClassName,
                p.FirstName,
                p.MiddleName,
                p.LastName
            })
            .ToArrayAsync(ct))
            .Select(st => new GetWordDataVOStudent(
                st.ClassName,
                StringUtils.JoinNames(st.FirstName, st.MiddleName, st.LastName),
                string.Join(", ", subjects[(st.ClassId, st.PersonId)].ToArray())
            ))
            .ToArray();

        return (await (
            from ap in this.DbContext.Set<GradeChangeExamsAdmProtocol>()
            join apc in this.DbContext.Set<GradeChangeExamsAdmProtocolCommissioner>()
                on new { ap.SchoolYear, ap.GradeChangeExamsAdmProtocolId }
                equals new { apc.SchoolYear, apc.GradeChangeExamsAdmProtocolId }
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { ap.InstId, ap.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }
            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId into g2 from t in g2.DefaultIfEmpty()
            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId into g3 from m in g3.DefaultIfEmpty()
            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId into g4 from r in g4.DefaultIfEmpty()
            join p in this.DbContext.Set<Person>() on ap.DirectorPersonId equals p.PersonId
            join pc in this.DbContext.Set<Person>() on apc.PersonId equals pc.PersonId

            where ap.SchoolYear == schoolYear &&
                ap.GradeChangeExamsAdmProtocolId == admProtocolId

            select new
            {
                InstName = i.Name,
                InstitutionTownName = !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                InstitutionMunicipalityName = !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                InstitutionRegionName = !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                ap.SchoolYear,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.CommissionMeetingDate,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.ExamSession,
                DirectorName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                DirectorNameInParentheses = StringUtils.JoinNames(p.FirstName, p.LastName),
                CommissionerName = StringUtils.JoinNames(pc.FirstName, pc.MiddleName, pc.LastName),
                apc.IsChairman,
                apc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(ap => new
            {
                ap.InstName,
                ap.InstitutionTownName,
                ap.InstitutionMunicipalityName,
                ap.InstitutionRegionName,
                ap.SchoolYear,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.CommissionMeetingDate,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.ExamSession,
                ap.DirectorName,
                ap.DirectorNameInParentheses,
            })
            .Select(g => new GetWordDataVO(
                g.Key.InstName,
                g.Key.InstitutionTownName,
                g.Key.InstitutionMunicipalityName,
                g.Key.InstitutionRegionName,
                $"{g.Key.SchoolYear}/{g.Key.SchoolYear + 1}",
                g.Key.ExamSession,
                g.Key.ProtocolNum,
                g.Key.ProtocolDate?.ToString("dd.MM.yyyy"),
                g.Key.CommissionMeetingDate.ToString("dd.MM.yyyy"),
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate.ToString("dd.MM.yyyy"),
                g.Key.DirectorName,
                g.Key.DirectorNameInParentheses,
                g.Where(c => c.IsChairman == true).Select(c => c.CommissionerName).Single(),
                g.Where(c => c.IsChairman == false)
                    .OrderBy(c => c.OrderNum)
                    .Select(c => new GetWordDataVOCommissioner(c.CommissionerName)).ToArray(),
                this.FormatCommissionMembersDivided(g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.CommissionerName).ToArray()),
                students
            ))
            .Single();
    }


    private GetWordDataVOCommissionerLeftRight[] FormatCommissionMembersDivided(string[] commissionerNames)
    {
        var leftColumnLength = (int)Math.Ceiling((decimal)commissionerNames.Length / 2);

        // We don't use OrderNum to prevent printing just "." at last right position
        return Enumerable.Range(0, leftColumnLength)
            .Select(i => new GetWordDataVOCommissionerLeftRight(
                $"{i + 1}. {commissionerNames[i]}",
                leftColumnLength + i < commissionerNames.Length ?
                    $"{leftColumnLength + i + 1}. {commissionerNames[leftColumnLength + i]}" :
                    string.Empty))
            .ToArray();
    }
}

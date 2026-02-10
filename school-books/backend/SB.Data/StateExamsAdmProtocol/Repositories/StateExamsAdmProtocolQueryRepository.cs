namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IStateExamsAdmProtocolQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class StateExamsAdmProtocolQueryRepository : Repository, IStateExamsAdmProtocolQueryRepository
{
    public StateExamsAdmProtocolQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int instId,
        int? schoolYear,
        AdmProtocolType? protocolType,
        string? protocolNum,
        DateTime? protocolDate,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var sPredicate = PredicateBuilder.True<StateExamsAdmProtocol>();
        sPredicate = sPredicate.AndEquals(p => p.SchoolYear, schoolYear);

        var gcPredicate = PredicateBuilder.True<GradeChangeExamsAdmProtocol>();
        gcPredicate = gcPredicate.AndEquals(p => p.SchoolYear, schoolYear);

        var stateExamsAdmProtocolQuery = this.DbContext.Set<StateExamsAdmProtocol>()
            .Where(sPredicate)
            .Where(ap => ap.InstId == instId)
            .Select(ap => new
            {
                AdmProtocolId = ap.StateExamsAdmProtocolId,
                ProtocolType = AdmProtocolType.StateExams,
                ap.SchoolYear,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.ExamSession
            });

        var gradeChangeExamsAdmProtocolQuery = this.DbContext.Set<GradeChangeExamsAdmProtocol>()
            .Where(gcPredicate)
            .Where(ap => ap.InstId == instId)
            .Select(ap => new
            {
                AdmProtocolId = ap.GradeChangeExamsAdmProtocolId,
                ProtocolType = AdmProtocolType.GradeChangeExams,
                ap.SchoolYear,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.ExamSession
            });

        var filterPredicate = PredicateBuilder.True(stateExamsAdmProtocolQuery);
        filterPredicate = filterPredicate.AndEquals(p => p.ProtocolType, protocolType);
        filterPredicate = filterPredicate.AndEquals(p => p.ProtocolDate, protocolDate);
        filterPredicate = filterPredicate.AndStringContains(ap => ap.ProtocolNum, protocolNum);

        return await stateExamsAdmProtocolQuery
            .Concat(gradeChangeExamsAdmProtocolQuery)
            .Where(filterPredicate)
            .OrderByDescending(ap => ap.AdmProtocolId)
            .Select(ap => new GetAllVO(
                ap.AdmProtocolId,
                ap.ProtocolType,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.ExamSession))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        CancellationToken ct)
    {
        return (await (
            from ap in this.DbContext.Set<StateExamsAdmProtocol>()
            join apc in this.DbContext.Set<StateExamsAdmProtocolCommissioner>()
                on new { ap.SchoolYear, ap.StateExamsAdmProtocolId }
                equals new { apc.SchoolYear, apc.StateExamsAdmProtocolId }

            where ap.SchoolYear == schoolYear &&
                ap.StateExamsAdmProtocolId == stateExamsAdmProtocolId

            select new
            {
                ap.SchoolYear,
                ap.StateExamsAdmProtocolId,
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
                ap.StateExamsAdmProtocolId,
                ap.ProtocolNum,
                ap.ProtocolDate,
                ap.CommissionMeetingDate,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.ExamSession,
                ap.DirectorPersonId,
            })
            .Select(g => new GetVO(
                g.Key.StateExamsAdmProtocolId,
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
        int stateExamsAdmProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var subjects = await (
            from apss in this.DbContext.Set<StateExamsAdmProtocolStudentSubject>()
            join s in this.DbContext.Set<Subject>() on apss.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on apss.SubjectTypeId equals st.SubjectTypeId

            where apss.SchoolYear == schoolYear &&
                apss.StateExamsAdmProtocolId == stateExamsAdmProtocolId

            select new
            {
                apss.ClassId,
                apss.PersonId,
                SubjectName = st.SubjectTypeId == SubjectType.DefaultSubjectTypeId ? s.SubjectName : $"{s.SubjectName} / {st.Name}",
                apss.IsAdditional
            })
            .ToArrayAsync(ct);

        var additionalSubjects = subjects
            .Where(s => s.IsAdditional == true)
            .ToLookup(s => new { s.ClassId, s.PersonId }, s => s.SubjectName);

        var qualificationSubjects = subjects
            .Where(s => s.IsAdditional == false)
            .ToLookup(s => new { s.ClassId, s.PersonId }, s => s.SubjectName);


        return await (
            from aps in this.DbContext.Set<StateExamsAdmProtocolStudent>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { aps.SchoolYear, aps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join p in this.DbContext.Set<Person>() on aps.PersonId equals p.PersonId
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1 from cgp in j1.DefaultIfEmpty()
            join s in this.DbContext.Set<Subject>() on aps.SecondMandatorySubjectId equals s.SubjectId
                into j2 from s in j2.DefaultIfEmpty()
            join st in this.DbContext.Set<SubjectType>() on aps.SecondMandatorySubjectTypeId equals st.SubjectTypeId
                into j3 from st in j3.DefaultIfEmpty()

            where aps.SchoolYear == schoolYear &&
                aps.StateExamsAdmProtocolId == stateExamsAdmProtocolId

            orderby cg.BasicClassId, cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetStudentAllVO(
                cgp.ClassName ?? cg.ClassName,
                aps.ClassId,
                aps.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                aps.HasFirstMandatorySubject,
                st == null || st.SubjectTypeId == SubjectType.DefaultSubjectTypeId ? s.SubjectName : $"{s.SubjectName} / {st.Name}",
                additionalSubjects[new { aps.ClassId, aps.PersonId }].ToArray(),
                qualificationSubjects[new { aps.ClassId, aps.PersonId }].ToArray()
            ))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct)
    {
        var subjects = await this.DbContext.Set<StateExamsAdmProtocolStudentSubject>()
            .Where(apss => apss.SchoolYear == schoolYear &&
                apss.StateExamsAdmProtocolId == stateExamsAdmProtocolId &&
                apss.ClassId == classId &&
                apss.PersonId == personId)
            .Select(apss => new
            {
                apss.SubjectId,
                apss.SubjectTypeId,
                apss.IsAdditional
            })
            .ToArrayAsync(ct);

        var additionalSubjects = subjects
        .Where(s => s.IsAdditional == true)
            .Select(s => new GetStudentVOSubject(s.SubjectId, s.SubjectTypeId))
            .ToArray();

        var qualificationSubjects = subjects
            .Where(s => s.IsAdditional == false)
            .Select(s => new GetStudentVOSubject(s.SubjectId, s.SubjectTypeId))
            .ToArray();

        return await this.DbContext.Set<StateExamsAdmProtocolStudent>()
            .Where(aps => aps.SchoolYear == schoolYear &&
                aps.StateExamsAdmProtocolId == stateExamsAdmProtocolId &&
                aps.ClassId == classId &&
                aps.PersonId == personId)
            .Select(aps => new GetStudentVO(
                aps.ClassId,
                aps.PersonId,
                aps.HasFirstMandatorySubject,
                aps.SecondMandatorySubjectId != null ? new GetStudentVOSubject(aps.SecondMandatorySubjectId.Value, aps.SecondMandatorySubjectTypeId ?? -1) : null,
                additionalSubjects,
                qualificationSubjects
            ))
            .SingleAsync(ct);
    }

    public async Task<bool> IsStudentDuplicatedAsync(
        int schoolYear,
        int stateExamsAdmProtocolId,
        int classId,
        int personId,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<StateExamsAdmProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.StateExamsAdmProtocolId == stateExamsAdmProtocolId &&
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
        var subjects = await (
            from apss in this.DbContext.Set<StateExamsAdmProtocolStudentSubject>()
            join s in this.DbContext.Set<Subject>() on apss.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on apss.SubjectTypeId equals st.SubjectTypeId

            where apss.SchoolYear == schoolYear &&
                apss.StateExamsAdmProtocolId == admProtocolId

            select new
            {
                apss.ClassId,
                apss.PersonId,
                SubjectName = st.SubjectTypeId == SubjectType.DefaultSubjectTypeId ? s.SubjectName : $"{s.SubjectName} / {st.Name}",
                apss.IsAdditional
            })
            .ToArrayAsync(ct);

        var additionalSubjects = subjects
            .Where(s => s.IsAdditional == true)
            .ToLookup(s => (s.ClassId, s.PersonId), s => s.SubjectName);

        var qualificationSubjects = subjects
            .Where(s => s.IsAdditional == false)
            .ToLookup(s => (s.ClassId, s.PersonId), s => s.SubjectName);

        var students = (await (
            from aps in this.DbContext.Set<StateExamsAdmProtocolStudent>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { aps.SchoolYear, aps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join p in this.DbContext.Set<Person>() on aps.PersonId equals p.PersonId
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1 from cgp in j1.DefaultIfEmpty()
            join s in this.DbContext.Set<Subject>() on aps.SecondMandatorySubjectId equals s.SubjectId
                into j2 from s in j2.DefaultIfEmpty()
            join st in this.DbContext.Set<SubjectType>() on aps.SecondMandatorySubjectTypeId equals st.SubjectTypeId
                into j3 from st in j3.DefaultIfEmpty()

            where aps.SchoolYear == schoolYear &&
                aps.StateExamsAdmProtocolId == admProtocolId

            orderby cg.BasicClassId, cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new
            {
                aps.ClassId,
                aps.PersonId,
                ClassName = cgp.ClassName ?? cg.ClassName,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                aps.HasFirstMandatorySubject,
                SubjectName = st == null || st.SubjectTypeId == SubjectType.DefaultSubjectTypeId ? s.SubjectName : $"{s.SubjectName} / {st.Name}"
            })
           .ToArrayAsync(ct))
           .Select(st => new GetWordDataVOStudent(
                st.ClassName,
                StringUtils.JoinNames(st.FirstName, st.MiddleName, st.LastName),
                st.HasFirstMandatorySubject ? "ДА" : "-",
                st.SubjectName,
                additionalSubjects[(st.ClassId, st.PersonId)].Any() ?
                    string.Join(", ", additionalSubjects[(st.ClassId, st.PersonId)].ToArray()) :
                    "-",
                qualificationSubjects[(st.ClassId, st.PersonId)].Any() ?
                    string.Join(", ", qualificationSubjects[(st.ClassId, st.PersonId)].ToArray()) :
                    "-"
           ))
           .ToArray();

        return (await (
            from ap in this.DbContext.Set<StateExamsAdmProtocol>()
            join apc in this.DbContext.Set<StateExamsAdmProtocolCommissioner>()
                on new { ap.SchoolYear, ap.StateExamsAdmProtocolId }
                equals new { apc.SchoolYear, apc.StateExamsAdmProtocolId }
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { ap.InstId, ap.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }
            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId into g2 from t in g2.DefaultIfEmpty()
            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId into g3 from m in g3.DefaultIfEmpty()
            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId into g4 from r in g4.DefaultIfEmpty()
            join p in this.DbContext.Set<Person>() on ap.DirectorPersonId equals p.PersonId
            join pc in this.DbContext.Set<Person>() on apc.PersonId equals pc.PersonId

            where ap.SchoolYear == schoolYear &&
                ap.StateExamsAdmProtocolId == admProtocolId

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
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => new GetWordDataVOCommissioner(c.CommissionerName)).ToArray(),
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

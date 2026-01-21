namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IQualificationAcquisitionProtocolsQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class QualificationAcquisitionProtocolsQueryRepository : Repository, IQualificationAcquisitionProtocolsQueryRepository
{
    public QualificationAcquisitionProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        CancellationToken ct)
    {
        return (await (
            from ap in this.DbContext.Set<QualificationAcquisitionProtocol>()
            join apc in this.DbContext.Set<QualificationAcquisitionProtocolCommissioner>()
                on new { ap.SchoolYear, ap.QualificationAcquisitionProtocolId }
                equals new { apc.SchoolYear, apc.QualificationAcquisitionProtocolId }

            where ap.SchoolYear == schoolYear &&
                ap.QualificationAcquisitionProtocolId == qualificationAcquisitionProtocolId

            select new
            {
                ap.SchoolYear,
                ap.QualificationAcquisitionProtocolId,
                ap.ProtocolType,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.Profession,
                ap.Speciality,
                ap.QualificationDegreeId,
                ap.Date,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.DirectorPersonId,
                apc.PersonId,
                apc.IsChairman,
                apc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(ap => new
            {
                ap.SchoolYear,
                ap.QualificationAcquisitionProtocolId,
                ap.ProtocolType,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.Profession,
                ap.Speciality,
                ap.QualificationDegreeId,
                ap.Date,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                ap.DirectorPersonId,
            })
            .Select(g => new GetVO(
                g.Key.SchoolYear,
                g.Key.QualificationAcquisitionProtocolId,
                g.Key.ProtocolType,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate,
                g.Key.Profession,
                g.Key.Speciality,
                g.Key.QualificationDegreeId,
                g.Key.Date,
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate,
                g.Key.DirectorPersonId,
                g.Where(c => c.IsChairman == true).Select(c => c.PersonId).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.PersonId).ToArray()
            ))
            .Single();
    }

    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<QualificationAcquisitionProtocolStudent>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edps.SchoolYear, edps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
                into j1
            from cgp in j1.DefaultIfEmpty()

            where edps.SchoolYear == schoolYear &&
                edps.QualificationAcquisitionProtocolId == qualificationAcquisitionProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetStudentAllVO(
                cg.ClassId,
                p.PersonId,
                cgp.ClassName ?? cg.ClassName,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                edps.ExamsPassed,
                edps.TheoryPoints.ToString() ?? string.Empty,
                edps.PracticePoints.ToString() ?? string.Empty,
                GradeUtils.GetFullDecimalGradeText(edps.AverageDecimalGrade))
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetStudentVO> GetStudentAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        int classId,
        int personId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<QualificationAcquisitionProtocolStudent>()
            .Where(aps => aps.SchoolYear == schoolYear &&
                aps.QualificationAcquisitionProtocolId == qualificationAcquisitionProtocolId &&
                aps.ClassId == classId &&
                aps.PersonId == personId)
            .Select(aps => new GetStudentVO(
                aps.ClassId,
                aps.PersonId,
                aps.ExamsPassed,
                aps.TheoryPoints,
                aps.PracticePoints,
                aps.AverageDecimalGrade
            ))
            .SingleAsync(ct);
    }

    public async Task<bool> IsStudentDuplicatedAsync(
        int schoolYear,
        int qualificationAcquisitionProtocolId,
        int classId,
        int personId,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<QualificationAcquisitionProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.QualificationAcquisitionProtocolId == qualificationAcquisitionProtocolId &&
                edps.ClassId == classId &&
                edps.PersonId == personId

            select edps.PersonId
        ).AnyAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int qualificationAcquisitionProtocolId,
       CancellationToken ct)
    {
        var students = await (
            from edps in this.DbContext.Set<QualificationAcquisitionProtocolStudent>()
            join p in this.DbContext.Set<Person>() on edps.PersonId equals p.PersonId
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edps.SchoolYear, edps.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            join cgp in this.DbContext.Set<ClassGroup>()
                on new { cg.SchoolYear, cg.ParentClassId }
                equals new { cgp.SchoolYear, ParentClassId = (int?)cgp.ClassId }
            into j1
            from cgp in j1.DefaultIfEmpty()

            where edps.SchoolYear == schoolYear &&
                edps.QualificationAcquisitionProtocolId == qualificationAcquisitionProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            let totalPoints = edps.TheoryPoints + edps.PracticePoints

            select new GetWordDataVOStudent(
                cgp.ClassName ?? cg.ClassName,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                edps.ExamsPassed,
                edps.TheoryPoints != null ? edps.TheoryPoints!.Value.ToString("0.00") : string.Empty,
                edps.PracticePoints != null ? edps.PracticePoints!.Value.ToString("0.00") : string.Empty,
                totalPoints != null ? totalPoints!.Value.ToString("0.00") : string.Empty,
                edps.AverageDecimalGrade != null ? edps.AverageDecimalGrade!.Value.ToString("0.00") : string.Empty,
                GradeUtils.GetDecimalGradeTextOnly(edps.AverageDecimalGrade)
                )
        ).ToArrayAsync(ct);

        return (await (
            from erp in this.DbContext.Set<QualificationAcquisitionProtocol>()
            join qd in this.DbContext.Set<QualificationDegree>() on erp.QualificationDegreeId equals qd.Id
            join p in this.DbContext.Set<Person>() on erp.DirectorPersonId equals p.PersonId
            join erpc in this.DbContext.Set<QualificationAcquisitionProtocolCommissioner>()
                on new { erp.SchoolYear, erp.QualificationAcquisitionProtocolId }
                equals new { erpc.SchoolYear, erpc.QualificationAcquisitionProtocolId }
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { erp.InstId, erp.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }

            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId
            into g2
            from t in g2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into g3
            from m in g3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into g4
            from r in g4.DefaultIfEmpty()
            join pc in this.DbContext.Set<Person>() on erpc.PersonId equals pc.PersonId

            where erp.SchoolYear == schoolYear &&
                erp.QualificationAcquisitionProtocolId == qualificationAcquisitionProtocolId

            select new
            {
                SchoolYear = erp.SchoolYear + " / " + (erp.SchoolYear + 1),
                InstitutionName = i.Name,
                InstitutionTownName = !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                InstitutionMunicipalityName = !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                InstitutionRegionName = !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                erp.ProtocolType,
                erp.ProtocolNumber,
                erp.ProtocolDate,
                erp.Profession,
                erp.Speciality,
                QualificationDegreeName = qd.Name,
                erp.Date,
                erp.CommissionNominationOrderNumber,
                erp.CommissionNominationOrderDate,
                DirectorName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                DirectorNameInParentheses = StringUtils.JoinNames(p.FirstName, p.LastName),
                CommissionerName = StringUtils.JoinNames(pc.FirstName, pc.MiddleName, pc.LastName),
                erpc.IsChairman,
                erpc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(erp => new
            {
                erp.SchoolYear,
                erp.InstitutionName,
                erp.InstitutionTownName,
                erp.InstitutionMunicipalityName,
                erp.InstitutionRegionName,
                erp.ProtocolType,
                erp.ProtocolNumber,
                erp.ProtocolDate,
                erp.Profession,
                erp.Speciality,
                erp.QualificationDegreeName,
                erp.Date,
                erp.DirectorName,
                erp.DirectorNameInParentheses,
                erp.CommissionNominationOrderNumber,
                erp.CommissionNominationOrderDate
            })
            .Select(g => new GetWordDataVO(
                g.Key.SchoolYear,
                g.Key.InstitutionName,
                g.Key.InstitutionTownName,
                g.Key.InstitutionMunicipalityName,
                g.Key.InstitutionRegionName,
                g.Key.ProtocolType,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate?.ToString("dd.MM.yyyy"),
                g.Key.Profession,
                g.Key.Speciality,
                g.Key.QualificationDegreeName,
                g.Key.Date.ToString("dd.MM.yyyy"),
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate.ToString("dd.MM.yyyy"),
                g.Key.DirectorName,
                g.Key.DirectorNameInParentheses,
                g.Where(c => c.IsChairman == true).Select(c => c.CommissionerName).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => new GetWordDataVOCommissioner(c.CommissionerName)).ToArray(),
                this.FormatCommissionMembersDivided(g.Where(c => c.IsChairman == false).Select(c => c.CommissionerName).ToArray()),
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

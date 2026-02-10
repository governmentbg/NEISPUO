namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IQualificationExamResultProtocolsQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class QualificationExamResultProtocolsQueryRepository : Repository, IQualificationExamResultProtocolsQueryRepository
{
    public QualificationExamResultProtocolsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int qualificationExamResultProtocolId,
        CancellationToken ct)
    {
        var classIds = await (
            from edpc in this.DbContext.Set<QualificationExamResultProtocolClass>()
            where edpc.SchoolYear == schoolYear && edpc.QualificationExamResultProtocolId == qualificationExamResultProtocolId
            select edpc.ClassId
         ).ToArrayAsync(ct);

        return (await (
            from ap in this.DbContext.Set<QualificationExamResultProtocol>()
            join apc in this.DbContext.Set<QualificationExamResultProtocolCommissioner>()
                on new { ap.SchoolYear, ap.QualificationExamResultProtocolId }
                equals new { apc.SchoolYear, apc.QualificationExamResultProtocolId }

            where ap.SchoolYear == schoolYear &&
                ap.QualificationExamResultProtocolId == qualificationExamResultProtocolId

            select new
            {
                ap.SchoolYear,
                ap.QualificationExamResultProtocolId,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.SessionType,
                ap.Profession,
                ap.Speciality,
                ap.QualificationDegreeId,
                ap.GroupNum,
                ap.EduFormId,
                ap.QualificationExamTypeId,
                ap.Date,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate,
                apc.PersonId,
                apc.IsChairman,
                apc.OrderNum
            })
            .ToArrayAsync(ct))
            .GroupBy(ap => new
            {
                ap.SchoolYear,
                ap.QualificationExamResultProtocolId,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.SessionType,
                ap.Profession,
                ap.Speciality,
                ap.QualificationDegreeId,
                ap.GroupNum,
                ap.EduFormId,
                ap.QualificationExamTypeId,
                ap.Date,
                ap.CommissionNominationOrderNumber,
                ap.CommissionNominationOrderDate
            })
            .Select(g => new GetVO(
                g.Key.SchoolYear,
                g.Key.QualificationExamResultProtocolId,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate,
                g.Key.SessionType,
                g.Key.Profession,
                g.Key.Speciality,
                g.Key.QualificationDegreeId,
                g.Key.GroupNum,
                classIds,
                g.Key.EduFormId,
                g.Key.QualificationExamTypeId,
                g.Key.Date,
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate,
                g.Where(c => c.IsChairman == true).Select(c => c.PersonId).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.PersonId).ToArray()
            ))
            .Single();
    }

    public async Task<TableResultVO<GetStudentAllVO>> GetStudentAllAsync(
        int schoolYear,
        int qualificationExamResultProtocolId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<QualificationExamResultProtocolStudent>()
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
                edps.QualificationExamResultProtocolId == qualificationExamResultProtocolId
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
        int qualificationExamResultProtocolId,
        int[] personIds,
    CancellationToken ct)
    {
        return await (
            from edps in this.DbContext.Set<QualificationExamResultProtocolStudent>()

            where edps.SchoolYear == schoolYear &&
                edps.QualificationExamResultProtocolId == qualificationExamResultProtocolId &&
                this.DbContext.MakeIdsQuery(personIds).Any(id => edps.PersonId == id.Id)

            select edps.PersonId
        ).AnyAsync(ct);
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int qualificationExamResultProtocolId,
       CancellationToken ct)
    {
        var classNames = await (
            from edpc in this.DbContext.Set<QualificationExamResultProtocolClass>()
            join cg in this.DbContext.Set<ClassGroup>()
                on new { edpc.SchoolYear, edpc.ClassId }
                equals new { cg.SchoolYear, cg.ClassId }
            where edpc.SchoolYear == schoolYear && edpc.QualificationExamResultProtocolId == qualificationExamResultProtocolId
            select cg.ClassName
        ).ToArrayAsync(ct);

        var students = await (
            from edps in this.DbContext.Set<QualificationExamResultProtocolStudent>()
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
                edps.QualificationExamResultProtocolId == qualificationExamResultProtocolId
            orderby cgp.ClassName, cg.ClassName, p.FirstName, p.MiddleName, p.LastName

            select new GetWordDataVOStudent(StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName), cgp.ClassName ?? cg.ClassName)
        ).ToArrayAsync(ct);

        return (await (
            from erp in this.DbContext.Set<QualificationExamResultProtocol>()
            join qd in this.DbContext.Set<QualificationDegree>() on erp.QualificationDegreeId equals qd.Id
            join qet in this.DbContext.Set<QualificationExamType>() on erp.QualificationExamTypeId equals qet.Id
            join erpc in this.DbContext.Set<QualificationExamResultProtocolCommissioner>()
                on new { erp.SchoolYear, erp.QualificationExamResultProtocolId }
                equals new { erpc.SchoolYear, erpc.QualificationExamResultProtocolId }
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { erp.InstId, erp.SchoolYear } equals new { InstId = i.InstitutionId, i.SchoolYear }

            join ef in this.DbContext.Set<EduForm>() on erp.EduFormId equals ef.ClassEduFormId
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
            join pc in this.DbContext.Set<Person>() on erpc.PersonId equals pc.PersonId

            where erp.SchoolYear == schoolYear &&
                erp.QualificationExamResultProtocolId == qualificationExamResultProtocolId

            select new
            {
                SchoolYear = erp.SchoolYear + " / " + (erp.SchoolYear + 1),
                InstitutionName = i.Name,
                InstitutionTownName = !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                InstitutionMunicipalityName = !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                InstitutionRegionName = !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                erp.ProtocolNumber,
                erp.ProtocolDate,
                erp.SessionType,
                erp.Profession,
                erp.Speciality,
                QualificationDegreeName = qd.Name,
                erp.GroupNum,
                EduFormName = ef.Name,
                QualificationExamTypeName = qet.Name,
                erp.Date,
                erp.CommissionNominationOrderNumber,
                erp.CommissionNominationOrderDate,
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
                erp.ProtocolNumber,
                erp.ProtocolDate,
                erp.SessionType,
                erp.Profession,
                erp.Speciality,
                erp.QualificationDegreeName,
                erp.GroupNum,
                erp.EduFormName,
                erp.QualificationExamTypeName,
                erp.Date,
                erp.CommissionNominationOrderNumber,
                erp.CommissionNominationOrderDate
            })
            .Select(g => new GetWordDataVO(
                g.Key.SchoolYear,
                g.Key.InstitutionName,
                g.Key.InstitutionTownName,
                g.Key.InstitutionMunicipalityName,
                g.Key.InstitutionRegionName,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate?.ToString("dd.MM.yyyy"),
                g.Key.SessionType,
                g.Key.Profession,
                g.Key.Speciality,
                g.Key.QualificationDegreeName,
                g.Key.GroupNum,
                classNames,
                g.Key.EduFormName,
                g.Key.QualificationExamTypeName,
                g.Key.Date.ToString("dd.MM.yyyy"),
                g.Key.CommissionNominationOrderNumber,
                g.Key.CommissionNominationOrderDate.ToString("dd.MM.yyyy"),
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

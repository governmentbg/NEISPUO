namespace SB.Data;

using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static SB.Domain.IGraduationThesisDefenseProtocolQueryRepository;
using SB.Common;
using System.Threading;
using System;

internal class GraduationThesisDefenseProtocolQueryRepository : Repository, IGraduationThesisDefenseProtocolQueryRepository
{
    public GraduationThesisDefenseProtocolQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int graduationThesisDefenseProtocolId,
        CancellationToken ct)
    {
        return (await (
            from ap in this.DbContext.Set<GraduationThesisDefenseProtocol>()
            join apc in this.DbContext.Set<GraduationThesisDefenseProtocolCommissioner>()
                on new { ap.SchoolYear, ap.GraduationThesisDefenseProtocolId }
                equals new { apc.SchoolYear, apc.GraduationThesisDefenseProtocolId }

            where ap.SchoolYear == schoolYear &&
                ap.GraduationThesisDefenseProtocolId == graduationThesisDefenseProtocolId

            select new
            {
                ap.SchoolYear,
                ap.GraduationThesisDefenseProtocolId,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.SessionType,
                ap.EduFormId,
                ap.CommissionMeetingDate,
                ap.DirectorOrderNumber,
                ap.DirectorOrderDate,
                ap.DirectorPersonId,
                apc.PersonId,
                apc.IsChairman,
                apc.OrderNum,
                ap.Section1StudentsCapacity,
                ap.Section2StudentsCapacity,
                ap.Section3StudentsCapacity,
                ap.Section4StudentsCapacity,
            })
            .ToArrayAsync(ct))
            .GroupBy(ap => new
            {
                ap.SchoolYear,
                ap.GraduationThesisDefenseProtocolId,
                ap.ProtocolNumber,
                ap.ProtocolDate,
                ap.SessionType,
                ap.EduFormId,
                ap.CommissionMeetingDate,
                ap.DirectorOrderNumber,
                ap.DirectorOrderDate,
                ap.DirectorPersonId,
                ap.Section1StudentsCapacity,
                ap.Section2StudentsCapacity,
                ap.Section3StudentsCapacity,
                ap.Section4StudentsCapacity,
            })
            .Select(g => new GetVO(
                g.Key.SchoolYear,
                g.Key.GraduationThesisDefenseProtocolId,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate,
                g.Key.SessionType,
                g.Key.EduFormId,
                g.Key.CommissionMeetingDate,
                g.Key.DirectorOrderNumber,
                g.Key.DirectorOrderDate,
                g.Key.DirectorPersonId,
                g.Where(c => c.IsChairman == true).Select(c => c.PersonId).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.PersonId).ToArray(),
                g.Key.Section1StudentsCapacity,
                g.Key.Section2StudentsCapacity,
                g.Key.Section3StudentsCapacity,
                g.Key.Section4StudentsCapacity
            ))
            .Single();
    }

    public async Task<GetWordDataVO> GetWordDataAsync(
       int schoolYear,
       int gaduationThesisDefenseProtocolId,
       CancellationToken ct)
    {
        return (await (
            from erp in this.DbContext.Set<GraduationThesisDefenseProtocol>()
            join p in this.DbContext.Set<Person>() on erp.DirectorPersonId equals p.PersonId
            join erpc in this.DbContext.Set<GraduationThesisDefenseProtocolCommissioner>()
                on new { erp.SchoolYear, erp.GraduationThesisDefenseProtocolId }
                equals new { erpc.SchoolYear, erpc.GraduationThesisDefenseProtocolId }
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
                erp.GraduationThesisDefenseProtocolId == gaduationThesisDefenseProtocolId

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
                EduFormName = ef.Name,
                erp.CommissionMeetingDate,
                erp.DirectorOrderNumber,
                erp.DirectorOrderDate,
                CommissionerName = StringUtils.JoinNames(pc.FirstName, pc.MiddleName, pc.LastName),
                erpc.IsChairman,
                erpc.OrderNum,
                DirectorName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                DirectorNameInParentheses = StringUtils.JoinNames(p.FirstName, p.LastName),
                erp.Section1StudentsCapacity,
                erp.Section2StudentsCapacity,
                erp.Section3StudentsCapacity,
                erp.Section4StudentsCapacity
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
                erp.EduFormName,
                erp.CommissionMeetingDate,
                erp.DirectorOrderNumber,
                erp.DirectorOrderDate,
                erp.DirectorName,
                erp.DirectorNameInParentheses,
                erp.Section1StudentsCapacity,
                erp.Section2StudentsCapacity,
                erp.Section3StudentsCapacity,
                erp.Section4StudentsCapacity
            })
            .Select(g => new GetWordDataVO(
                g.Key.InstitutionName,
                g.Key.InstitutionTownName,
                g.Key.InstitutionMunicipalityName,
                g.Key.InstitutionRegionName,
                g.Key.SchoolYear,
                g.Key.ProtocolNumber,
                g.Key.ProtocolDate?.ToString("dd.MM.yyyy"),
                g.Key.SessionType,
                g.Key.EduFormName,
                g.Key.CommissionMeetingDate.ToString("dd.MM.yyyy"),
                g.Key.DirectorOrderNumber,
                g.Key.DirectorOrderDate.ToString("dd.MM.yyyy"),
                g.Key.DirectorName,
                g.Key.DirectorNameInParentheses,
                g.Where(c => c.IsChairman == true).Select(c => c.CommissionerName).Single(),
                g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => new GetWordDataVOCommissioner(c.CommissionerName)).ToArray(),
                this.FormatCommissionMembersDivided(g.Where(c => c.IsChairman == false).OrderBy(c => c.OrderNum).Select(c => c.CommissionerName).ToArray()),
                g.Key.Section1StudentsCapacity,
                g.Key.Section2StudentsCapacity,
                g.Key.Section3StudentsCapacity,
                g.Key.Section4StudentsCapacity
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

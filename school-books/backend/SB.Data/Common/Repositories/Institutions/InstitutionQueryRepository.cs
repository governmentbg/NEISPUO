namespace SB.Data;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IInstitutionsQueryRepository;

internal class InstitutionsQueryRepository : Repository, IInstitutionsQueryRepository
{
    private const int NeispuoStartSchoolYear = 2021;
    private const int BsDetailedSchoolTypeId = 133;
    private const int SpDetailedSchoolTypeId = 134;

    private const int SofiaCityId = 22;
    private const int SofiaRegionId = 23;
    private const int RcpppoSofiaRegionInstitutionId = 2300014;

    private static readonly int[] DirectorNKPDPositionIds = new[] { 1, 3, 4, 201, 263, 602, 679, 722, 856, 857, 5869, 5870 };

    public InstitutionsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int? regionId,
        string? institutionId,
        string? institutionName,
        string? townName,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var institutionPredicate = PredicateBuilder.True<Institution>();
        if (!string.IsNullOrWhiteSpace(institutionName))
        {
            string[] words = institutionName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                institutionPredicate = institutionPredicate.AndStringContains(i => i.Abbreviation, word);
            }
        }

        if (!string.IsNullOrWhiteSpace(institutionId))
        {
            institutionPredicate = institutionPredicate.AndStringContains(i => i.InstitutionId.ToString(), institutionId);
        }

        var townPredicate = PredicateBuilder.True<Town>();
        if (!string.IsNullOrWhiteSpace(townName))
        {
            string[] words = townName.Split(null); // split by whitespace
            foreach (var word in words)
            {
                townPredicate = townPredicate.AndStringContains(t => t.Name, word);
            }
        }

        // this query is based uppon the [inst_basic].[NavigationTree] view
        // and should be kept in sync with it
        return await (
            from i in this.DbContext.Set<Institution>()
                .Where(institutionPredicate)

                // InstitutionConfData is never supposed to have more
                // than one row per institution and is used to determine
                // the current school year
            join icd in this.DbContext.Set<InstitutionConfData>()
            on i.InstitutionId equals icd.InstitutionId

            join id in this.DbContext.Set<InstitutionDepartment>()
            on i.InstitutionId equals id.InstitutionId

            join t in this.DbContext.Set<Town>()
                .Where(townPredicate)
            on id.TownId equals t.TownId

            join m in this.DbContext.Set<Municipality>()
            on t.MunicipalityId equals m.MunicipalityId

            join r in this.DbContext.Set<Region>()
            on m.RegionId equals r.RegionId

            join dst in this.DbContext.Set<DetailedSchoolType>()
            on i.DetailedSchoolTypeId equals dst.DetailedSchoolTypeId

            join bi in this.DbContext.Set<BudgetingInstitution>()
            on i.BudgetingSchoolTypeId equals bi.BudgetingInstitutionId

            where id.IsMain == true &&
                (
                  regionId == null
                  || (regionId != SofiaCityId && regionId != SofiaRegionId && r.RegionId == regionId)

                  // Workaround - hide institution with id 2300014 from the list of institutions for Sofia city
                  // and show it only for Sofia region for RuoAdmin and RuoExperts.
                  || (regionId == SofiaCityId && r.RegionId == SofiaCityId && i.InstitutionId != RcpppoSofiaRegionInstitutionId)
                  || (regionId == SofiaRegionId && (r.RegionId == SofiaRegionId || i.InstitutionId == RcpppoSofiaRegionInstitutionId))
                )

            orderby i.InstitutionId

            select new GetAllVO(
                i.InstitutionId,
                icd.SchoolYear,
                i.Abbreviation,
                t.Name)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        bool hasInstitutionAdminWriteAccess,
        CancellationToken ct)
    {
        var directorPersonId = await (
            from sp in this.DbContext.Set<StaffPosition>()
            where sp.InstitutionId == instId &&
                sp.CurrentlyValid &&
                sp.IsValid &&
                (sp.PositionKindId == StaffPosition.DefaultPositionKindId || sp.PositionKindId == null) &&
                this.DbContext.MakeIdsQuery(DirectorNKPDPositionIds)
                    .Any(id => sp.NKPDPositionId == id.Id)
            orderby sp.PositionKindId descending
            select (int?)sp.PersonId
        ).FirstOrDefaultAsync(ct);

        bool showDefaultYearSettingsBanner = false;
        bool showSchoolYearSettingsBanner = false;
        bool showClassBooksBanner = false;

        var institutionInfo = await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join d in this.DbContext.Set<DetailedSchoolType>() on i.DetailedSchoolTypeId equals d.DetailedSchoolTypeId

            where i.SchoolYear == schoolYear &&
                i.InstitutionId == instId

            select new
            {
                i.InstitutionId,
                InstitutionName = i.Name,
                IsCurrentSchoolYear = i.IsCurrent,
                d.InstType,
                d.BaseSchoolTypeId,
                d.DetailedSchoolTypeId,
                SchoolYears = this.DbContext
                    .Set<InstitutionSchoolYear>()
                    .Where(isy => isy.InstitutionId == instId && isy.SchoolYear >= NeispuoStartSchoolYear)
                    .OrderByDescending(isy => isy.SchoolYear)
                    .Select(isy => isy.SchoolYear)
                    .ToArray(),
                DirectorPersonId = directorPersonId
            }
        ).SingleAsync(ct);

        if (hasInstitutionAdminWriteAccess && institutionInfo.IsCurrentSchoolYear)
        {
            showDefaultYearSettingsBanner =
                !(await this.DbContext.Set<SchoolYearSettingsDefault>()
                .Where(dys =>
                    dys.SchoolYear == schoolYear)
                .AnyAsync(ct));

            showSchoolYearSettingsBanner =
                !(await this.DbContext.Set<SchoolYearSettings>()
                .Where(syd =>
                    syd.SchoolYear == schoolYear &&
                    syd.InstId == instId)
                .AnyAsync(ct));

            showClassBooksBanner =
                !(await this.DbContext.Set<ClassBook>()
                .Where(syd =>
                    syd.SchoolYear == schoolYear &&
                    syd.InstId == instId)
                .AnyAsync(ct));
        }

        return new GetVO(
            institutionInfo.InstitutionId,
            institutionInfo.InstitutionName,
            institutionInfo.InstType,
            institutionInfo.SchoolYears,
            institutionInfo.DirectorPersonId,
            ShowDefaultSettingsBanner: showDefaultYearSettingsBanner,
            ShowSchoolYearSettingsBanner: showSchoolYearSettingsBanner && institutionInfo.InstType != InstType.DG,
            ShowClassBooksBanner: showClassBooksBanner,
            ShowSpbsBook: institutionInfo.DetailedSchoolTypeId == BsDetailedSchoolTypeId ||
                institutionInfo.DetailedSchoolTypeId == SpDetailedSchoolTypeId,
            ShowReqBookQualification: institutionInfo.InstType != InstType.DG,
            ShowProtocols: institutionInfo.InstType != InstType.DG,
            ShowSchoolYearSettings: institutionInfo.InstType != InstType.DG
        );
    }

    public async Task<GetProtocolTemplateDataVO> GetProtocolTemplateDataAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var directorNames = await (
            from sp in this.DbContext.Set<StaffPosition>()
            join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId
            where sp.InstitutionId == instId &&
                sp.CurrentlyValid &&
                sp.IsValid &&
                (sp.PositionKindId == StaffPosition.DefaultPositionKindId || sp.PositionKindId == null) &&
                this.DbContext.MakeIdsQuery(DirectorNKPDPositionIds)
                    .Any(id => sp.NKPDPositionId == id.Id)
            orderby sp.PositionKindId descending
            select new
            {
                p.FirstName,
                p.MiddleName,
                p.LastName
            }
        ).FirstOrDefaultAsync(ct);

        return await (
            from i in this.DbContext.Set<InstitutionSchoolYear>()

            join la in this.DbContext.Set<LocalArea>() on i.LocalAreaId equals la.LocalAreaId
            into g1
            from la in g1.DefaultIfEmpty()

            join t in this.DbContext.Set<Town>() on i.TownId equals t.TownId
            into g2
            from t in g2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into g3
            from m in g3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into g4
            from r in g4.DefaultIfEmpty()

            where i.SchoolYear == schoolYear && i.InstitutionId == instId

            select new GetProtocolTemplateDataVO(
                i.Name,
                !string.IsNullOrEmpty(t.Name) ? $"град {t.Name}," : string.Empty,
                !string.IsNullOrEmpty(m.Name) ? $"община {m.Name}," : string.Empty,
                !string.IsNullOrEmpty(r.Name) ? $"област {r.Name}" : string.Empty,
                directorNames != null ? directorNames.FirstName : null,
                directorNames != null ? directorNames.MiddleName : null,
                directorNames != null ? directorNames.LastName : null)
         ).SingleAsync(ct);
    }
}

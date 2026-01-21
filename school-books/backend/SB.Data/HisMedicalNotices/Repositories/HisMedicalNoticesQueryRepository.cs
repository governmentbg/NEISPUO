
namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using SB.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using static SB.Domain.IHisMedicalNoticesQueryRepository;

internal class HisMedicalNoticesQueryRepository : Repository, IHisMedicalNoticesQueryRepository
{
    private readonly int NeispuoExtSystemId;
    private readonly int SqlCommandTimeout = 180;

    public HisMedicalNoticesQueryRepository(
        UnitOfWork unitOfWork,
        IOptions<DomainOptions> domainOptions)
        : base(unitOfWork)
    {
        this.NeispuoExtSystemId = domainOptions.Value.NeispuoExtSystemId
            ?? throw new Exception($"Missing {nameof(DomainOptions.NeispuoExtSystemId)} in configuration");
    }

    public async Task<MedicalNoticeBatchDO> GetNextWithReadReceiptsAndSaveAsync(
        int extSystemId,
        int next,
        CancellationToken ct)
    {
        // Increase SQL command timeout to avoid timeout exceptions
        this.DbContext.Database.SetCommandTimeout(this.SqlCommandTimeout);

        try
        {
            // take twice as much rows as requested
            // to account for the multiplication effect of
            // joining with HisMedicalNoticeSchoolYear/ClassBookExtProvider
            // and selecting (cbep.SchoolYear, cbep.InstId)
            int take = next * 2;

            var threeMonthsBack = DateTime.Now.AddMonths(-3);
            var res = await (
                from hmn in this.DbContext.Set<HisMedicalNotice>()

                join hmnsy in this.DbContext.Set<HisMedicalNoticeSchoolYear>()
                on hmn.HisMedicalNoticeId
                equals hmnsy.HisMedicalNoticeId

                join hmnrr in this.DbContext.Set<HisMedicalNoticeReadReceipt>().Where(hmnrr => hmnrr.ExtSystemId == extSystemId)
                on hmn.HisMedicalNoticeId equals hmnrr.HisMedicalNoticeId
                into j1 from hmnrr in j1.DefaultIfEmpty()

                join p in this.DbContext.Set<Person>()
                on new { hmn.Identifier, hmn.PersonalIdTypeId }
                equals new { Identifier = p.PersonalId, PersonalIdTypeId = p.PersonalIdType }

                join sc in this.DbContext.Set<StudentClass>()
                on new { hmnsy.SchoolYear, p.PersonId }
                equals new { sc.SchoolYear, sc.PersonId }

                join cbep in this.DbContext.Set<ClassBookExtProvider>()
                on new { sc.SchoolYear, sc.InstitutionId }
                equals new { cbep.SchoolYear, InstitutionId = cbep.InstId }

                where cbep.ExtSystemId == extSystemId &&
                    (from hmnrr in this.DbContext.Set<HisMedicalNoticeReadReceipt>()
                    where hmnrr.ExtSystemId == cbep.ExtSystemId &&
                        hmnrr.HisMedicalNoticeId == hmn.HisMedicalNoticeId &&
                        hmnrr.IsAcknowledged == true
                    select hmnrr.HisMedicalNoticeId).Any() == false &&
                    hmn.CreateDate > threeMonthsBack

                select new
                {
                    hmn.HisMedicalNoticeId,
                    hmn.NrnMedicalNotice,
                    hmn.NrnExamination,
                    hmn.IdentifierType,
                    hmn.Identifier,
                    hmn.GivenName,
                    hmn.FamilyName,
                    hmn.Pmi,
                    hmn.FromDate,
                    hmn.ToDate,
                    hmn.AuthoredOn,
                    p.PersonId,
                    HasReadReceipt = hmnrr != null,
                    cbep.SchoolYear,
                    cbep.InstId
                })
                // joining with StudentClass
                // may produce duplicate rows, so we need to remove them
                .Distinct()
                .OrderBy(hmn => hmn.HisMedicalNoticeId)
                .Take(take)
                .ToArrayAsync(ct);

            bool hasMore = res.Length >= take;

            var nextHisMedicalNotices =
                res.GroupBy(hmn =>
                    new
                    {
                        hmn.HisMedicalNoticeId,
                        hmn.NrnMedicalNotice,
                        hmn.NrnExamination,
                        hmn.IdentifierType,
                        hmn.Identifier,
                        hmn.GivenName,
                        hmn.FamilyName,
                        hmn.Pmi,
                        hmn.FromDate,
                        hmn.ToDate,
                        hmn.AuthoredOn,
                        hmn.PersonId,
                        hmn.HasReadReceipt,
                    })
                .OrderBy(hmn => hmn.Key.HisMedicalNoticeId)
                // limit the result to the requested number of rows
                .Take(next)
                .ToArray();

            var lastHisSyncTime = await this.DbContext.Set<HisMedicalNoticeBatch>()
                .OrderByDescending(hs => hs.HisMedicalNoticeBatchId)
                .Select(hs => (DateTime?)hs.CreateDate)
                .Take(1)
                .SingleOrDefaultAsync(ct);

            await this.DbContext.Set<HisMedicalNoticeReadReceipt>()
                .AddRangeAsync(
                    nextHisMedicalNotices
                        .Where(hmn => !hmn.Key.HasReadReceipt)
                        .Select(hmn =>
                            new HisMedicalNoticeReadReceipt(
                                extSystemId,
                                hmn.Key.HisMedicalNoticeId,
                                hmn.Select(hmn => (hmn.SchoolYear, hmn.InstId)).ToArray())),
                    ct);

            await this.DbContext.SaveChangesAsync(ct);

            return new MedicalNoticeBatchDO
            {
                MedicalNotices = nextHisMedicalNotices
                    .Select(hmn =>
                        new MedicalNoticeDO
                        {
                            MedicalNoticeId = hmn.Key.HisMedicalNoticeId,
                            PersonId = hmn.Key.PersonId,
                            HisMedicalNotice =
                                new HisMedicalNoticeDO
                                {
                                    NrnMedicalNotice = hmn.Key.NrnMedicalNotice,
                                    NrnExamination = hmn.Key.NrnExamination,
                                    Patient = new HisMedicalNoticePatientDO
                                    {
                                        IdentifierType = hmn.Key.IdentifierType,
                                        Identifier = hmn.Key.Identifier,
                                        GivenName = hmn.Key.GivenName,
                                        FamilyName = hmn.Key.FamilyName,
                                    },
                                    Practitioner = new HisMedicalNoticePractitionerDO
                                    {
                                        Pmi = hmn.Key.Pmi,
                                    },
                                    MedicalNotice = new HisMedicalNoticeInfoDO
                                    {
                                        FromDate = hmn.Key.FromDate,
                                        ToDate = hmn.Key.ToDate,
                                        AuthoredOn = hmn.Key.AuthoredOn,
                                    }
                                },
                        })
                    .ToArray(),
                LastHisSyncTime = lastHisSyncTime,
                HasMore = hasMore,
            };
        }
        catch (SqlException sqlEx)
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
    }

    public async Task<NeispuoMedicalNoticeBatchVO> GetNeispuoNextWithReadReceiptsAndSaveAsync(
        int next,
        CancellationToken ct)
    {
        // Increase SQL command timeout to avoid timeout exceptions
        this.DbContext.Database.SetCommandTimeout(this.SqlCommandTimeout);

        try
        {
            // take twice as much rows as requested
            // to account for the multiplication effect of
            // joining with HisMedicalNoticeSchoolYear
            int take = next * 2;

            var threeMonthsBack = DateTime.Now.AddMonths(-3);
            var res = await (
                from hmn in this.DbContext.Set<HisMedicalNotice>()

                join hmnsy in this.DbContext.Set<HisMedicalNoticeSchoolYear>()
                on hmn.HisMedicalNoticeId
                equals hmnsy.HisMedicalNoticeId

                join p in this.DbContext.Set<Person>()
                on new { hmn.Identifier, hmn.PersonalIdTypeId }
                equals new { Identifier = p.PersonalId, PersonalIdTypeId = p.PersonalIdType }

                join hmnrr in this.DbContext.Set<HisMedicalNoticeReadReceipt>()
                    .Where(hmnrr => hmnrr.ExtSystemId == this.NeispuoExtSystemId)
                on hmn.HisMedicalNoticeId
                equals hmnrr.HisMedicalNoticeId
                into j1 from hmnrr in j1.DefaultIfEmpty()

                where hmn.CreateDate > threeMonthsBack &&
                    (hmnrr == null || hmnrr.IsAcknowledged == false)

                select new
                {
                    hmn.HisMedicalNoticeId,
                    hmn.NrnMedicalNotice,
                    hmn.Pmi,
                    hmn.AuthoredOn,
                    hmn.FromDate,
                    hmn.ToDate,
                    hmnsy.SchoolYear,
                    p.PersonId,
                    HasReadReceipt = hmnrr != null,
                })
                .OrderBy(hmn => hmn.HisMedicalNoticeId)
                .Take(take)
                .ToArrayAsync(ct);

            bool hasMore = res.Length >= take;

            var nextHisMedicalNotices =
                res.GroupBy(hmn =>
                    new
                    {
                        hmn.HisMedicalNoticeId,
                        hmn.NrnMedicalNotice,
                        hmn.Pmi,
                        hmn.AuthoredOn,
                        hmn.FromDate,
                        hmn.ToDate,
                        hmn.PersonId,
                        hmn.HasReadReceipt
                    })
                .OrderBy(hmn => hmn.Key.HisMedicalNoticeId)
                // limit the result to the requested number of rows
                .Take(next)
                .ToArray();

            await this.DbContext.Set<HisMedicalNoticeReadReceipt>()
                .AddRangeAsync(
                    nextHisMedicalNotices.Where(hmn => !hmn.Key.HasReadReceipt)
                        .Select(hmn => new HisMedicalNoticeReadReceipt(this.NeispuoExtSystemId, hmn.Key.HisMedicalNoticeId)),
                    ct);

            await this.DbContext.SaveChangesAsync(ct);

            return new NeispuoMedicalNoticeBatchVO(
                nextHisMedicalNotices
                    .Select(hmn =>
                        new NeispuoMedicalNoticeVO(
                            hmn.Key.HisMedicalNoticeId,
                            hmn.Key.NrnMedicalNotice,
                            hmn.Key.Pmi,
                            hmn.Key.AuthoredOn,
                            hmn.Key.FromDate,
                            hmn.Key.ToDate,
                            hmn.Key.PersonId,
                            hmn.Select(g => g.SchoolYear).ToArray()))
                    .ToArray(),
                hasMore);
        }
        catch (SqlException sqlEx)
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
    }

    public async Task ExecuteAcknowledgeAsync(int extSystemId, int[] hisMedicalNoticeIds, CancellationToken ct)
    {
        await this.DbContext.Set<HisMedicalNoticeReadReceipt>()
            .Where(rr =>
                rr.ExtSystemId == extSystemId &&
                rr.IsAcknowledged == false &&
                this.DbContext.MakeIdsQuery(hisMedicalNoticeIds)
                    .Any(id => rr.HisMedicalNoticeId == id.Id))
            .ExecuteUpdateAsync(
                s => s.SetProperty(rr => rr.IsAcknowledged, true),
                ct);
    }

    public async Task<int> ExecuteAcknowledgeNeispuoAsync(int hisMedicalNoticeId, CancellationToken ct)
    {
        return await this.DbContext.Set<HisMedicalNoticeReadReceipt>()
            .Where(rr =>
                rr.ExtSystemId == this.NeispuoExtSystemId &&
                rr.HisMedicalNoticeId == hisMedicalNoticeId &&
                rr.IsAcknowledged == false)
            .ExecuteUpdateAsync(
                s => s.SetProperty(rr => rr.IsAcknowledged, true),
                ct);
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        string? nrnMedicalNotice,
        string? nrnExamination,
        string? identifier,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<HisMedicalNotice>();
        if (!string.IsNullOrWhiteSpace(nrnMedicalNotice))
        {
            predicate = predicate.AndStringContains(i => i.NrnMedicalNotice, nrnMedicalNotice);
        }

        if (!string.IsNullOrWhiteSpace(nrnExamination))
        {
            predicate = predicate.AndStringContains(i => i.NrnExamination, nrnExamination);
        }

        if (!string.IsNullOrWhiteSpace(identifier))
        {
            predicate = predicate.AndStringContains(i => i.Identifier, identifier);
        }

        var query =
            from hmn in this.DbContext.Set<HisMedicalNotice>().Where(predicate)
            join hmnsy in this.DbContext.Set<HisMedicalNoticeSchoolYear>() on hmn.HisMedicalNoticeId equals hmnsy.HisMedicalNoticeId

            where hmnsy.SchoolYear == schoolYear

            select new
            {
                hmnsy.SchoolYear,
                hmn.HisMedicalNoticeId,
                hmn.NrnMedicalNotice,
                hmn.NrnExamination,
                hmn.Identifier
            };

        return await query.GroupBy(q => new
        {
            q.HisMedicalNoticeId,
            q.NrnMedicalNotice,
            q.NrnExamination,
            q.Identifier
        })
        .Select(hmn => new GetAllVO(
            hmn.Key.HisMedicalNoticeId,
            hmn.Key.NrnMedicalNotice,
            hmn.Key.NrnExamination,
            hmn.Key.Identifier,
            string.Join(", ", hmn.Select(r => r.SchoolYear).Distinct()))
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<TableResultVO<GetAllVO>> GetAllForRegion(
        int regionId,
        int schoolYear,
        string? nrnMedicalNotice,
        string? nrnExamination,
        string? identifier,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<HisMedicalNotice>();
        if (!string.IsNullOrWhiteSpace(nrnMedicalNotice))
        {
            predicate = predicate.AndStringContains(i => i.NrnMedicalNotice, nrnMedicalNotice);
        }

        if (!string.IsNullOrWhiteSpace(nrnExamination))
        {
            predicate = predicate.AndStringContains(i => i.NrnExamination, nrnExamination);
        }

        if (!string.IsNullOrWhiteSpace(identifier))
        {
            predicate = predicate.AndStringContains(i => i.Identifier, identifier);
        }

        var query =
            from hmn in this.DbContext.Set<HisMedicalNotice>().Where(predicate)
            join hmnsy in this.DbContext.Set<HisMedicalNoticeSchoolYear>() on hmn.HisMedicalNoticeId equals hmnsy.HisMedicalNoticeId

            join p in this.DbContext.Set<Person>()
            on new { hmn.Identifier, hmn.PersonalIdTypeId }
            equals new { Identifier = p.PersonalId, PersonalIdTypeId = p.PersonalIdType }

            join sc in this.DbContext.Set<StudentClass>()
            on new { hmnsy.SchoolYear, p.PersonId }
            equals new { sc.SchoolYear, sc.PersonId }

            join id in this.DbContext.Set<InstitutionDepartment>()
            on sc.InstitutionId equals id.InstitutionId

            join t in this.DbContext.Set<Town>()
            on id.TownId equals t.TownId

            join m in this.DbContext.Set<Municipality>()
            on t.MunicipalityId equals m.MunicipalityId

            where hmnsy.SchoolYear == schoolYear && m.RegionId == regionId

            select new
            {
                hmnsy.SchoolYear,
                hmn.HisMedicalNoticeId,
                hmn.NrnMedicalNotice,
                hmn.NrnExamination,
                hmn.Identifier
            };

        return await query.GroupBy(q => new
        {
            q.HisMedicalNoticeId,
            q.NrnMedicalNotice,
            q.NrnExamination,
            q.Identifier
        })
        .Select(hmn => new GetAllVO(
            hmn.Key.HisMedicalNoticeId,
            hmn.Key.NrnMedicalNotice,
            hmn.Key.NrnExamination,
            hmn.Key.Identifier,
            string.Join(", ", hmn.Select(r => r.SchoolYear).Distinct()))
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int hisMedicalNoticeId,
        CancellationToken ct)
    {
        var matchedStudent = await (
            from pmn in this.DbContext.Set<PersonMedicalNotice>()
            join p in this.DbContext.Set<Person>() on pmn.PersonId equals p.PersonId
            where pmn.HisMedicalNoticeId == hisMedicalNoticeId
            select new
            {
                Name = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                p.PersonalId
            }
         ).FirstOrDefaultAsync(ct);

        var receipts = await (
            from hmnrr in this.DbContext.Set<HisMedicalNoticeReadReceipt>()
            join es in this.DbContext.Set<ExtSystem>() on hmnrr.ExtSystemId equals es.ExtSystemId
            where hmnrr.HisMedicalNoticeId == hisMedicalNoticeId
            orderby hmnrr.ExtSystemId

            select new GetVOReceipt(
                es.Name,
                hmnrr.CreateDate,
                hmnrr.IsAcknowledged ? hmnrr.ModifyDate : null,
                hmnrr.Accesses.Select(a => a.InstId.ToString() + '/' + a.SchoolYear.ToString()).ToArray())
        ).ToArrayAsync(ct);

        return await (
            from hmn in this.DbContext.Set<HisMedicalNotice>()

            where hmn.HisMedicalNoticeId == hisMedicalNoticeId
            select new GetVO(
                string.Join(", ", hmn.SchoolYears.Select(s => s.SchoolYear)),
                hmn.NrnMedicalNotice,
                hmn.NrnExamination,
                hmn.IdentifierType,
                hmn.Identifier,
                hmn.GivenName,
                hmn.FamilyName,
                hmn.Pmi,
                matchedStudent != null ? matchedStudent.Name : null,
                matchedStudent != null ? matchedStudent.PersonalId : null,
                hmn.AuthoredOn,
                hmn.FromDate,
                hmn.ToDate,
                receipts)
            )
            .SingleAsync(ct);
    }
}

namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IScheduleAndAbsencesByTermAllClassesReportsQueryRepository;

internal class ScheduleAndAbsencesByTermAllClassesReportsQueryRepository : Repository, IScheduleAndAbsencesByTermAllClassesReportsQueryRepository
{
    private readonly string blobServiceHmacKey;
    private readonly string blobServicePublicWebUrl;

    public ScheduleAndAbsencesByTermAllClassesReportsQueryRepository(UnitOfWork unitOfWork, IOptions<DomainOptions> domainOptions)
        : base(unitOfWork)
    {
        var opts = domainOptions.Value;

        if (string.IsNullOrEmpty(opts.BlobServiceHMACKey))
        {
            throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServiceHMACKey)} should have a configured value");
        }

        if (string.IsNullOrEmpty(opts.BlobServicePublicUrl))
        {
            throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServicePublicUrl)} should have a configured value");
        }

        this.blobServiceHmacKey = opts.BlobServiceHMACKey;
        this.blobServicePublicWebUrl = opts.BlobServicePublicUrl;
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<ScheduleAndAbsencesByTermAllClassesReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.ScheduleAndAbsencesByTermAllClassesReportId,
                r.SchoolYear,
                EnumUtils.GetEnumDescription(r.Term),
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermAllClassesReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<ScheduleAndAbsencesByTermAllClassesReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.ScheduleAndAbsencesByTermAllClassesReportId == scheduleAndAbsencesByTermAllClassesReportId

            select new GetVO(
                r.SchoolYear,
                r.ScheduleAndAbsencesByTermAllClassesReportId,
                r.Term,
                BlobUtils.CreateBlobDownloadUrl(
                    this.blobServiceHmacKey,
                    this.blobServicePublicWebUrl,
                    r.BlobId),
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int scheduleAndAbsencesByTermAllClassesReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<ScheduleAndAbsencesByTermAllClassesReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.ScheduleAndAbsencesByTermAllClassesReportId == scheduleAndAbsencesByTermAllClassesReportId
            select mtr.CreatedBySysUserId
        ).SingleAsync(ct);
    }
}

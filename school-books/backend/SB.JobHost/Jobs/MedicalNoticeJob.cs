namespace SB.JobHost;

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SB.Common;
using SB.Domain;
using static SB.Domain.IHisMedicalNoticesQueryRepository;

public partial class MedicalNoticeJob : PeriodicBackgroundService
{
    [GeneratedRegex("Violation of PRIMARY KEY constraint '(PK_[a-zA-Z_0-9]+)'. Cannot insert duplicate key in object '[a-zA-Z_0-9]+\\.[a-zA-Z_0-9]+'. The duplicate key value is \\(.+\\)\\.")]
    protected static partial Regex PrimaryKeyConstraintRegex();

    [GeneratedRegex("The timeout period elapsed prior to completion of the operation or the server is not responding")]
    protected static partial Regex TimeoutRegex();

    private readonly ILogger<MedicalNoticeJob> logger;
    private IServiceScopeFactory scopeFactory;
    private int batchSize;

    public MedicalNoticeJob(
        ILogger<MedicalNoticeJob> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<JobHostOptions> optionsAccessor)
        : base(optionsAccessor.Value.MedicalNoticeJobOptions.BackoffPeriod)
    {
        this.logger = logger;
        this.scopeFactory = scopeFactory;
        this.batchSize = optionsAccessor.Value.MedicalNoticeJobOptions.BatchSize;
    }

    protected override async Task<bool> ExecuteAsync(CancellationToken stoppingToken, CancellationToken stopToken)
    {
        var ct = stoppingToken;
        NeispuoMedicalNoticeVO[] hisMedicalNotices;
        bool hasMore;
        try
        {
            using var scope = this.scopeFactory.CreateScope();

            var hisMedicalNoticesQueryRepository =
                scope.ServiceProvider.GetRequiredService<IHisMedicalNoticesQueryRepository>();

            (hisMedicalNotices, hasMore) =
                await hisMedicalNoticesQueryRepository.GetNeispuoNextWithReadReceiptsAndSaveAsync(
                    this.batchSize,
                    ct);
        }
        catch (DomainUpdateSqlException ex)
        {
            var sqlErr = ex.SqlException.AsKnownSqlError();

            bool duplicated =
                sqlErr.Type == KnownSqlErrorType.UniqueKeyConstraintError &&
                sqlErr.ConstraintOrIndexName == "PK_HisMedicalNoticeReadReceipt";

            bool timeout =
                sqlErr.Type == KnownSqlErrorType.TimeoutError;

            if (ct.IsCancellationRequested || duplicated || timeout)
            {
                // Ignore cancellations.
                // This can naturally occur when the old version is
                // shutting down and the new version is starting up

                // Ignore duplicate HisMedicalNoticeReadReceipt errors.
                // This can naturally occur when the old version is
                // shutting down and the new version is starting up

                // Ignore timeout errors.
                // This can naturally occur when the database is under heavy load.

                if (!ct.IsCancellationRequested)
                {
                    this.logger.LogWarning($"{nameof(IHisMedicalNoticesQueryRepository.GetNeispuoNextWithReadReceiptsAndSaveAsync)} error occurred - {{error}}",
                        duplicated
                            ? "Cannot insert duplicate key in HisMedicalNoticeReadReceipt"
                            : "Timeout");
                }

                //backoff to try and alleviate the problem
                return true;
            }

            this.logger.LogError(ex, "Error while fetching medical notices");

            //backoff to try and alleviate the problem
            return true;
        }

        if (hisMedicalNotices.Length == 0)
        {
            return true;
        }

        int succeededCount = 0;
        foreach (var hmn in hisMedicalNotices)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }

            try
            {
                using var scope = this.scopeFactory.CreateScope();

                var unitOfWork =
                    scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var absencesAggregateRepository =
                    scope.ServiceProvider.GetRequiredService<IAbsencesAggregateRepository>();
                var attendancesAggregateRepository =
                    scope.ServiceProvider.GetRequiredService<IAttendancesAggregateRepository>();
                var personMedicalNoticeAggregateRepository =
                    scope.ServiceProvider.GetRequiredService<IPersonMedicalNoticeAggregateRepository>();
                var hisMedicalNoticesQueryRepository =
                    scope.ServiceProvider.GetRequiredService<IHisMedicalNoticesQueryRepository>();

                var absences = await absencesAggregateRepository.FindAllUnexcusedForPeriodForInstWithoutExtProviderAsync(
                    hmn.SchoolYears,
                    hmn.PersonId,
                    hmn.FromDate,
                    hmn.ToDate,
                    ct);

                foreach (var absence in absences)
                {
                    absence.ExcuseWithHisMedicalNotice(
                        hmn.HisMedicalNoticeId,
                        hmn.NrnMedicalNotice,
                        hmn.Pmi,
                        hmn.AuthoredOn,
                        hmn.FromDate,
                        hmn.ToDate);
                }

                var attendances = await attendancesAggregateRepository.FindAllUnexcusedForPeriodForInstWithoutExtProviderAsync(
                    hmn.SchoolYears,
                    hmn.PersonId,
                    hmn.FromDate,
                    hmn.ToDate,
                    ct);

                foreach (var attendance in attendances)
                {
                    attendance.ExcuseWithHisMedicalNotice(
                        hmn.HisMedicalNoticeId,
                        hmn.NrnMedicalNotice,
                        hmn.Pmi,
                        hmn.AuthoredOn,
                        hmn.FromDate,
                        hmn.ToDate);
                }

                foreach (int schoolYear in hmn.SchoolYears)
                {
                    PersonMedicalNotice pmn = new(schoolYear, hmn.PersonId, hmn.HisMedicalNoticeId);
                    await personMedicalNoticeAggregateRepository.AddAsync(pmn, ct);
                }

                await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

                int acknowledgedCount =
                    await hisMedicalNoticesQueryRepository.ExecuteAcknowledgeNeispuoAsync(
                        hmn.HisMedicalNoticeId,
                        ct);

                if (acknowledgedCount != 1)
                {
                    // Already acknowledged, nothing to do.
                    // This can naturally occur when the old version is
                    // shutting down and the new version is starting up
                    await transaction.RollbackAsync(ct);
                    continue;
                }

                await unitOfWork.SaveAsync(ct);

                await transaction.CommitAsync(ct);

                succeededCount++;
            }
            catch (Exception ex)
            {
                if (ct.IsCancellationRequested)
                {
                    // Ignore cancellations.
                    // This can naturally occur when the old version is
                    // shutting down and the new version is starting up

                    continue;
                }

                this.logger.LogError(ex, "Error while handling medical notice {HisMedicalNoticeId}", hmn.HisMedicalNoticeId);
            }
        }

        return !hasMore ||
            // Backof in case of failure of all fetched notices.
            // This may be a sign of a more serious problem and lets
            // not hammer the database too much.
            (hisMedicalNotices.Length > 0 && succeededCount == 0);
    }
}

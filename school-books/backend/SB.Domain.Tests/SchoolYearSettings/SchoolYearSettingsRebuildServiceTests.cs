namespace SB.Domain.Tests.SchoolYearSettings;

using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Medallion.Threading;
using System.Data;
using System.Data.Common;

#pragma warning disable CA2012 // Use ValueTasks correctly

public class SchoolYearSettingsRebuildServiceTests
{
    const int schoolYear = 2021;
    const int instId = 300125;

    [Fact]
    public async Task When_TryAcquireAsync_returns_null_RebuildAndSaveAsync_should_throw_DomainValidationException()
    {
        // Setup
        ITransaction transaction = Mock.Of<ITransaction>(m => m.GetDbTransaction() == Mock.Of<DbTransaction>());
        Func<string, IDbTransaction, bool, IDistributedLock> nullLockFactory =
            (_, _, _) =>
                Mock.Of<IDistributedLock>(m =>
                    m.TryAcquireAsync(It.IsAny<TimeSpan>(), default)
                        == ValueTask.FromResult<IDistributedSynchronizationHandle?>(null));

        SchoolYearSettingsRebuildService schoolYearSettingsRebuildService =
            new(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<ISchoolYearSettingsQueryRepository>(),
                Mock.Of<IClassBookSchoolYearSettingsAggregateRepository>(),
                Mock.Of<ISchoolYearSettingsProvider>(),
                Mock.Of<IClassBookCachedQueryStore>(),
                nullLockFactory);

        // Act
        var ex = await Record.ExceptionAsync(async () =>
        {
            await schoolYearSettingsRebuildService.RebuildAndSaveAsync(schoolYear, instId, transaction, default);
        });

        // Verify
        Assert.NotNull(ex);
        Assert.IsType<DomainValidationException>(ex);
        Assert.Equal(new [] { "lock_acquire_failed" }, ((DomainValidationException)ex).Errors);
    }
}

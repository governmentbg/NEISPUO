namespace SB.Domain.Tests.OffDays;

using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Medallion.Threading;
using System.Data;
using System.Data.Common;
using static SB.Domain.IOffDaysQueryRepository;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

#pragma warning disable CA2012 // Use ValueTasks correctly

public class OffDayRebuildServiceTests
{
    const int schoolYear = 2021;
    const int instId = 300125;

    private static (
        IUnitOfWork unitOfWork,
        IOffDaysQueryRepository offDaysQueryRepository,
        IClassBookOffDayDatesAggregateRepository classBookOffDayDatesAggregateRepository,
        ISchedulesAggregateRepository schedulesAggregateRepository,
        Func<string, IDbTransaction, bool, IDistributedLock> lockFactory,
        ITransaction transaction
    ) GetStandartDependencies()
        => (
            unitOfWork: Mock.Of<IUnitOfWork>(),
            offDaysQueryRepository: Mock.Of<IOffDaysQueryRepository>(),
            classBookOffDayDatesAggregateRepository: Mock.Of<IClassBookOffDayDatesAggregateRepository>(),
            schedulesAggregateRepository: Mock.Of<ISchedulesAggregateRepository>(),
            lockFactory:
                (_, _, _) =>
                    Mock.Of<IDistributedLock>(m =>
                        m.TryAcquireAsync(It.IsAny<TimeSpan>(), default)
                            == ValueTask.FromResult<IDistributedSynchronizationHandle?>(Mock.Of<IDistributedSynchronizationHandle>())),
            transaction: Mock.Of<ITransaction>(m => m.GetDbTransaction() == Mock.Of<DbTransaction>())
        );

    [Fact]
    public async Task When_TryAcquireAsync_returns_null_RebuildAndSaveAsync_should_throw_DomainValidationException()
    {
        // Setup
        var (
            unitOfWork,
            offDaysQueryRepository,
            schedulesAggregateRepository,
            classBookOffDayDatesAggregateRepository,
            _,
            transaction
        ) = GetStandartDependencies();

        Func<string, IDbTransaction, bool, IDistributedLock> nullLockFactory =
            (_, _, _) =>
                Mock.Of<IDistributedLock>(m =>
                    m.TryAcquireAsync(It.IsAny<TimeSpan>(), default)
                        == ValueTask.FromResult<IDistributedSynchronizationHandle?>(null));

        OffDayRebuildService offDayRebuildService =
            new(unitOfWork,
                offDaysQueryRepository,
                schedulesAggregateRepository,
                classBookOffDayDatesAggregateRepository,
                nullLockFactory);

        // Act
        var ex = await Record.ExceptionAsync(async () =>
        {
            await offDayRebuildService.RebuildAndSaveAsync(schoolYear, instId, null, transaction, default);
        });

        // Verify
        Assert.NotNull(ex);
        Assert.IsType<DomainValidationException>(ex);
        Assert.Equal(new [] { "lock_acquire_failed" }, ((DomainValidationException)ex).Errors);
    }

    public static IEnumerable<object[]> GetIntersectionTestData()
    {
        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "Descr1", new DateTime(2021, 09, 15), new DateTime(2021, 09, 16), true, false, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, "Descr2", new DateTime(2021, 09, 16), new DateTime(2021, 09, 16), true, false, Array.Empty<int>(), Array.Empty<int>())
            },
            new[] { "The OffDays 1001 and 1002 are intersecting for the period 16.09.2021-16.09.2021." },
            new[] { "Неучебните дни Descr1 и Descr2 се засичат за датата 16.09.2021." }
        };

        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "Descr1", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), true, false, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, "Descr2", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), true, false, Array.Empty<int>(), Array.Empty<int>())
            },
            new[] { "The OffDays 1001 and 1002 are intersecting for the period 15.09.2021-17.09.2021." },
            new[] { "Неучебните дни Descr1 и Descr2 се засичат за дните 15.09.2021-17.09.2021." }
        };

        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "Descr1", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), false, false, new[] { 1, 2, 3 }, Array.Empty<int>()),
                new(1002, "Descr2", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), false, false, new[] { 2, 3, 4 }, Array.Empty<int>())
            },
            new[] { "The OffDays 1001 and 1002 have intersecting BasicClassIds for the period 15.09.2021-17.09.2021." },
            new[] { "Неучебните дни Descr1 и Descr2 се засичат за дните 15.09.2021-17.09.2021 за випуските втори клас и трети клас." }
        };

        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "Descr1", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), false, false, Array.Empty<int>(), new[] { 1001, 1002, 1003 }),
                new(1002, "Descr2", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), false, false, Array.Empty<int>(), new[] { 1002, 1003, 1004 })
            },
            new[] { "The OffDays 1001 and 1002 have intersecting ClassBookIds for the period 15.09.2021-17.09.2021." },
            new[] { "Неучебните дни Descr1 и Descr2 се засичат за дните 15.09.2021-17.09.2021 за групите/паралелките 1 - б и 1 - в." }
        };

        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "Descr1", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), true, false, new[] { 1, 2, 3 }, Array.Empty<int>())
            },
            new[] { "OffDays with IsForAllClasses=True should not have BasicClassIds or ClassBookIds." },
            Array.Empty<string>()
        };

        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "Descr1", new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), false, false, new[] { 1, 2, 3 }, new[] { 1001, 1002, 1003 })
            },
            new[] { "OffDays with IsForAllClasses=False should not have both BasicClassIds and ClassBookIds." },
            Array.Empty<string>()
        };
    }

    [Theory, MemberData(nameof(GetIntersectionTestData))]
    public async Task RebuildAndSaveAsync_should_throw_DomainValidationException_if_offdays_are_intersecting(
        GetAllForRebuildVO[] offDays,
        string[] errors,
        string[] errorMessages)
    {
        // Setup
        var (
            unitOfWork,
            _,
            classBookOffDayDatesAggregateRepository,
            schedulesAggregateRepository,
            lockFactory,
            transaction
        ) = GetStandartDependencies();

        IOffDaysQueryRepository offDaysQueryRepository = Mock.Of<IOffDaysQueryRepository>(m =>
            m.GetAllBasicClassNamesAsync(default)
                == Task.FromResult(
                    new GetAllBasicClassNamesVO[]
                    {
                        new(1, "първи клас"),
                        new(2, "втори клас"),
                        new(3, "трети клас"),
                    }
                ) &&
            m.GetAllClassBookNamesAsync(instId, default)
                == Task.FromResult(
                    new GetAllClassBookNamesVO[]
                    {
                        new(1001, "1 - а"),
                        new(1002, "1 - б"),
                        new(1003, "1 - в"),
                    }
                ) &&
            m.GetAllForRebuildAsync(schoolYear, instId, default)
                == Task.FromResult(offDays));

        OffDayRebuildService offDayRebuildService =
            new(unitOfWork,
                offDaysQueryRepository,
                classBookOffDayDatesAggregateRepository,
                schedulesAggregateRepository,
                lockFactory);

        // Act
        var ex = await Record.ExceptionAsync(async () =>
        {
            await offDayRebuildService.RebuildAndSaveAsync(schoolYear, instId, null, transaction, default);
        });

        // Verify
        Assert.NotNull(ex);
        Assert.IsType<DomainValidationException>(ex);
        Assert.Equal(errors, ((DomainValidationException)ex).Errors);
        Assert.Equal(errorMessages, ((DomainValidationException)ex).ErrorMessages);
    }

    public static IEnumerable<object[]> GetCreationTestData()
    {
        // two new OffDays(OD1, OD2) are created at the same time, an unlikely scenario but still should work
        yield return new object[]
        {
            new GetAllForRebuildVO[]
            {
                new(1001, "OD1", new DateTime(2021, 09, 16), new DateTime(2021, 09, 17), true, false, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, "OD2", new DateTime(2021, 09, 17), new DateTime(2021, 09, 17), false, false, new[] { 1 }, Array.Empty<int>())
            },
            new GetAllClassBooksVO[]
            {
                new(schoolYear, 2001, 1),
                new(schoolYear, 2002, 2),
            },
            Array.Empty<ClassBookOffDayDate>(),
            new ClassBookOffDayDate[]
            {
                new ClassBookOffDayDate(schoolYear, 2001, new DateTime(2021, 09, 16), 1001, false),
                new ClassBookOffDayDate(schoolYear, 2001, new DateTime(2021, 09, 17), 1002, false),
                new ClassBookOffDayDate(schoolYear, 2002, new DateTime(2021, 09, 16), 1001, false),
                new ClassBookOffDayDate(schoolYear, 2002, new DateTime(2021, 09, 17), 1001, false),
            },
            Array.Empty<ClassBookOffDayDate>(),
            Array.Empty<ClassBookOffDayDate>()
        };

        yield return new object[]
        {
            // a new OffDay OD2 is added to the existing OD1
            new GetAllForRebuildVO[]
            {
                new(1001, "OD1", new DateTime(2021, 09, 16), new DateTime(2021, 09, 17), true, false, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, "OD2", new DateTime(2021, 09, 17), new DateTime(2021, 09, 18), false, false, new[] { 1 }, Array.Empty<int>())
            },
            new GetAllClassBooksVO[]
            {
                new(schoolYear, 2001, 1),
                new(schoolYear, 2002, 2),
            },
            new ClassBookOffDayDate[]
            {
                new(schoolYear, 2001, new DateTime(2021, 09, 16), 1001, false),
                new(schoolYear, 2001, new DateTime(2021, 09, 17), 1001, false),
                new(schoolYear, 2002, new DateTime(2021, 09, 16), 1001, false),
                new(schoolYear, 2002, new DateTime(2021, 09, 17), 1001, false),
            },
            new ClassBookOffDayDate[]
            {
                new(schoolYear, 2001, new DateTime(2021, 09, 18), 1002, false),
            },
            new ClassBookOffDayDate[]
            {
                new(schoolYear, 2001, new DateTime(2021, 09, 17), 1002, false),
            },
            Array.Empty<ClassBookOffDayDate>()
        };

        yield return new object[]
        {
            // the OffDay OD2 is being removed
            new GetAllForRebuildVO[]
            {
                new(1001, "OD1", new DateTime(2021, 09, 16), new DateTime(2021, 09, 17), true, false, Array.Empty<int>(), Array.Empty<int>()),
            },
            new GetAllClassBooksVO[]
            {
                new(schoolYear, 2001, 1),
                new(schoolYear, 2002, 2),
            },
            new ClassBookOffDayDate[]
            {
                new(schoolYear, 2001, new DateTime(2021, 09, 16), 1001, false),
                new(schoolYear, 2001, new DateTime(2021, 09, 17), 1002, false),
                new(schoolYear, 2001, new DateTime(2021, 09, 18), 1002, false),
                new(schoolYear, 2002, new DateTime(2021, 09, 16), 1001, false),
                new(schoolYear, 2002, new DateTime(2021, 09, 17), 1001, false),
            },
            Array.Empty<ClassBookOffDayDate>(),
            new ClassBookOffDayDate[]
            {
                new(schoolYear, 2001, new DateTime(2021, 09, 17), 1001, false),
            },
            new ClassBookOffDayDate[]
            {
                new(schoolYear, 2001, new DateTime(2021, 09, 18), 1002, false),
            }
        };
    }

    [Theory, MemberData(nameof(GetCreationTestData))]
    public async Task RebuildAndSaveAsync_should_create_update_delete_ClassBookOffDayDates(
        GetAllForRebuildVO[] offDays,
        GetAllClassBooksVO[] classBooks,
        ClassBookOffDayDate[] existing,
        ClassBookOffDayDate[] created,
        ClassBookOffDayDate[] updated,
        ClassBookOffDayDate[] deleted)
    {
        // Setup
        var (
            unitOfWork,
            _,
            _,
            _,
            lockFactory,
            transaction
        ) = GetStandartDependencies();

        var offDaysQueryRepository = Mock.Of<IOffDaysQueryRepository>(m =>
            m.GetAllForRebuildAsync(schoolYear, instId, default)
                == Task.FromResult(offDays) &&
            m.GetAllClassBooksAsync(schoolYear, instId, default)
                == Task.FromResult(classBooks));

        var classBookOffDayDatesAggregateRepository =
            Mock.Of<IClassBookOffDayDatesAggregateRepository>(m =>
                m.FindAllByInstitutionAsync(schoolYear, instId, default)
                    == Task.FromResult(existing));

        var schedulesAggregateRepository =
            Mock.Of<ISchedulesAggregateRepository>(m =>
                m.RemoveScheduleLessonsForDateAsync(schoolYear, instId, It.IsAny<int>(), It.IsAny<DateTime>(), default) == Task.CompletedTask);

        List<ClassBookOffDayDate> added = new();
        Mock.Get(classBookOffDayDatesAggregateRepository).
            Setup(m => m.AddAsync(It.IsAny<ClassBookOffDayDate>(), true, default))
            .Callback<ClassBookOffDayDate, bool, CancellationToken>((od, _, _) => added.Add(od))
            .Returns(Task.CompletedTask);

        List<ClassBookOffDayDate> removed = new();
        Mock.Get(classBookOffDayDatesAggregateRepository).
            Setup(m => m.Remove(It.IsAny<ClassBookOffDayDate>(), false, true))
            .Callback<ClassBookOffDayDate, bool, bool>((od, _, _) => removed.Add(od));

        OffDayRebuildService offDayRebuildService =
            new(unitOfWork,
                offDaysQueryRepository,
                classBookOffDayDatesAggregateRepository,
                schedulesAggregateRepository,
                lockFactory);

        // save the existing ClassBookOffDayDates state in an immutable way, as well need to diff with it later
        var existingState = existing.Select(od => (od.ClassBookId, od.Date, od.OffDayId)).ToArray();

        // Act
        await offDayRebuildService.RebuildAndSaveAsync(schoolYear, instId, null, transaction, default);

        // Verify
        var comparer = KeyEqualityComparer<ClassBookOffDayDate>.Create(
            od => (
                od.SchoolYear,
                od.ClassBookId,
                od.Date,
                od.OffDayId,
                od.IsPgOffProgramDay));

        AssertUtils.SetsEqualWithoutDuplicates(created, added, comparer);
        AssertUtils.SetsEqualWithoutDuplicates(updated, existing.ExceptBy(existingState, od => (od.ClassBookId, od.Date, od.OffDayId)), comparer);
        AssertUtils.SetsEqualWithoutDuplicates(deleted, removed, comparer);
        Mock.Get(unitOfWork).Verify(uow => uow.SaveAsync(default), Times.Once());
    }
}

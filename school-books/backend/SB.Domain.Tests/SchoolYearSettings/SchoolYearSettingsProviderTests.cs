namespace SB.Domain.Tests.SchoolYearSettings;

using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using static SB.Domain.ISchoolYearSettingsQueryRepository;
using static SB.Domain.ISchoolYearSettingsProvider;

public class SchoolYearSettingsProviderTests
{
    const int schoolYear = 2021;
    const int instId = 300125;
    const int classBookId = 5000;

    private static (
        ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository,
        ILogger<SchoolYearSettingsProvider> logger
    ) GetStandartDependencies()
        => (
            schoolYearSettingsQueryRepository:
                Mock.Of<ISchoolYearSettingsQueryRepository>(
                    m => m.GetDefaultAsync(schoolYear, default)
                        == Task.FromResult(
                            new GetDefaultVO(
                                // PG
                                new DateTime(2021, 09, 15),
                                new DateTime(2021, 09, 15),
                                new DateTime(2022, 01, 31),
                                new DateTime(2022, 02, 01),
                                new DateTime(2022, 05, 31),
                                new DateTime(2022, 09, 14),
                                // Sport
                                new DateTime(2021, 09, 01),
                                new DateTime(2021, 09, 01),
                                new DateTime(2022, 01, 31),
                                new DateTime(2022, 02, 01),
                                new DateTime(2022, 06, 30),
                                new DateTime(2022, 08, 31),
                                // Cplr
                                new DateTime(2021, 10, 01),
                                new DateTime(2021, 10, 01),
                                new DateTime(2022, 01, 31),
                                new DateTime(2022, 02, 01),
                                new DateTime(2022, 06, 30),
                                new DateTime(2022, 09, 30),
                                // Other
                                new DateTime(2021, 09, 15),
                                new DateTime(2021, 09, 15),
                                new DateTime(2022, 01, 31),
                                new DateTime(2022, 02, 01),
                                new DateTime(2022, 06, 30),
                                new DateTime(2022, 09, 14))) &&
                        m.GetAllForRebuildAsync(schoolYear, instId, default)
                            == Task.FromResult(Array.Empty<GetAllForRebuildVO>())),
            logger: new NullLogger<SchoolYearSettingsProvider>());

    [Fact]
    public async Task When_multiple_IsForAllClasses_should_throw_DomainValidationException()
    {
        // Setup
        var (
            _,
            logger
        ) = GetStandartDependencies();

        var schoolYearSettingsQueryRepository = Mock.Of<ISchoolYearSettingsQueryRepository>(m =>
            m.GetAllForRebuildAsync(schoolYear, instId, default)
                == Task.FromResult(new GetAllForRebuildVO[]
                {
                    new(1001, new DateTime(2021, 09, 15), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                    new(1002, new DateTime(2021, 09, 15), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                }));

        SchoolYearSettingsProvider schoolYearSettingsProvider = new(schoolYearSettingsQueryRepository, logger);

        // Act
        var ex = await Record.ExceptionAsync(async () =>
        {
            await schoolYearSettingsProvider.GetForClassBookAsync(schoolYear, instId, classBookId, 1, Array.Empty<int?>(), default);
        });

        // Verify
        Assert.NotNull(ex);
        Assert.IsType<DomainValidationException>(ex);
        Assert.Equal(new [] { "There should be at maximum 1 SchoolYearSettings with IsForAllClasses=True" }, ((DomainValidationException)ex).Errors);
    }

    [Fact]
    public async Task When_mismatching_child_basicClassIds_should_throw_DomainValidationException()
    {
        // Setup
        var (
            schoolYearSettingsQueryRepository,
            logger
        ) = GetStandartDependencies();

        int? basicClassId = null;
        int?[] childBasicClassIds = new int?[] { -6, 1 };

        SchoolYearSettingsProvider schoolYearSettingsProvider = new(schoolYearSettingsQueryRepository, logger);

        // Act
        var ex = await Record.ExceptionAsync(async () =>
        {
            await schoolYearSettingsProvider.GetForClassBookAsync(schoolYear, instId, classBookId, basicClassId, childBasicClassIds, default);
        });

        // Verify
        Assert.NotNull(ex);
        Assert.IsType<DomainValidationException>(ex);
        Assert.Equal(new [] { "Child BasicClassIds missmatch!" }, ((DomainValidationException)ex).Errors);
    }

    [Fact]
    public async Task Should_cache_queries()
    {
        // Setup
        var (
            schoolYearSettingsQueryRepository,
            logger
        ) = GetStandartDependencies();

        SchoolYearSettingsProvider schoolYearSettingsProvider = new(schoolYearSettingsQueryRepository, logger);

        // Act
        await schoolYearSettingsProvider.GetForClassBookAsync(schoolYear, instId, classBookId, 1, Array.Empty<int?>(), default);
        await schoolYearSettingsProvider.GetForClassBookAsync(schoolYear, instId, classBookId, 2, Array.Empty<int?>(), default);

        // Verify
        Mock.Get(schoolYearSettingsQueryRepository).Verify(repo => repo.GetAllForRebuildAsync(schoolYear, instId, default), Times.Once());
        Mock.Get(schoolYearSettingsQueryRepository).Verify(repo => repo.IsSportSchoolAsync(schoolYear, instId, default), Times.Once());
        Mock.Get(schoolYearSettingsQueryRepository).Verify(repo => repo.GetDefaultAsync(schoolYear, default), Times.Once());
    }

    public static IEnumerable<object?[]> GetCreationTestData()
    {
        // the all classes SchoolYearSettings is used
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, false, new int[] { 2, 3 }, Array.Empty<int>()),
            },
            1,
            Array.Empty<int?>(),
            new GetForClassBookVO(1001, new DateTime(2021, 09, 15), new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), new DateTime(2022, 09, 14), false, null)
        };

        // the defaults for PG are used with all classes SchoolYearSettings with custom first term end and second term start dates
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, false, new int[] { 2, 3 }, Array.Empty<int>()),
            },
            -6,
            Array.Empty<int?>(),
            new GetForClassBookVO(1001, new DateTime(2021, 09, 15), new DateTime(2021, 09, 15), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 31), new DateTime(2022, 09, 14), false, null)
        };

        // the defaults for SportSchool are used
        yield return new object?[]
        {
            true,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, false, new int[] { 2, 3 }, Array.Empty<int>()),
            },
            6,
            Array.Empty<int?>(),
            new GetForClassBookVO(null, new DateTime(2021, 09, 01), new DateTime(2021, 09, 01), new DateTime(2022, 01, 31), new DateTime(2022, 02, 01), new DateTime(2022, 06, 30), new DateTime(2022, 08, 31), false, null)
        };

        // the defaults for Other schools are used
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, false, new int[] { 2, 3 }, Array.Empty<int>()),
            },
            6,
            Array.Empty<int?>(),
            new GetForClassBookVO(null, new DateTime(2021, 09, 15), new DateTime(2021, 09, 15), new DateTime(2022, 01, 31), new DateTime(2022, 02, 01), new DateTime(2022, 06, 30), new DateTime(2022, 09, 14), false, null)
        };

        // the Lvl2 basicClassId is matched with a SchoolYearSettings
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, false, new int[] { 1, 2, 3 }, Array.Empty<int>()),
            },
            1,
            new int?[] { 1, 2 },
            new GetForClassBookVO(1002, new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), new DateTime(2022, 09, 14), false, null)
        };

        // the Lvl2 basicClassId is matched with a SchoolYearSettings with the latest EndDate
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 30), false, null, false, new int[] { 1, 2, 3 }, Array.Empty<int>()),
                new(1003, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 31), false, null, false, new int[] { 1, }, Array.Empty<int>()),
            },
            1,
            new int?[] { 1, 2 },
            new GetForClassBookVO(1003, new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 31), new DateTime(2022, 09, 14), false, null)
        };

        // the child basicClassId is matched with a SchoolYearSettings with the latest EndDate
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 30), false, null, false, new int[] { 1, 2 }, Array.Empty<int>()),
                new(1003, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 31), false, null, false, new int[] { 2, 3 }, Array.Empty<int>()),
            },
            null,
            new int?[] { 1, 2 },
            new GetForClassBookVO(1003, new DateTime(2021, 09, 15), new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 31), new DateTime(2022, 09, 14), false, null)
        };

        // the classBookId is matched with a SchoolYearSettings with the latest EndDate
        yield return new object?[]
        {
            false,
            new GetAllForRebuildVO[]
            {
                new(1001, new DateTime(2021, 09, 16), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 06, 30), false, null, true, Array.Empty<int>(), Array.Empty<int>()),
                new(1002, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 30), false, null, false, new int[] { 1, 2, 3 }, Array.Empty<int>()),
                new(1003, new DateTime(2021, 09, 17), new DateTime(2022, 02, 05), new DateTime(2022, 02, 07), new DateTime(2022, 05, 31), false, null, false, new int[] { 1, }, Array.Empty<int>()),
                new(1004, new DateTime(2021, 09, 18), new DateTime(2022, 02, 06), new DateTime(2022, 02, 08), new DateTime(2022, 05, 29), false, null, false, Array.Empty<int>(), new int[] { 5000, 5001, 5002 }),
                new(1005, new DateTime(2021, 09, 19), new DateTime(2022, 02, 07), new DateTime(2022, 02, 09), new DateTime(2022, 05, 30), false, null, false, Array.Empty<int>(), new int[] { 5000, }),
            },
            1,
            new int?[] { 1, 2 },
            new GetForClassBookVO(1005, new DateTime(2021, 09, 15), new DateTime(2021, 09, 19), new DateTime(2022, 02, 07), new DateTime(2022, 02, 09), new DateTime(2022, 05, 30), new DateTime(2022, 09, 14), false, null)
        };
    }

    [Theory, MemberData(nameof(GetCreationTestData))]
    public async Task Should_determine_dates(
        bool isSportSchool,
        GetAllForRebuildVO[] schoolYearSettings,
        int? basicClassId,
        int?[] childBasicClassIds,
        GetForClassBookVO expected)
    {
        // Setup
        var (
            schoolYearSettingsQueryRepository,
            logger
        ) = GetStandartDependencies();

        Mock.Get(schoolYearSettingsQueryRepository).
            Setup(m => m.GetAllForRebuildAsync(schoolYear, instId, default))
            .Returns(Task.FromResult(schoolYearSettings));

        Mock.Get(schoolYearSettingsQueryRepository).
            Setup(m => m.IsSportSchoolAsync(schoolYear, instId, default))
            .Returns(Task.FromResult(isSportSchool));

        SchoolYearSettingsProvider schoolYearSettingsProvider = new(schoolYearSettingsQueryRepository, logger);

        // Act
        var result = await schoolYearSettingsProvider.GetForClassBookAsync(
            schoolYear,
            instId,
            classBookId,
            basicClassId,
            childBasicClassIds,
            default);

        // Verify
        Assert.Equal(expected, result);
    }
}

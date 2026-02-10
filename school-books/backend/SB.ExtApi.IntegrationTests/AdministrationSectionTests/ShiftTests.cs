namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class ShiftTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly ShiftDO shift =
        new()
        {
            Name = "Първа смяна тест",
            Hours = new ShiftHourDO[]
            {
                new()
                {
                    HourNumber = 1,
                    StartTime = "07:30",
                    EndTime = "08:10"
                },
                new()
                {
                    HourNumber = 2,
                    StartTime = "08:20",
                    EndTime = "09:00"
                },
                new()
                {
                    HourNumber = 3,
                    StartTime = "09:20",
                    EndTime = "10:00"
                },
                new()
                {
                    HourNumber = 4,
                    StartTime = "10:10",
                    EndTime = "10:50"
                },
                new()
                {
                    HourNumber = 5,
                    StartTime = "11:00",
                    EndTime = "11:40"
                },
                new()
                {
                    HourNumber = 6,
                    StartTime = "11:50",
                    EndTime = "12:30"
                },
                new()
                {
                    HourNumber = 7,
                    StartTime = "12:35",
                    EndTime = "13:15"
                },
            },
        };
    private readonly ExtApiWebApplicationFactory appFactory;

    public ShiftTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_V_XII data = fixtures.Values.Item2;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
    }

    [Fact]
    public async Task GetAll_returns_all_shifts_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsGetAsync(this.schoolYear, this.institutionId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_shift()
    {
        var shiftId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsPostAsync(this.schoolYear, this.institutionId, this.shift);
        Assert.InRange(shiftId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_shift()
    {
        var shiftId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsPostAsync(this.schoolYear, this.institutionId, this.shift);

        var shiftUpdateData =
            new ShiftDO
            {
                Name = "Първа смяна тест Update",
                Hours = new ShiftHourDO[]
                {
                    new()
                    {
                        HourNumber = 2,
                        StartTime = "08:20",
                        EndTime = "09:00"
                    },
                    new()
                    {
                        HourNumber = 3,
                        StartTime = "09:20",
                        EndTime = "10:00"
                    },
                    new()
                    {
                        HourNumber = 4,
                        StartTime = "10:10",
                        EndTime = "10:50"
                    },
                    new()
                    {
                        HourNumber = 5,
                        StartTime = "11:20",
                        EndTime = "12:00"
                    }
                },
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsPutAsync(this.schoolYear, this.institutionId, shiftId, shiftUpdateData);

        var shiftUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsGetAsync(this.schoolYear, this.institutionId))
           .Where(e => e.ShiftId == shiftId)
           .SingleOrDefault();

        Assert.NotNull(shiftUpdated);
        Assert.Equal("Първа смяна тест Update", shiftUpdated.Name);
        Assert.Equal(4, shiftUpdated.Hours.Count);
        Assert.Equal(2, shiftUpdated.Hours.First().HourNumber);
        Assert.Equal("11:20", shiftUpdated.Hours.Last().StartTime);
        Assert.Equal("12:00", shiftUpdated.Hours.Last().EndTime);
    }

    [Fact]
    public async Task Should_remove_classbook_shift()
    {
        var shiftId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsPostAsync(this.schoolYear, this.institutionId, this.shift);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsDeleteAsync(this.schoolYear, this.institutionId, shiftId);

        var shiftDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsGetAsync(this.schoolYear, this.institutionId))
            .Where(s => s.ShiftId == shiftId)
            .SingleOrDefault();

        Assert.Null(shiftDeleted);
    }

    [Fact]
    public async Task Should_create_classbook_shift_with_overlapping_start_end_time()
    {
        var shift = new ShiftDO
        {
            Name = "Първа смяна тест",
            Hours = new ShiftHourDO[]
            {
                new()
                {
                    HourNumber = 1,
                    StartTime = "07:30",
                    EndTime = "08:30"
                },
                new()
                {
                    HourNumber = 2,
                    StartTime = "08:00",
                    EndTime = "09:00"
                },
                new()
                {
                    HourNumber = 3,
                    StartTime = "00:00",
                    EndTime = "00:00"
                },
                new()
                {
                    HourNumber = 4,
                    StartTime = "23:30",
                    EndTime = "00:30"
                }
            },
        };

        var shiftId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsPostAsync(this.schoolYear, this.institutionId, shift);
        Assert.InRange(shiftId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_create_multiday_shift()
    {
        var shift = new ShiftDO
        {
            Name = "Първа смяна тест",
            IsMultiday = true,
            Hours = new ShiftHourDO[]
            {
                new()
                {
                    Day = 1,
                    HourNumber = 1,
                    StartTime = "07:30",
                    EndTime = "08:30"
                },
                new()
                {
                    Day = 1,
                    HourNumber = 2,
                    StartTime = "08:00",
                    EndTime = "09:00"
                },
                new()
                {
                    Day = 2,
                    HourNumber = 1,
                    StartTime = "08:30",
                    EndTime = "09:30"
                },
                new()
                {
                    Day = 2,
                    HourNumber = 2,
                    StartTime = "09:30",
                    EndTime = "10:30"
                }
            },
        };

        var shiftId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsPostAsync(this.schoolYear, this.institutionId, shift);
        Assert.InRange(shiftId, 1, int.MaxValue);

        var createdShift = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ShiftsGetAsync(this.schoolYear, this.institutionId))
           .Where(e => e.ShiftId == shiftId)
           .Single();

        Assert.Equal(shift.Hours.Select(h => h.Day), createdShift.Hours.Select(h => h.Day));
        Assert.Equal(shift.Hours.Select(h => h.HourNumber), createdShift.Hours.Select(h => h.HourNumber));
        Assert.Equal(shift.Hours.Select(h => h.StartTime), createdShift.Hours.Select(h => h.StartTime));
        Assert.Equal(shift.Hours.Select(h => h.EndTime), createdShift.Hours.Select(h => h.EndTime));
    }
}

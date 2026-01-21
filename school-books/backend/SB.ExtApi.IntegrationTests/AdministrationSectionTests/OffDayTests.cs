namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class OffDayTests : RestoreSnapshotFixture
{
    private static readonly DateTime date = new(2023, 4, 6);
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly ExtApiWebApplicationFactory appFactory;

    private readonly OffDayDO[] offDays;

    public OffDayTests(
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

        // we cant use a MemberData here because we need to use the data.ClassBookId
        this.offDays =
            new OffDayDO[]
            {
                new()
                {
                    From = date,
                    To = date.AddDays(1),
                    Description = "Description",
                    IsForAllClasses = false,
                    BasicClassIds = new int[] { 1 }
                },
                new()
                {
                    From = date,
                    To = date.AddDays(1),
                    Description = "Description",
                    IsForAllClasses = false,
                    ClassBookIds = new int[] { data.ClassBookId }
                },
            };
    }

    [Theory, InlineData(0), InlineData(1)]
    public async Task GetAll_returns_all_offDays_successfully(int offDayIndex)
    {
        var offDay = this.offDays[offDayIndex];

        // Setup
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysPostAsync(this.schoolYear, this.institutionId, offDay);

        // Act
        var offDays = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysGetAsync(this.schoolYear, this.institutionId);

        // Verify
        var comparer = KeyEqualityComparer<OffDayDO>.Create(
            od => (
                od.From,
                od.To,
                od.Description,
                od.IsForAllClasses,
                od.BasicClassIds.Aggregate(
                    new HashCode(),
                    (hc, bcId) =>
                    {
                        hc.Add(bcId);
                        return hc;
                    },
                    hc => hc.ToHashCode()),
                od.ClassBookIds.Aggregate(
                    new HashCode(),
                    (hc, cbId) =>
                    {
                        hc.Add(cbId);
                        return hc;
                    },
                    hc => hc.ToHashCode())
            ));

        Assert.Equal(new[] { offDay }, offDays, comparer);
    }

    [Theory, InlineData(0), InlineData(1)]
    public async Task Should_create_classbook_offDay(int offDayIndex)
    {
        var offDayId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysPostAsync(this.schoolYear, this.institutionId, this.offDays[offDayIndex]);
        Assert.InRange(offDayId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_offDay()
    {
        var offDayId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysPostAsync(this.schoolYear, this.institutionId,
            new OffDayDO
            {
                From = date,
                To = date.AddDays(1),
                Description = "Description",
                IsForAllClasses = false,
                BasicClassIds = new int[] { 6 }
            });

        var offDayUpdateData =
            new OffDayDO
            {
                From = date.AddDays(7),
                To = date.AddDays(8),
                Description = "DescriptionUpdate",
                IsForAllClasses = false,
                BasicClassIds = new int[] { 7 }
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysPutAsync(this.schoolYear, this.institutionId, offDayId, offDayUpdateData);

        var offDayUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysGetAsync(this.schoolYear, this.institutionId))
           .Where(e => e.OffDayId == offDayId)
           .SingleOrDefault();

        Assert.NotNull(offDayUpdated);
        Assert.Equal(date.AddDays(7), offDayUpdated.From);
        Assert.Equal(date.AddDays(8), offDayUpdated.To);
        Assert.Equal("DescriptionUpdate", offDayUpdated.Description);
        Assert.Equal(1, offDayUpdated.BasicClassIds.Count);
        Assert.False(offDayUpdated.IsForAllClasses);
    }

    [Theory, InlineData(0), InlineData(1)]
    public async Task Should_remove_classbook_offDay(int offDayIndex)
    {
        var offDayId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysPostAsync(this.schoolYear, this.institutionId, this.offDays[offDayIndex]);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysDeleteAsync(this.schoolYear, this.institutionId, offDayId);

        var offDayDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).OffDaysGetAsync(this.schoolYear, this.institutionId))
            .Where(od => od.OffDayId == offDayId)
            .SingleOrDefault();

        Assert.Null(offDayDeleted);
    }
}

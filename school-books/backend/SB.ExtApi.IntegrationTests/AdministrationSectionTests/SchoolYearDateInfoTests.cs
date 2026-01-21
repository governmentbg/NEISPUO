namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class SchoolYearDateInfoTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly ExtApiWebApplicationFactory appFactory;

    private readonly SchoolYearDateInfoDO[] schoolYearDateInfos;

    public SchoolYearDateInfoTests(
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
        this.schoolYearDateInfos =
            new SchoolYearDateInfoDO[]
            {
                new()
                {
                    SchoolYearStartDate = new DateTime(2025, 09, 17),
                    FirstTermEndDate = new DateTime(2026, 01, 31),
                    SecondTermStartDate = new DateTime(2026, 02, 05),
                    SchoolYearEndDate = new DateTime(2026, 06, 15),
                    Description = "Description",
                    IsForAllClasses = false,
                    BasicClassIds = new int[] { 1 }
                },
                new()
                {
                    SchoolYearStartDate = new DateTime(2025, 09, 17),
                    FirstTermEndDate = new DateTime(2026, 01, 31),
                    SecondTermStartDate = new DateTime(2026, 02, 05),
                    SchoolYearEndDate = new DateTime(2026, 06, 15),
                    Description = "Description1",
                    IsForAllClasses = false,
                    ClassBookIds = new int[] { data.ClassBookId }
                },
                new()
                {
                    SchoolYearStartDate = new DateTime(2025, 09, 17),
                    FirstTermEndDate = new DateTime(2026, 01, 31),
                    SecondTermStartDate = new DateTime(2026, 02, 05),
                    SchoolYearEndDate = new DateTime(2026, 06, 15),
                    Description = "Description2",
                    IsForAllClasses = true
                },
                new()
                {
                    SchoolYearStartDate = new DateTime(2025, 09, 17),
                    FirstTermEndDate = new DateTime(2026, 01, 31),
                    SecondTermStartDate = new DateTime(2026, 02, 05),
                    SchoolYearEndDate = new DateTime(2026, 06, 15),
                    Description = "Description3",
                    IsForAllClasses = true
                },
            };
    }

    [Theory, InlineData(0), InlineData(1), InlineData(2), InlineData(3)]
    public async Task GetAll_returns_all_schoolYearDateInfos_successfully(int schoolYearDateInfoIndex)
    {
        var schoolYearDateInfo = this.schoolYearDateInfos[schoolYearDateInfoIndex];

        // Setup
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosPostAsync(this.schoolYear, this.institutionId, schoolYearDateInfo);

        // Act
        var schoolYearDateInfos = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosGetAsync(this.schoolYear, this.institutionId);

        // Verify
        var comparer = KeyEqualityComparer<SchoolYearDateInfoDO>.Create(
            od => (
                od.SchoolYearStartDate,
                od.FirstTermEndDate,
                od.SecondTermStartDate,
                od.SchoolYearEndDate,
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

        Assert.Equal(new[] { schoolYearDateInfo }, schoolYearDateInfos, comparer);
    }

    [Theory, InlineData(0), InlineData(1), InlineData(2), InlineData(3)]
    public async Task Should_create_classbook_schoolYearDateInfo(int schoolYearDateInfoIndex)
    {
        var schoolYearDateInfo = this.schoolYearDateInfos[schoolYearDateInfoIndex];
        var schoolYearDateInfoId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosPostAsync(this.schoolYear, this.institutionId, schoolYearDateInfo);
        Assert.InRange(schoolYearDateInfoId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_schoolYearDateInfo()
    {
        var schoolYearDateInfoId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosPostAsync(this.schoolYear, this.institutionId,
            new SchoolYearDateInfoDO
            {
                SchoolYearStartDate = new DateTime(2025, 09, 17),
                FirstTermEndDate = new DateTime(2026, 01, 31),
                SecondTermStartDate = new DateTime(2026, 02, 05),
                SchoolYearEndDate = new DateTime(2026, 06, 15),
                Description = "Description",
                IsForAllClasses = false,
                BasicClassIds = new int[] { 6 }
            });

        var schoolYearDateInfoUpdateData =
            new SchoolYearDateInfoDO
            {
                SchoolYearStartDate = new DateTime(2025, 09, 18),
                FirstTermEndDate = new DateTime(2026, 02, 01),
                SecondTermStartDate = new DateTime(2026, 02, 06),
                SchoolYearEndDate = new DateTime(2026, 06, 16),
                Description = "DescriptionUpdate",
                IsForAllClasses = false,
                BasicClassIds = new int[] { 7 }
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosPutAsync(this.schoolYear, this.institutionId, schoolYearDateInfoId, schoolYearDateInfoUpdateData);

        var schoolYearDateInfoUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosGetAsync(this.schoolYear, this.institutionId))
           .Where(e => e.SchoolYearDateInfoId == schoolYearDateInfoId)
           .SingleOrDefault();

        Assert.NotNull(schoolYearDateInfoUpdated);
        Assert.Equal(new DateTime(2025, 09, 18), schoolYearDateInfoUpdated.SchoolYearStartDate);
        Assert.Equal(new DateTime(2026, 02, 01), schoolYearDateInfoUpdated.FirstTermEndDate);
        Assert.Equal(new DateTime(2026, 02, 06), schoolYearDateInfoUpdated.SecondTermStartDate);
        Assert.Equal(new DateTime(2026, 06, 16), schoolYearDateInfoUpdated.SchoolYearEndDate);
        Assert.Equal("DescriptionUpdate", schoolYearDateInfoUpdated.Description);
        Assert.Equal(new int[] { 7 }, schoolYearDateInfoUpdated.BasicClassIds);
        Assert.False(schoolYearDateInfoUpdated.IsForAllClasses);
    }

    [Theory, InlineData(0), InlineData(1), InlineData(2), InlineData(3)]
    public async Task Should_remove_classbook_schoolYearDateInfo(int schoolYearDateInfoIndex)
    {
        var schoolYearDateInfo = this.schoolYearDateInfos[schoolYearDateInfoIndex];
        var schoolYearDateInfoId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosPostAsync(this.schoolYear, this.institutionId, schoolYearDateInfo);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosDeleteAsync(this.schoolYear, this.institutionId, schoolYearDateInfoId);

        var schoolYearDateInfoDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).SchoolYearDateInfosGetAsync(this.schoolYear, this.institutionId))
            .Where(od => od.SchoolYearDateInfoId == schoolYearDateInfoId)
            .SingleOrDefault();

        Assert.Null(schoolYearDateInfoDeleted);
    }
}

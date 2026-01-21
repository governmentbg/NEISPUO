namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class ParentMeetingTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private const string descriptionUpdate = "DescriptionUpdate";
    private static readonly DateTime date = new(2023, 4, 6);
    private readonly ParentMeetingDO parentMeeting =
        new ParentMeetingDO
        {
            Date = date,
            StartTime = "17:45",
            Location = null,
            Title = "ParentMeetingTitle",
            Description = "ParentMeetingDescription"
        };
    private readonly ExtApiWebApplicationFactory appFactory;

    public ParentMeetingTests(
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
        this.classBookId = data.ClassBookId;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_parentMeetings_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_parentMeeting()
    {
        var parentMeetingId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.parentMeeting);
        Assert.InRange(parentMeetingId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_parentMeeting()
    {
        var parentMeetingId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.parentMeeting);

        var parentMeetingUpdateData =
            new ParentMeetingDO
            {
                Date = date.AddDays(7),
                StartTime = "16:45",
                Location = "ClassRoom",
                Title = "ParentMeetingTitleUpdate",
                Description = descriptionUpdate,
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsPutAsync(this.schoolYear, this.institutionId, this.classBookId, parentMeetingId, parentMeetingUpdateData);

        var parentMeetingUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.ParentMeetingId == parentMeetingId)
           .SingleOrDefault();

        Assert.NotNull(parentMeetingUpdated);
        Assert.Equal("16:45", parentMeetingUpdated.StartTime);
        Assert.Equal(date.AddDays(7), parentMeetingUpdated.Date);
        Assert.Equal("ClassRoom", parentMeetingUpdated.Location);
        Assert.Equal("ParentMeetingTitleUpdate", parentMeetingUpdated.Title);
        Assert.Equal(descriptionUpdate, parentMeetingUpdated.Description);
    }

    [Fact]
    public async Task Should_remove_classbook_parentMeetings()
    {
        var parentMeetingId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.parentMeeting);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, parentMeetingId);

        var parentMeetingDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksParentMeetingsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(pm => pm.ParentMeetingId == parentMeetingId)
            .SingleOrDefault();

        Assert.Null(parentMeetingDeleted);
    }
}

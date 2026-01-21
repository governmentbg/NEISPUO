namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class ReplrParticipationTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private static readonly DateTime date = new(2023, 4, 6);
    private const string attendeesUpdate = "AttendeesUpdate";
    private readonly ReplrParticipationDO replrParticipation;
    private readonly ExtApiWebApplicationFactory appFactory;

    public ReplrParticipationTests(
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
        this.classId = data.ClassId;
        this.personId = data.PersonId;
        this.replrParticipation =
            new ReplrParticipationDO
            {
                ReplrParticipationTypeId = 1,
                Date = date,
                Topic = null,
                Attendees = "Attendees",
                InstId = null
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_replrParticipations_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_replrParticipation()
    {
        var replrParticipationId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.replrParticipation);
        Assert.InRange(replrParticipationId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_replrParticipation()
    {
        var replrParticipationId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.replrParticipation);

        var replrParticipationUpdateData =
            new ReplrParticipationDO
            {
                ReplrParticipationTypeId = 2,
                Date = date.AddDays(1),
                Topic = "Topic",
                Attendees = attendeesUpdate,
                InstId = this.institutionId
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsPutAsync(this.schoolYear, this.institutionId, this.classBookId, replrParticipationId, replrParticipationUpdateData);

        var replrParticipationUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.ReplrParticipationId == replrParticipationId)
           .SingleOrDefault();

        Assert.NotNull(replrParticipationUpdated);
        Assert.Equal(2, replrParticipationUpdated.ReplrParticipationTypeId);
        Assert.Equal("Topic", replrParticipationUpdated.Topic);
        Assert.Equal(date.AddDays(1), replrParticipationUpdated.Date);
        Assert.Equal(attendeesUpdate, replrParticipationUpdated.Attendees);
        Assert.Equal(this.institutionId, replrParticipationUpdated.InstId);
    }

    [Fact]
    public async Task Should_remove_classbook_replrParticipation()
    {
        var replrParticipationId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.replrParticipation);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, replrParticipationId);

        var replrParticipationDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksReplrParticipationsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(s => s.ReplrParticipationId == replrParticipationId)
            .SingleOrDefault();

        Assert.Null(replrParticipationDeleted);
    }
}

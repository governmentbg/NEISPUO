namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class TopicTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int scheduleLessonId;
    private readonly DateTime scheduleLessonDate;
    private readonly int scheduleLessonIdRemove;
    private readonly DateTime scheduleLessonDateRemove;
    private readonly ExtApiWebApplicationFactory appFactory;

    public TopicTests(
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
        this.scheduleLessonId = data.ScheduleLessonId;
        this.scheduleLessonDate = data.ScheduleLessonDate;
        this.scheduleLessonIdRemove = data.ScheduleLessonIdRemove;
        this.scheduleLessonDateRemove = data.ScheduleLessonDateRemove;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_topics_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksTopicsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_topic()
    {
        var topicId = await this.CreateTopicAsync(this.scheduleLessonId, this.scheduleLessonDate);
        Assert.InRange(topicId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_remove_classbook_topic()
    {
        var topicId = await this.CreateTopicAsync(this.scheduleLessonIdRemove, this.scheduleLessonDateRemove);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksTopicsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, topicId);

        var topicDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksTopicsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(t => t.TopicId == topicId)
            .SingleOrDefault();

        Assert.Null(topicDeleted);
    }

    private async Task<int> CreateTopicAsync(int scheduleLessonId, DateTime scheduleLessonDate)
    {
        var topic =
            new TopicDO
            {
                Date = scheduleLessonDate,
                Title = "TopicTitle",
                ScheduleLessonId = scheduleLessonId
            };

        return await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksTopicsPostAsync(this.schoolYear, this.institutionId, this.classBookId, topic);
    }
}

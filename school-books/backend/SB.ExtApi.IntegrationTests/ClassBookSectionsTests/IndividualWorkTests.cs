namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class IndividualWorkTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private static readonly DateTime date = new(2023, 4, 6);
    private const string individualWorkActivityUpdate = "IndividualWorkActivityUpdate";
    private readonly IndividualWorkDO individualWork;
    private readonly ExtApiWebApplicationFactory appFactory;

    public IndividualWorkTests(
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
        this.individualWork =
            new IndividualWorkDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                Date = date,
                IndividualWorkActivity = "IndividualWorkActivity",
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_individualWorks_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_individualWork()
    {
        var individualWorkId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.individualWork);
        Assert.InRange(individualWorkId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_individualWork()
    {
        var individualWorkId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.individualWork);

        var individualWorkUpdateData =
            new IndividualWorkDO
            {
                ClassId = this.classId,
                PersonId = this.personId,
                Date = date.AddDays(1),
                IndividualWorkActivity = individualWorkActivityUpdate
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksPutAsync(this.schoolYear, this.institutionId, this.classBookId, individualWorkId, individualWorkUpdateData);

        var individualWorkUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.IndividualWorkId == individualWorkId)
           .SingleOrDefault();

        Assert.NotNull(individualWorkUpdated);
        Assert.Equal(date.AddDays(1), individualWorkUpdated.Date);
        Assert.Equal(individualWorkActivityUpdate, individualWorkUpdated.IndividualWorkActivity);
    }

    [Fact]
    public async Task Should_remove_classbook_individualWork()
    {
        var individualWorkId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.individualWork);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, individualWorkId);

        var individualWorkDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksIndividualWorksGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(s => s.IndividualWorkId == individualWorkId)
            .SingleOrDefault();

        Assert.Null(individualWorkDeleted);
    }
}

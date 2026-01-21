namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class RemarkTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private readonly int curriculumId;
    private static readonly DateTime date = new(2023, 4, 6);
    private const string descriptionUpdate = "DescriptionUpdate";
    private readonly RemarkDO remark;
    private readonly ExtApiWebApplicationFactory appFactory;

    public RemarkTests(
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
        this.curriculumId = data.CurriculumId;
        this.remark =
            new RemarkDO
            {
                ClassId = data.ClassId,
                PersonId = data.PersonId,
                CurriculumId = data.CurriculumId,
                Type = RemarkType.Good,
                Date = date,
                Description = "RemarkDescription"
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_remarks_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_remark()
    {
        var remarkId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.remark);
        Assert.InRange(remarkId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_remark()
    {
        var remarkId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.remark);

        var remarkUpdateData =
           new RemarkDO
           {
               ClassId = this.classId,
               PersonId = this.personId,
               CurriculumId = this.curriculumId,
               Type = RemarkType.Good,
               Date = date.AddDays(7),
               Description = descriptionUpdate
           };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksPutAsync(this.schoolYear, this.institutionId, this.classBookId, remarkId, remarkUpdateData);

        var remarkUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.RemarkId == remarkId)
           .SingleOrDefault();

        Assert.NotNull(remarkUpdated);
        Assert.Equal(date.AddDays(7), remarkUpdated.Date);
        Assert.Equal(descriptionUpdate, remarkUpdated.Description);
    }

    [Fact]
    public async Task Should_remove_classbook_remark()
    {
        var remarkId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.remark);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, remarkId);

        var remarkDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksRemarksGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(r => r.RemarkId == remarkId)
            .SingleOrDefault();

        Assert.Null(remarkDeleted);
    }
}

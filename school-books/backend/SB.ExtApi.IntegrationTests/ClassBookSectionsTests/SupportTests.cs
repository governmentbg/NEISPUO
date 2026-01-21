namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class SupportTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private readonly int[] teacherIds;
    private static readonly DateTime date = new(2023, 9, 16);
    private const string descriptionUpdate = "DescriptionUpdate";
    private const string expectedResultUpdate = "ExpectedResultUpdate";
    private readonly SupportDO support;
    private readonly ExtApiWebApplicationFactory appFactory;

    public SupportTests(
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
        this.teacherIds = data.SupportTeacherIds;
        this.classId = data.ClassId;
        this.personId = data.PersonId;

        this.support =
            new SupportDO
            {
                Description = "SupportDescription",
                ExpectedResult = "SupportExpectedResult",
                EndDate = date,
                TeacherIds = this.teacherIds,
                IsForAllStudents = false,
                Students =
                    new SupportStudentDO[]
                    {
                        new SupportStudentDO
                        {
                            ClassId = this.classId,
                            PersonId = this.personId
                        },
                    },
                SupportDifficultyTypeIds = new int[] { 1 },
                Activities = new SupportActivityDO[]
                    {
                        new SupportActivityDO
                        {
                            SupportActivityTypeId = 1,
                            Target = "Target",
                            Result = "Result",
                            Date = date
                        },
                    },
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_supports_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_support()
    {
        var supportId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.support);
        Assert.InRange(supportId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_throw_on_support_without_students()
    {
        var ex = await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(
            async () => await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsPostAsync(
                this.schoolYear,
                this.institutionId,
                this.classBookId,
                new SupportDO
                {
                    Description = "SupportDescription",
                    ExpectedResult = "SupportExpectedResult",
                    EndDate = date,
                    TeacherIds = this.teacherIds,
                    IsForAllStudents = false,
                    Students = Array.Empty<SupportStudentDO>(),
                    SupportDifficultyTypeIds = new int[] { 1 },
                    Activities = new SupportActivityDO[]
                        {
                            new SupportActivityDO
                            {
                                SupportActivityTypeId = 1,
                                Target = "Target",
                                Result = "Result",
                                Date = date
                            },
                        },
                }));

        Assert.Contains("'Student Ids' must not be empty", ex.Message);
    }

    [Fact]
    public async Task Should_update_classbook_support()
    {
        var supportId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.support);
        var newTeacherIds = this.teacherIds.Take(1).ToArray();

        var supportUpdateData =
            new SupportDO
            {
                IsForAllStudents = true,
                Description = descriptionUpdate,
                ExpectedResult = "ExpectedResultUpdate",
                EndDate = date.AddDays(1),
                TeacherIds = newTeacherIds,
                Students = Array.Empty<SupportStudentDO>(),
                SupportDifficultyTypeIds = new int[] { 1, 2 },
                Activities = Array.Empty<SupportActivityDO>(),
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsPutAsync(this.schoolYear, this.institutionId, this.classBookId, supportId, supportUpdateData);

        var supportUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.SupportId == supportId)
           .SingleOrDefault();

        Assert.NotNull(supportUpdated);
        Assert.Equal(descriptionUpdate, supportUpdated.Description);
        Assert.Equal(expectedResultUpdate, supportUpdated.ExpectedResult);
        Assert.Equal(date.AddDays(1), supportUpdated.EndDate);
        Assert.Equal(new int[] { 1, 2 }, supportUpdated.SupportDifficultyTypeIds);
        Assert.Equal(newTeacherIds, supportUpdated.TeacherIds);
        Assert.Equal(Array.Empty<SupportStudentDO>(), supportUpdated.Students);
        Assert.Equal(Array.Empty<SupportActivityDO>(), supportUpdated.Activities);
    }

    [Fact]
    public async Task Should_remove_classbook_support()
    {
        var supportId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.support);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, supportId);

        var supportDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSupportsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(n => n.SupportId == supportId)
            .SingleOrDefault();

        Assert.Null(supportDeleted);
    }
}

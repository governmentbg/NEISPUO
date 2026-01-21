namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class StudentActivitiesTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int studentsCount;
    private readonly int personId;
    private readonly ExtApiWebApplicationFactory appFactory;

    public StudentActivitiesTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_PG data = fixtures.Values.Item4;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
        this.classBookId = data.ClassBookId;
        this.studentsCount = data.AllClassBookStudentClasses.DistinctBy(s => s.personId).Count();
        this.personId = data.PersonId;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_students_activities()
    {
        var students = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentActivitiesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        Assert.Equal(this.studentsCount, students.Count);
        Assert.All(students, s => Assert.NotNull(s.PersonId));
    }

    [Fact]
    public async Task Should_update_classbook_students_activities()
    {
        StudentActivitiesDO activitiesDO = new() { Activities = "test activities" };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentActivitiesPutAsync(this.schoolYear, this.institutionId, this.classBookId, this.personId, activitiesDO);

        var modified = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentActivitiesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(s => s.PersonId == this.personId)
           .SingleOrDefault();

        Assert.NotNull(modified);
        Assert.Equal(this.personId, modified.PersonId);
        Assert.Equal("test activities", modified.Activities);
    }
}

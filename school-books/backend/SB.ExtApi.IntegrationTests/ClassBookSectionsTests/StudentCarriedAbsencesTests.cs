namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class StudentCarriedAbsencesTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int studentsCount;
    private readonly int personId;
    private readonly ExtApiWebApplicationFactory appFactory;

    public StudentCarriedAbsencesTests(
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
        this.studentsCount = data.AllClassBookStudentClasses.DistinctBy(s => s.personId).Count();
        this.personId = data.PersonId;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_students_carried_absences()
    {
        var students = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentClassNumbersGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        Assert.Equal(this.studentsCount, students.Count);
        Assert.All(students, s => Assert.NotNull(s.PersonId));
    }

    [Fact]
    public async Task Should_update_classbook_students_carried_absences()
    {
        StudentCarriedAbsencesDO carriedAbsencesDO = new()
        {
            FirstTermExcusedCount = 10,
            FirstTermUnexcusedCount = 3,
            FirstTermLateCount = 0,
            SecondTermExcusedCount = 4,
            SecondTermUnexcusedCount = 0,
            SecondTermLateCount = 2
        };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentCarriedAbsencesPutAsync(this.schoolYear, this.institutionId, this.classBookId, this.personId, carriedAbsencesDO);

        var modified = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentCarriedAbsencesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(s => s.PersonId == this.personId)
           .SingleOrDefault();

        Assert.NotNull(modified);
        Assert.Equal(this.personId, modified.PersonId);
        Assert.Equal(10, modified.FirstTermExcusedCount);
        Assert.Equal(3, modified.FirstTermUnexcusedCount);
        Assert.Equal(0, modified.FirstTermLateCount);
        Assert.Equal(4, modified.SecondTermExcusedCount);
        Assert.Equal(0, modified.SecondTermUnexcusedCount);
        Assert.Equal(2, modified.SecondTermLateCount);
    }
}

namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class StudentSpecialNeedCurriculumsTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int studentsCount;
    private readonly int personId;
    private readonly int[] curriculumIds;
    private readonly ExtApiWebApplicationFactory appFactory;

    public StudentSpecialNeedCurriculumsTests(
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
        this.curriculumIds = data.ClassBookCurriculumIds;
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_students_special_needs_curriculums()
    {
        var students = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentSpecialNeedCurriculumsGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        Assert.Equal(this.studentsCount, students.Count);
        Assert.All(students, s => Assert.NotNull(s.PersonId));
    }

    [Fact]
    public async Task Should_update_classbook_students_special_needs_curriculums()
    {
        StudentSpecialNeedCurriculumsDO specialNeedCurriculumsDO = new() { StudentSpecialNeedCurriculumIds = this.curriculumIds };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentSpecialNeedCurriculumsPutAsync(this.schoolYear, this.institutionId, this.classBookId, this.personId, specialNeedCurriculumsDO);

        var modified = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksStudentSpecialNeedCurriculumsGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(s => s.PersonId == this.personId)
           .SingleOrDefault();

        Assert.NotNull(modified);
        Assert.Equal(this.personId, modified.PersonId);
        AssertUtils.SetsEqualWithoutDuplicates(this.curriculumIds, modified.StudentSpecialNeedCurriculumIds);
    }
}

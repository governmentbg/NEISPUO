namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class NoteTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classBookId;
    private readonly int classId;
    private readonly int personId;
    private const string descriptionUpdate = "DescriptionUpdate";
    private readonly NoteDO note;
    private readonly ExtApiWebApplicationFactory appFactory;

    public NoteTests(
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
        this.note =
            new NoteDO
            {
                Description = "NoteDescription",
                IsForAllStudents = false,
                Students =
                    new NoteStudentDO[]
                    {
                        new NoteStudentDO
                        {
                            ClassId = this.classId,
                            PersonId = this.personId
                        },
                    },
            };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_notes_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Fact]
    public async Task Should_create_classbook_note()
    {
        var noteId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.note);
        Assert.InRange(noteId, 1, int.MaxValue);
    }

    [Fact]
    public async Task Should_update_classbook_note()
    {
        var noteId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.note);

        var noteUpdateData =
            new NoteDO
            {
                IsForAllStudents = false,
                Description = descriptionUpdate,
                Students = Array.Empty<NoteStudentDO>()
            };

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesPutAsync(this.schoolYear, this.institutionId, this.classBookId, noteId, noteUpdateData);

        var noteUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
           .Where(e => e.NoteId == noteId)
           .SingleOrDefault();

        Assert.NotNull(noteUpdated);
        Assert.Equal(descriptionUpdate, noteUpdated.Description);
        Assert.Equal(Array.Empty<NoteStudentDO>(), noteUpdated.Students);
    }

    [Fact]
    public async Task Should_remove_classbook_note()
    {
        var noteId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesPostAsync(this.schoolYear, this.institutionId, this.classBookId, this.note);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesDeleteAsync(this.schoolYear, this.institutionId, this.classBookId, noteId);

        var noteDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksNotesGetAsync(this.schoolYear, this.institutionId, this.classBookId))
            .Where(n => n.NoteId == noteId)
            .SingleOrDefault();

        Assert.Null(noteDeleted);
    }
}

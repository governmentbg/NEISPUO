namespace SB.Domain;

public class NoteStudent
{
    // EF constructor
    private NoteStudent()
    {
        this.Note = null!;
    }

    internal NoteStudent(
        Note note,
        int personId)
    {
        this.Note = note;
        this.PersonId = personId;
    }

    public int SchoolYear { get; private set; }
    public int NoteId { get; private set; }
    public int PersonId { get; private set; }

    // relations
    public Note Note { get; private set; }
}

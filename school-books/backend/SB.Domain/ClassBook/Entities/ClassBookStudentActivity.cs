namespace SB.Domain;
public class ClassBookStudentActivity
{
    // EF constructor
    private ClassBookStudentActivity()
    {
        this.ClassBook = null!;
        this.Activities = null!;
    }

    internal ClassBookStudentActivity(ClassBook classBook, int personId, string activities)
    {
        this.ClassBook = classBook;
        this.PersonId = personId;
        this.Activities = activities;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public string Activities { get; private set; }

    public void Update(string activities)
    {
        this.Activities = activities;
    }

    // relations
    public ClassBook ClassBook { get; private set; }
}

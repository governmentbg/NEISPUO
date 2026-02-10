namespace SB.Domain;

public class ClassBookStudentFirstGradeResultSpecialNeeds
{
    // EF constructor
    private ClassBookStudentFirstGradeResultSpecialNeeds()
    {
        this.ClassBook = null!;
    }

    internal ClassBookStudentFirstGradeResultSpecialNeeds(
        ClassBook classBook,
        int personId
        )
    {
        this.ClassBook = classBook;
        this.PersonId = personId;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }
}

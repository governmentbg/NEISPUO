namespace SB.Domain;

public class ClassBookStudentSpecialNeeds
{
    // EF constructor
    private ClassBookStudentSpecialNeeds()
    {
        this.ClassBook = null!;
    }

    internal ClassBookStudentSpecialNeeds(
        ClassBook classBook,
        int personId,
        int curriculumId
        )
    {
        this.ClassBook = classBook;
        this.PersonId = personId;
        this.CurriculumId = curriculumId;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public int CurriculumId { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }
}

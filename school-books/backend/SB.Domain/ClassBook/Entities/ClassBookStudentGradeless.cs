namespace SB.Domain;

public class ClassBookStudentGradeless
{
    // EF constructor
    private ClassBookStudentGradeless()
    {
        this.ClassBook = null!;
    }

    internal ClassBookStudentGradeless(
        ClassBook classBook,
        int personId,
        int curriculumId,
        bool withoutFirstTermGrade,
        bool withoutSecondTermGrade,
        bool withoutFinalGrade)
    {
        this.ClassBook = classBook;
        this.PersonId = personId;
        this.CurriculumId = curriculumId;
        this.WithoutFirstTermGrade = withoutFirstTermGrade;
        this.WithoutSecondTermGrade = withoutSecondTermGrade;
        this.WithoutFinalGrade = withoutFinalGrade;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public int CurriculumId { get; private set; }

    public bool WithoutFirstTermGrade { get; private set; }

    public bool WithoutSecondTermGrade { get; private set; }

    public bool WithoutFinalGrade { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }
}

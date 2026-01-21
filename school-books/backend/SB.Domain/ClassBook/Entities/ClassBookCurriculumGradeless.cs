namespace SB.Domain;
public class ClassBookCurriculumGradeless
{
    // EF constructor
    private ClassBookCurriculumGradeless()
    {
        this.ClassBook = null!;
    }

    internal ClassBookCurriculumGradeless(ClassBook classBook, int curriculumId)
    {
        this.ClassBook = classBook;
        this.CurriculumId = curriculumId;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int CurriculumId { get; private set; }

    // relations
    public ClassBook ClassBook { get; private set; }
}

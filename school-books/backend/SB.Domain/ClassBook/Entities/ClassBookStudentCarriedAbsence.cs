namespace SB.Domain;

public class ClassBookStudentCarriedAbsence
{
    // EF constructor
    protected ClassBookStudentCarriedAbsence()
    {
        this.ClassBook = null!;
    }

    public ClassBookStudentCarriedAbsence(
        ClassBook classBook,
        int personId,
        int firstTermExcusedCount,
        int firstTermUnexcusedCount,
        int firstTermLateCount,
        int secondTermExcusedCount,
        int secondTermUnexcusedCount,
        int secondTermLateCount)
    {
        this.ClassBook = classBook;
        this.PersonId = personId;
        this.FirstTermExcusedCount = firstTermExcusedCount;
        this.FirstTermUnexcusedCount = firstTermUnexcusedCount;
        this.FirstTermLateCount = firstTermLateCount;
        this.SecondTermExcusedCount = secondTermExcusedCount;
        this.SecondTermUnexcusedCount = secondTermUnexcusedCount;
        this.SecondTermLateCount = secondTermLateCount;
    }

    public int SchoolYear { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public int FirstTermExcusedCount { get; private set; }

    public int FirstTermUnexcusedCount { get; private set; }

    public int FirstTermLateCount { get; private set; }

    public int SecondTermExcusedCount { get; private set; }

    public int SecondTermUnexcusedCount { get; private set; }

    public int SecondTermLateCount { get; private set; }

    public void Update(
        int firstTermExcusedCount,
        int firstTermUnexcusedCount,
        int firstTermLateCount,
        int secondTermExcusedCount,
        int secondTermUnexcusedCount,
        int secondTermLateCount)
    {
        this.FirstTermExcusedCount = firstTermExcusedCount;
        this.FirstTermUnexcusedCount = firstTermUnexcusedCount;
        this.FirstTermLateCount = firstTermLateCount;
        this.SecondTermExcusedCount = secondTermExcusedCount;
        this.SecondTermUnexcusedCount = secondTermUnexcusedCount;
        this.SecondTermLateCount = secondTermLateCount;
    }

    // relations
    public ClassBook ClassBook { get; private set; }
}

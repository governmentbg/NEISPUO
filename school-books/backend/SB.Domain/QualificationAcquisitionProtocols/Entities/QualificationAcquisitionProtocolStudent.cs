namespace SB.Domain;

public class QualificationAcquisitionProtocolStudent
{
    // EF constructor
    private QualificationAcquisitionProtocolStudent()
    {
        this.QualificationAcquisitionProtocol = null!;
    }

    public QualificationAcquisitionProtocolStudent(
        QualificationAcquisitionProtocol qualificationAcquisitionProtocol,
        int classId,
        int personId,
        bool examsPassed,
        decimal? theoryPoints,
        decimal? practicePoints,
        decimal? averageDecimalGrade)
    {
        this.QualificationAcquisitionProtocol = qualificationAcquisitionProtocol;
        this.ClassId = classId;
        this.PersonId = personId;

        this.ExamsPassed = examsPassed;
        this.TheoryPoints = theoryPoints;
        this.PracticePoints = practicePoints;
        this.AverageDecimalGrade = averageDecimalGrade;
    }

    public int QualificationAcquisitionProtocolId { get; private set; }

    public int SchoolYear { get; private set; }

    public int ClassId { get; private set; }

    public int PersonId { get; private set; }

    public bool ExamsPassed { get; private set; }

    public decimal? TheoryPoints { get; private set; }

    public decimal? PracticePoints { get; private set; }

    public decimal? AverageDecimalGrade { get; private set; }

    // relations
    public QualificationAcquisitionProtocol QualificationAcquisitionProtocol { get; private set; }

    public void UpdateData(
        bool examsPassed,
        decimal? theoryPoints,
        decimal? practicePoints,
        decimal? averageDecimalGrade)
    {
        this.ExamsPassed = examsPassed;
        this.TheoryPoints = theoryPoints;
        this.PracticePoints = practicePoints;
        this.AverageDecimalGrade = averageDecimalGrade;
    }
}

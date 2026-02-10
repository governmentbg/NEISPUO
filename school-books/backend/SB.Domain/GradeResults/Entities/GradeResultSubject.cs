namespace SB.Domain;

public class GradeResultSubject
{
    // EF constructor
    protected GradeResultSubject()
    {
        this.GradeResult = null!;
    }

    internal GradeResultSubject(
        GradeResult gradeResult,
        int curriculumId)
    {
        this.GradeResult = gradeResult;
        this.CurriculumId = curriculumId;
    }

    public int SchoolYear { get; private set; }
    public int GradeResultId { get; private set; }
    public int CurriculumId { get; private set; }
    public int? Session1Grade { get; private set; }
    public bool? Session1NoShow { get; private set; }
    public int? Session2Grade { get; private set; }
    public bool? Session2NoShow { get; private set; }
    public int? Session3Grade { get; private set; }
    public bool? Session3NoShow { get; private set; }

    public bool IsFilled => this.IsSession1Filled;

    private bool IsSession1Filled =>
        this.Session1Grade.HasValue ||
        this.Session1NoShow == true;
    private bool IsSession2Filled =>
        this.Session2Grade.HasValue ||
        this.Session2NoShow == true;
    private bool IsSession3Filled =>
        this.Session3Grade.HasValue ||
        this.Session3NoShow == true;

    // relations
    public GradeResult GradeResult { get; private set; }

    internal bool UpdateData(
        int? session1Grade,
        bool? session1NoShow,
        int? session2Grade,
        bool? session2NoShow,
        int? session3Grade,
        bool? session3NoShow)
    {
        if (this.Session1Grade == session1Grade &&
            this.Session1NoShow == session1NoShow &&
            this.Session2Grade == session2Grade &&
            this.Session2NoShow == session2NoShow &&
            this.Session3Grade == session3Grade &&
            this.Session3NoShow == session3NoShow)
        {
            return false;
        }

        this.Session1Grade = session1Grade;
        this.Session1NoShow = session1NoShow;
        this.Session2Grade = session2Grade;
        this.Session2NoShow = session2NoShow;
        this.Session3Grade = session3Grade;
        this.Session3NoShow = session3NoShow;

        this.AssertCorrectlyFilled();

        return true;
    }

    private void AssertCorrectlyFilled()
    {
        bool correct = (!this.IsSession2Filled || this.IsSession1Filled) &&
            (!this.IsSession3Filled || this.IsSession2Filled);

        if (!correct)
        {
            throw new DomainValidationException("A later session can only be filled if the previous one has been filled.");
        }
    }
}

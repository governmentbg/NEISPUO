namespace SB.Domain;

public class RegularGradePointAverageByStudentsReportItem
{
    // EF constructor
    private RegularGradePointAverageByStudentsReportItem()
    {
        this.ClassBookName = null!;
        this.StudentNames = null!;
        this.CurriculumInfo = null!;
    }

    public RegularGradePointAverageByStudentsReportItem(
        string classBookName,
        string studentNames,
        bool isTransferred,
        string curriculumInfo,
        decimal gradePointAverage,
        int totalGradesCount,
        int poorGradesCount,
        int fairGradesCount,
        int goodGradesCount,
        int veryGoodGradesCount,
        int excellentGradesCount,
        bool isTotal)
    {
        this.ClassBookName = classBookName;
        this.StudentNames = studentNames;
        this.IsTransferred = isTransferred;
        this.CurriculumInfo = curriculumInfo;
        this.GradePointAverage = gradePointAverage;
        this.TotalGradesCount = totalGradesCount;
        this.PoorGradesCount = poorGradesCount;
        this.FairGradesCount = fairGradesCount;
        this.GoodGradesCount = goodGradesCount;
        this.VeryGoodGradesCount = veryGoodGradesCount;
        this.ExcellentGradesCount = excellentGradesCount;
        this.IsTotal = isTotal;
    }

    public int RegularGradePointAverageByStudentsReportId { get; private set; }

    public int RegularGradePointAverageByStudentsReportItemId { get; private set; }

    public int SchoolYear { get; private set; }

    public string ClassBookName { get; private set; }

    public string StudentNames { get; private set; }

    public bool IsTransferred { get; private set; }

    public string CurriculumInfo { get; private set; }

    public decimal GradePointAverage { get; private set; }

    public int TotalGradesCount { get; private set; }

    public int PoorGradesCount { get; private set; }

    public int FairGradesCount { get; private set; }

    public int GoodGradesCount { get; private set; }

    public int VeryGoodGradesCount { get; private set; }

    public int ExcellentGradesCount { get; private set; }

    public bool IsTotal { get; private set; }
}

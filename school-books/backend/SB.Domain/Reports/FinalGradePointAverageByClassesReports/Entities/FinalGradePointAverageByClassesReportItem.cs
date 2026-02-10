namespace SB.Domain;

public class FinalGradePointAverageByClassesReportItem
{
    // EF constructor
    private FinalGradePointAverageByClassesReportItem()
    {
        this.ClassBookName = null!;
        this.CurriculumInfo = null!;
    }

    public FinalGradePointAverageByClassesReportItem(
        string classBookName,
        string curriculumInfo,
        int studentsCount,
        int studentsWithGradesCount,
        decimal studentsWithGradePercentage,
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
        this.CurriculumInfo = curriculumInfo;
        this.StudentsCount = studentsCount;
        this.StudentsWithGradesCount = studentsWithGradesCount;
        this.StudentsWithGradesPercentage = studentsWithGradePercentage;
        this.GradePointAverage = gradePointAverage;
        this.TotalGradesCount = totalGradesCount;
        this.PoorGradesCount = poorGradesCount;
        this.FairGradesCount = fairGradesCount;
        this.GoodGradesCount = goodGradesCount;
        this.VeryGoodGradesCount = veryGoodGradesCount;
        this.ExcellentGradesCount = excellentGradesCount;
        this.IsTotal = isTotal;
    }

    public int FinalGradePointAverageByClassesReportId { get; private set; }

    public int FinalGradePointAverageByClassesReportItemId { get; private set; }

    public int SchoolYear { get; private set; }

    public string ClassBookName { get; private set; }

    public string CurriculumInfo { get; private set; }

    public int StudentsCount { get; private set; }

    public int StudentsWithGradesCount { get; private set; }

    public decimal StudentsWithGradesPercentage { get; private set; }

    public decimal GradePointAverage { get; private set; }

    public int TotalGradesCount { get; private set; }

    public int PoorGradesCount { get; private set; }

    public int FairGradesCount { get; private set; }

    public int GoodGradesCount { get; private set; }

    public int VeryGoodGradesCount { get; private set; }

    public int ExcellentGradesCount { get; private set; }

    public bool IsTotal { get; private set; }
}

namespace SB.Domain;

public partial interface IRegularGradePointAverageByClassesReportsQueryRepository
{
    public record GetItemsForAddVO(
        string ClassBookName,
        string CurriculumInfo,
        int StudentsCount,
        int StudentsWithGradesCount,
        decimal GradePointAverage,
        int TotalGradesCount,
        int PoorGradesCount,
        int FairGradesCount,
        int GoodGradesCount,
        int VeryGoodGradesCount,
        int ExcellentGradesCount,
        bool IsTotal)
    {
        public decimal StudentsWithGradesPercentage => this.CalculateStudentsWithGradesPercentage();

        private decimal CalculateStudentsWithGradesPercentage()
        {
            if (this.StudentsCount == 0)
            {
                return 0M;
            }

            return ((decimal)this.StudentsWithGradesCount / this.StudentsCount) * 100;
        }
    }
}

namespace SB.Domain;

public partial interface IAbsencesByClassesReportsQueryRepository
{
    public record GetItemsForAddVO(
        string ClassBookName,
        int StudentsCount,
        int ExcusedAbsencesCount,
        decimal UnexcusedAbsencesCount,
        bool IsTotal)
    {
        public decimal ExcusedAbsencesCountAverage => this.CalculateExcusedAbsencesCountAverage();
        public decimal UnexcusedAbsencesCountAverage => this.CalculateUnexcusedAbsencesCountAverage();

        private decimal CalculateExcusedAbsencesCountAverage()
        {
            if (this.StudentsCount == 0)
            {
                return 0;
            }

            return (decimal)this.ExcusedAbsencesCount / this.StudentsCount;
        }

        private decimal CalculateUnexcusedAbsencesCountAverage()
        {
            if (this.StudentsCount == 0)
            {
                return 0;
            }

            return (decimal)this.UnexcusedAbsencesCount / this.StudentsCount;
        }
    }

}

namespace Domain.Models
{
    public class SchoolYearSanctionModel
    {
        public string schoolYear { get; set; }
        public string institution { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string type { get; set; }
        public string orderDate { get; set; }
        public string orderNumber { get; set; }
        public string ruoOrderDate { get; set; }
        public string ruoOrderNumber { get; set; }
        public string cancelationOrderDate { get; set; }
        public string cancelationOrderNumber { get; set; }
        public string description { get; set; }
        public string cancelationReason { get; set; }
    }
}

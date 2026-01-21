namespace MON.Models.ASP
{
    public class AspMonConfirmViewModel
    {
        public int Id { get; set; }
        public int IntYear { get; set; }
        public string IntMonth { get; set; }
        public string InstitutionCode { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public decimal NotExcused { get; set; }
        public decimal NotExcusedCorrection { get; set; }
        public decimal Days { get; set; }
        public decimal DaysCorrection { get; set; }
        public string AspStatus { get; set; }
        public string MonStatus { get; set; }
    }
}

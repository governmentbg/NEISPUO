namespace MON.Models.ASP
{
    public class AspUnprocessedRequestViewModel
    {
        public int Id { get; set; }
        public int SessionNo { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public string InstitutionId { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public decimal NotExcused { get; set; }
        public int Days { get; set; }
        public string AspStatus { get; set; }
        public string Error { get; set; }
    }
}

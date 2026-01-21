namespace MON.Models.StudentModels.Class
{
    using System;

    public class StudentClassDualFormCompanyModel
    {
        public int? Id { get; set; }
        public string Uic { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string Settlement { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

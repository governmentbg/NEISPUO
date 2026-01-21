namespace MON.Models.HealthInsurance
{
    using System;

    public class HealthInsuranceDto
    {
        public int PersonId { get; set; }
        public string Pin { get; set; }
        public int? PinTypeId { get; set; }
        public string PinType { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int InstitutionId { get; set; }
        public int InstitutionName { get; set; }
        public int StudentEduFormId { get; set; }
        public int ClassId { get; set; }
        public int? ClassTypeId { get; set; }
        public string ClassName { get; set; }
        public int StudentClassId { get; set; }
        public int PositionId { get; set; }
        public DateTime? PaymentStartByEnrollmentDate { get; set; }
        public DateTime? PaymentEndByEnrollmentDate { get; set; }
        public DateTime? PaymentStartByBirthDate { get; set; }
        public DateTime? PaymentEndByBirthDate { get; set; }
        public int? MonthDays { get; set; }
        public int? DaysToPayByBirtDate { get; set; }
        public int? DaysToPayByEnrollmentDate { get; set; }
        public long RowNumber { get; set; }
    }
}

namespace MON.Models.Absence
{
    using MON.Shared;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AbsenceExportModel
    {
        public int InstitutionCode { get; set; }

        public short SchoolYear { get; set; }

        public short Month { get; set; }

        public int StudentIdentificationType { get; set; }

        public string Identification { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int Class { get; set; }

        public string ClassCode { get; set; }

        public string GroupCode { get; set; }

        public string ClassName { get; set; }

        public int AttendsClassRegularly { get; set; }

        public DateTime? UnsubscriptionDate { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal MonthlyAbsencesForValidReason { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal MonthlyAbsencesForUnvalidReason { get; set; }

        public int? PersonId { get; set; }

        public string ToFileLine()
        {
            return $"{(Month < 9 ? SchoolYear + 1 : SchoolYear):0000}|{Month:00}|{InstitutionCode:0000000}|{Identification.Truncate(10).PadLeft(10, '0')}|{StudentIdentificationType:0}" +
                $"|{Class.ToString().Truncate(2).PadLeft(2, '0')}|{MonthlyAbsencesForUnvalidReason:000.00}";
        }
    }
}

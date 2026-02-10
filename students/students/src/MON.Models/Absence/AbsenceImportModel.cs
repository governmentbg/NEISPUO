using MON.Shared;
using System;

namespace MON.Models.Absence
{
    public class AbsenceImportModel
    {
        public int InstitutionCode { get; set; }

        public int SchoolYear { get; set; }

        public int Month { get; set; }

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

        public decimal MonthlyAbsencesForValidReason { get; set; }

        public decimal MonthlyAbsencesForUnvalidReason { get; set; }

        public int? PersonId { get; set; }
        public int? StudentAbsenceId { get; set; }
        public string SchoolYearName { get; set; }

        public string ToFileLine()
        {
            return $"{InstitutionCode:0000000}|{SchoolYear}|{Month:00}|{(StudentIdentificationType > 0 ? StudentIdentificationType.ToString() : string.Empty)}|{Identification.Truncate(30)}" +
                $"|{FirstName.Truncate(255)}|{MiddleName.Truncate(255)}|{LastName.Truncate(255)}|{Class:00}|{ClassCode.Truncate(4)}" +
                $"|{GroupCode.Truncate(4)}|{ClassName.Truncate(255)}|{(AttendsClassRegularly > 0 ? AttendsClassRegularly.ToString() : string.Empty)}" +
                $"|{UnsubscriptionDate:dd.mm.yyyy}|{MonthlyAbsencesForValidReason:0.00}|{MonthlyAbsencesForUnvalidReason:0.00}";
        }
    }
}


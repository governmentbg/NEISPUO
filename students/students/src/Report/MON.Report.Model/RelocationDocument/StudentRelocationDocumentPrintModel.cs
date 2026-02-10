namespace MON.Report.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [DataObject]
    public class StudentRelocationDocumentPrintModel
    {
        public int Id { get; set; }

        public short SchoolYear { get; set; }

        public int BasicClassId { get; set; }

        public int PersonId { get; set; }

        public int? CurrentStudentClassId { get; set; }

        public string BasicClassName { get; set; }

        public string ClassType { get; set; }

        public string Speciality { get; set; }

        public string Profession { get; set; }

        public string StudentProfileOrSpeciality => !string.IsNullOrWhiteSpace(ClassType)
            ? ClassType
            : $"{Profession}, {Speciality}";

        public string NoteNumber { get; set; }

        public DateTime NoteDate { get; set; }
        public string NoteDateStr => NoteDate.ToString("dd.MM.yyyy");

        public DateTime? DischargeDate { get; set; }

        public string DischargeDateStr => DischargeDate.HasValue ? DischargeDate.Value.ToString("dd.MM.yyyy") : "-";


        public int? SendingInstitutionId { get; set; }

        public string SendingInstitutionName { get; set; }

        public string SendingInstitutionCity { get; set; }

        /// <summary>
        /// Община
        /// </summary>
        public string SendingInstitutionMunicipality { get; set; }

        /// <summary>
        /// Област
        /// </summary>
        public string SendingInstitutionRegion { get; set; }

        /// <summary>
        /// Район
        /// </summary>
        public string SendingInstitutionDistrict { get; set; }

        public string SendingInstitutionAddress => $"гр./с. {SendingInstitutionCity}, общ. {SendingInstitutionMunicipality}{(string.IsNullOrWhiteSpace(SendingInstitutionDistrict) ? "" : $", район {SendingInstitutionDistrict}")}, обл. {SendingInstitutionRegion}";

        public string HostingInstitutionName { get; set; }
        public string HostingInstitutionCity { get; set; }
        public string HostingInstitutionMunicipality { get; set; }
        public string HostingInstitutionRegion { get; set; }
        public string HostingInstitutionDistrict { get; set; }
        public string HostingInstitutionAddress => $"гр./с. {HostingInstitutionCity}, общ. {HostingInstitutionMunicipality}{(string.IsNullOrWhiteSpace(HostingInstitutionDistrict) ? "" : $", район {HostingInstitutionDistrict}")}, обл. {HostingInstitutionRegion}";

        public string PrincipalName { get; set; }

        public string SchoolYearName { get; set; }

        public string Pin { get; set; }

        public string EduForm { get; set; }

        public string StudentFullName { get; set; }

        public string StudentNationality { get; set; }

        public DateTime? StudentBirthDate { get; set; }

        public string StudentBirthDateStr => StudentBirthDate.HasValue ? StudentBirthDate.Value.ToString("dd.MM.yyyy") : "-";

        public string StudentBirthCity { get; set; }

        public string StudentBirthMunicipality { get; set; }

        public string StudentBirthRegion { get; set; }

        public string StudentBirthAddress
        {
            get
            {
                return StudentBirthPlaceId.HasValue
                    ? $"гр./с. {StudentBirthCity}, общ. {StudentBirthMunicipality}, обл. {StudentBirthRegion}"
                    : $"{StudentBirthCity}, {StudentBirthPlaceCountry}";
            }
        }

        public List<SubjectEvaluationPrintModel> Evaluations { get; set; } = new List<SubjectEvaluationPrintModel>();

        public List<SubjectCurrentGradesPrintModel> CurrentGrades { get; set; } = new List<SubjectCurrentGradesPrintModel>();

        public RelocationDocumentAbsencePrintModel Absences { get; set; }

        public List<StudentSanctionPrintModel> Sanctions { get; set; }
        public string EduOrganization { get; set; }
        public DateTime? EduEndDate { get; set; }
        public string EduEndDateStr => EduEndDate.HasValue ? EduEndDate.Value.ToString("dd.MM.yyyy") : "-";
        public DateTime? EduStartDate { get; set; }
        public string EduStartDateStr => EduStartDate.HasValue ? EduStartDate.Value.ToString("dd.MM.yyyy") : "-";

        public int? StudentBirthPlaceId { get; set; }
        public string StudentBirthPlaceCountry { get; set; }
    }
}

using MON.Shared.Enums.SchoolBooks;
using MON.Shared.Extensions;
using System;
using System.Collections.Generic;

namespace MON.Models
{
    public class RecognitionViewModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionCountry { get; set; }
        public string BasicClass { get; set; }
        public SchoolTerm? Term { get; set; }
        public string EducationLevel { get; set; }
        public string SPPOOProfession { get; set; }
        public string SPPOOSpeciality { get; set; }
        public string RuoDocumentNumber { get; set; }
        public DateTime? RuoDocumentDate { get; set; }
        public string DiplomaNumber { get; set; }
        public DateTime? DiplomaDate { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
        public string TermName =>  this.Term?.GetEnumDescription() ?? "";

        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
    }
}

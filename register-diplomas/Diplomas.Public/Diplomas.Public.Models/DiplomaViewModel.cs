using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class DiplomaViewModel
    {
        public DiplomaViewModel()
        {
            Contents = new List<PropertyDescriptionValue>();
            Documents = new List<DiplomaDocumentModel>();
        }

        public int Id { get; set; }
        public List<PropertyDescriptionValue> Contents { get; set; }
        public string ContentsJson { get; set; }
        public string SchemaJson { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public short? YearGraduated { get; set; }
        public PersonViewModel Person { get; set; }
        public BasicDocumentViewModel BasicDocument { get; set; }
        public List<DiplomaDocumentModel> Documents { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegistrationNumberTotal { get; set; }
        public string RegistrationNumberYear { get; set; }
        public string RegistrationNumberDate { get; set; }
        public string InstitutionName { get; set; }
        public bool IsCancelled { get; set; }
        public string CancellationDate { get; set; }
        public decimal? Gpa { get; set; }
        public string GpaText { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models
{
    public class RecognitionModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        [MaxLength(1000)]
        public string InstitutionName { get; set; }
        public int? InstitutionCountryId { get; set; }
        public int EducationLevelId { get; set; }
        public int? SPPOOProfessionId { get; set; }
        public int? SPPOOSpecialityId { get; set; }
        public int? Term { get; set; }
        public int? BasicClassId { get; set; }
        [MaxLength(100)]
        public string RuoDocumentNumber { get; set; }
        public DateTime? RuoDocumentDate { get; set; }
        [MaxLength(100)]
        public string DiplomaNumber { get; set; }
        public DateTime? DiplomaDate { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
        public IEnumerable<RecognitionEqualizationModel> Equalizations { get; set; }
        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public string SPPOOProfessionName { get; set; }
        public string SPPOOSpecialityName { get; set; }
        public string EducationLeveName { get; set; }
        public string BasicClassName { get; set; }
        public int? VetLevel { get; set; }
        public bool IsSelfEduForm { get; set; }
    }
}

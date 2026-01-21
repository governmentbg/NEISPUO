namespace MON.Models.Diploma
{
    using System;

    public class DiplomaAdditionalDocumentModel
    {
        public int? Id { get; set; }
        /// <summary>
        /// Свързан документ
        /// </summary>
        public int? MainDiplomaId { get; set; }
        public int? BasicDocumentId { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionAddress { get; set; }
        public string Town { get; set; }
        public string Municipality { get; set; }
        public string Region { get; set; }
        public string LocalArea { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationNumberYear { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }

    public class DiplomaAdditionalDocumentViewModel : DiplomaAdditionalDocumentModel
    {
        public string BasicDocumentName { get; set; }
        public string InstitutionDetails { get; set; }
    }
}
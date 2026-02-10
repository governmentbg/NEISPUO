namespace MON.Models.Diploma.Import
{
    using System;

    public class DiplomaImportValidationExclusionModel
    {
        public string Id { get; set; }
        public short PersonalIdTypeId { get; set; }
        public string PersonalId { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public int? InstitutonId { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}

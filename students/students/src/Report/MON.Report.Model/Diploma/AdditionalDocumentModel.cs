namespace MON.Report.Model.Diploma
{
    public class AdditionalDocumentModel
    {
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegNumberTotal { get; set; }
        public string RegNumberYear { get; set; }
        public string RegDate { get; set; }
        public int? InstitutionId { get; set; }
        public string Institution { get; set; }
        public bool IsValidation { get; set; }
        public bool IsRecognition { get; set; }
    }
}

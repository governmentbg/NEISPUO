namespace MON.Models.Diploma
{
    public class DiplomaTemplateEditModel
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string Contents { get; set; }
        public string SubjectContents { get; set; }
        public DropdownViewModel Institution { get; set; }
        public int InstitutionId { get; set; }
        public int BasciDocumentId { get; set; }
    }
}

namespace MON.Models.Diploma
{
    public class DiplomaTemplateUpdateModel
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public int InstitutionId { get; set; }
        public string Contents { get; set; }
        public string SubjectContents { get; set; }
    }
}

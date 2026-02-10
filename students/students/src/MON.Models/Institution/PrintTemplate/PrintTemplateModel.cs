namespace MON.Models.Institution.PrintTemplate
{
    public class PrintTemplateModel
    {
        public int? Id { get; set; }
        public int? InstitutionId { get; set; }
        public int? RuoRegId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BasicDocumentId { get; set; }
        public int Left1Margin { get; set; }
        public int Top1Margin { get; set; }
        public int Left2Margin { get; set; }
        public int Top2Margin { get; set; }
        public int PrintFormId { get; set; }
    }
}

namespace MON.Models.Diploma
{
    public class DiplomaTemplateListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InstitutionName { get; set; }
        public string BasicDocumentTypeName { get; set; }
        public bool CanBeDeleted { get; set; }
        public string BasicClassName { get; set; }
    }
}

namespace MON.Models.Institution.PrintTemplate
{
    public class PrintTemplateViewModel : PrintTemplateModel
    {
        public bool HasContents { get; set; }
        public string BasicDocumentName { get; set; }

        public DropdownViewModel BasicDocument { get; set; }
        public short PrintFormEdition {get; set; }
    }
}

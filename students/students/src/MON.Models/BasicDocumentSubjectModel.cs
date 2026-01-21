namespace MON.Models
{

    public class BasicDocumentSubjectModel
    {
        public int? Id { get; set; }
        public int Position { get; set; }
        public int? ParentId { get; set; }
        public bool SubjectCanChange { get; set; }
        public DropdownViewModel SubjectDropDown { get; set; }
        public int? SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public string Uid { get; set; }
    }
}

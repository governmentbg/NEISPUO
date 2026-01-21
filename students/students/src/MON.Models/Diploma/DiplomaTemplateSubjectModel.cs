namespace MON.Models.Diploma
{
    public class DiplomaTemplateSubjectModel
    {
        public DropdownViewModel SubjectDropdown { get; set; }

        public SubjectTypeDropdownViewModel SubjectTypeDropdown { get; set; }
        public int Position { get; set; }
        public bool SubjectCanChange { get; set; }
        public int? ParentId { get; set; }
    }
}

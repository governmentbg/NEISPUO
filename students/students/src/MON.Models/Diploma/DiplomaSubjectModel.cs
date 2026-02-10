namespace MON.Models.Diploma
{
    public class DiplomaSubjectModel
    {
        public int? Id { get; set; }
        public int? DiplomaId { get; set; }
        public int? BasicDocumentPartId { get; set; }
        public int? BasicDocumentSubjectId { get; set; }
        public DropdownViewModel SubjectDropDown { get; set; }
        public decimal? Grade { get; set; }
        public string GradeText { get; set; }
        public int? Position { get; set; }
        public int? Horarium { get; set; }
        public string Uid { get; set; }

        /// <summary>
        /// Забранява редакцията на SubjectDropDown и Position.
        /// Предметите в дадена диплома, които са дефинирани в BasicDocumentSubjects, не трябва да се променят.
        /// </summary>
        public bool LockedForEdit { get; set; }
        public bool IsHorariumHidden { get; set; }
        public bool SubjectCanChange { get; set; }
        public int? ParentId { get; set; }
    }
}

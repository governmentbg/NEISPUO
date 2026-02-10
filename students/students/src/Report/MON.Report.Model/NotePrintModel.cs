namespace MON.Report.Model
{
    public class NotePrintModel
    {
        public string IssueDate { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string InstitutionName { get; set; }

        public long NoteNumber { get; set; }

        public short? SchoolYear { get; set; }
    }
}

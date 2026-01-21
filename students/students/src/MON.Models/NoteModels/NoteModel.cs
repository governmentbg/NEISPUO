namespace MON.Models.NoteModels
{
    using System;

    public class NoteModel
    {
        public int? NoteId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime IssueDate { get; set; }

        public short? SchoolYear { get; set; }

        public int? InstitutionId { get; set; }

        public int PersonId { get; set; }
    }
}

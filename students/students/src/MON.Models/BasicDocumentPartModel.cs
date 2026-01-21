namespace MON.Models
{
    using System.Collections.Generic;

    public class BasicDocumentPartUpdateModel
    {
        public int? Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool? IsHorariumHidden { get; set; }
        public int? BasicClassId { get; set; }
        public int? PrintedLines { get; set; }
        public int? TotalLines { get; set; }
        public List<int> SubjectTypes { get; set; } = new List<int>();
        public List<int> ExternalEvaluationTypes { get; set; }
        public List<BasicDocumentSubjectModel> BasicDocumentSubjects { get; set; }
    }
}

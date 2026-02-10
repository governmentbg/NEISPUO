using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class DocumentSubjectViewModel
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public string Subject { get; set; }
        public string SubjectType { get; set; }
        public string GradeText { get; set; }
        public decimal? Grade { get; set; }
        public decimal? Points { get; set; }
        public string FLLevel { get; set; }
        public int? Horarium { get; set; }
        public int? BasicDocumentPartId { get; set; }
    }
}

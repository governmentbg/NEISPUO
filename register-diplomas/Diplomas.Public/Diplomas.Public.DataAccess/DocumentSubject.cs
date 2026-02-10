using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class DocumentSubject
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int BasicDocumentPartId { get; set; }
        public int? BasicDocumentSubjectId { get; set; }
        public int SubjectId { get; set; }
        public decimal Grade { get; set; }
        public string GradeText { get; set; }
        public int Position { get; set; }

        public virtual BasicDocumentPart BasicDocumentPart { get; set; }
        public virtual BasicDocumentPart BasicDocumentSubject { get; set; }
        public virtual Diploma Document { get; set; }
        public virtual TmpSubject Subject { get; set; }
    }
}

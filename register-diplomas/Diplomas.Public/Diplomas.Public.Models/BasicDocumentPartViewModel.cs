using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class BasicDocumentPartViewModel
    {
        public BasicDocumentPartViewModel()
        {
            Subjects = new List<DocumentSubjectViewModel>();
        }

        public int Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public List<DocumentSubjectViewModel> Subjects { get; set; }
        public bool IsHorariumHidden { get; set; }
        public string SubjectTypesList { get; set; }
        public List<int> SubjectTypes { get; set; }
        public string ExternalEvaluationTypesList { get; set; }
        public List<int> ExternalEvaluationTypes { get; set; }

    }
}

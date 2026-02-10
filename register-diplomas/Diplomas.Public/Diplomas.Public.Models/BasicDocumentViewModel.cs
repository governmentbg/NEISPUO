using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class BasicDocumentViewModel
    {
        public BasicDocumentViewModel()
        {
            Parts = new List<BasicDocumentPartViewModel>();
        }

        public string Name { get; set; }
        public List<BasicDocumentPartViewModel> Parts { get; set; }
        public List<DocumentSubjectViewModel> Subjects { get; set; }
    }
}

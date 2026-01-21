using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class DiplomaDocumentModel
    {
        public int? Id { get; set; }
        public int DiplomaId { get; set; }
        public int BlobId { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}

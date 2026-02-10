using System.Collections.Generic;

namespace MON.Models.Interfaces
{
    public interface IDocumentModels
    {
        IEnumerable<DocumentModel> Documents { get; set; }
    }
}

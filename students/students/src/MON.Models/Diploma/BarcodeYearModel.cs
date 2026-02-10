using System.Collections.Generic;

namespace MON.Models.Diploma
{
    public class BarcodeYearModel
    {
        public int Id { get; set; }
        public short Edition { get; set; }
        public short SchoolYear { get; set; }
        public string HeaderPage { get; set; }
        public string InternalPage { get; set; }
        public int BasicDocumentId { get; set; }
    }

    public class BarcodeYearListViewModel
    {
        public string DiplomaTypeName { get; set; }
        public IEnumerable<BarcodeYearModel> BarcodeYears { get; set; }
    }
}

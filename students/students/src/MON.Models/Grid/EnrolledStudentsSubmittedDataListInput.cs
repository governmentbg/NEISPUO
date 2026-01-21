namespace MON.Models.Grid
{
    public class EnrolledStudentsSubmittedDataListInput: PagedListInput
    {
        public short? Year { get; set; }
        public byte? Month { get; set; }
        public string ExportTypeCode { get; set; }
        public byte? AspStatus { get; set; }
    }
}

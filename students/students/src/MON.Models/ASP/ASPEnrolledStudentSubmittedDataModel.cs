namespace MON.Models.ASP
{
    public class ASPEnrolledStudentSubmittedDataModel : ASPEnrolledStudentClassData
    {
        public short Year { get; set; }
        public byte Month { get; set; }
        public string ExportTypeCode { get; set; }
    }
}

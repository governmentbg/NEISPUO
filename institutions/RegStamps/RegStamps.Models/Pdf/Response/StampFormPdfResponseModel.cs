namespace RegStamps.Models.Pdf.Response
{
    using Shared.Database;
    using Shared.Response;

    public class StampFormPdfResponseModel
    {
        public SchoolDataResponseModel SchoolData { get; set; } = new SchoolDataResponseModel();
        public StampDetailsDataDatabaseModel StampData { get; set; } = new StampDetailsDataDatabaseModel();
    }
}

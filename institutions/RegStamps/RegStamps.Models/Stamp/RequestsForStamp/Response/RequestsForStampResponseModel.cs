namespace RegStamps.Models.Stamp.RequestsForStamp.Response
{
    using Database;
    using Shared.Response;

    public class RequestsForStampResponseModel
    {
        public SchoolDataResponseModel SchoolData { get; set; } = new SchoolDataResponseModel();
        public IEnumerable<RequestStampDatabaseModel> RequestStampList { get; set; } = new List<RequestStampDatabaseModel>();
    }
}

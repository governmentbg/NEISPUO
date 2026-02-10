namespace RegStamps.Models.Stamp.ListStamp.Response
{
    using Database;

    using Shared.Response;

    public class ListStampResponseModel
    {
        public SchoolDataResponseModel SchoolData { get; set; } = new SchoolDataResponseModel();
        public IEnumerable<StampDataDatabaseModel> StampDataList { get; set; } = new List<StampDataDatabaseModel>();
        public IEnumerable<RequestBackDatabaseModel> RequestBackDataList { get; set; } = new List<RequestBackDatabaseModel>();
    }
}

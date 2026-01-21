namespace RegStamps.Models.Stamp.StampDetails.Response
{
    using Shared.Database;
    using Shared.Response;

    public class StampDetailsResponseModel
    {
        public SchoolDataResponseModel SchoolData { get; set; } = new SchoolDataResponseModel();

        public StampDetailsDataDatabaseModel StampData { get; set; } = new StampDetailsDataDatabaseModel();

        public RequestStampDetailsDatabaseModel RequestStampData { get; set; } = new RequestStampDetailsDatabaseModel();

        public KeeperDataDatabaseModel KeeperData { get; set; } = new KeeperDataDatabaseModel();

        public IEnumerable<RequestFileDataDatabaseModel> RequestFileDataList { get; set; } = new List<RequestFileDataDatabaseModel>();
    }
}

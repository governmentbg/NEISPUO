namespace RegStamps.Models.Stamp.EditStamp
{
    using RegStamps.Models.Stamp.EditStamp.Response;

    public class EditStampViewModel
    {
        public EditStampResponseModel StampData { get; set; } = new EditStampResponseModel();
        public EditRequestResponseModel RequestStampData { get; set; } = new EditRequestResponseModel();
        public EditKeeperResponseModel KeeperData { get; set; } = new EditKeeperResponseModel();
        public EditRequestFileResponseModel RequestFileData { get; set; } = new EditRequestFileResponseModel();
    }
}

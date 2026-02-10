namespace RegStamps.Models.Check.Request
{
    public class CheckDataRequestModel
    {
        public int StampId { get; set; }
        public int RequestId { get; set; }
        public int KeeperId { get; set; }
        public int PlaceId { get; set; }
    }
}

namespace MON.Models.Refugee
{
    public class RefugeeApplicationCancellationModel
    {
        public int? ApplicationId { get; set; }
        public int? ApplicationChildId { get; set; }
        public string CancellationReason { get; set; }
    }
}

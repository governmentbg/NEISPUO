namespace MON.Models.Institution
{
    public class InstitutionInfoModel : InstitutionCacheModel
    {
        public string Region { get; set; }
        public string Municipality { get; set; }
        public int? MunicipalityId { get; set; }
        public string Town { get; set; }
        public int? TownId { get; set; }
    }
}

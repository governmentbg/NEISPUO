namespace MON.Models
{
    public class CurrencyModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? ExchangeRate { get; set; }
        public bool IsMain { get; set; }
    }
}

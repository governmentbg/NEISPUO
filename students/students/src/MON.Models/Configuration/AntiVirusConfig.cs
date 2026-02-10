namespace MON.Models.Configuration
{
    public class AntiVirusConfig
    {
        public bool Enabled { get; set; }
        public int MaxSize { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}

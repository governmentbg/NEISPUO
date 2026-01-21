namespace MON.Models.Configuration
{
    public class CachingConfig
    {
        public bool UseRedis { get; set; }
        public string RedisConnectionString { get; set; }
        public string RedisPassword { get; set; }
    }
}

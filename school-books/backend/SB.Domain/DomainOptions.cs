namespace SB.Domain;

using System;

public class DomainOptions
{
    public string? BlobServicePublicUrl { get; set; }

    public string? BlobServiceHMACKey { get; set; }

    public string? RedisConnectionString { get; set; }

    public int MemoryCacheSizeLimitMB { get; set; }

    public TimeSpan ShortCacheExpiration { get; set; }

    public TimeSpan MediumCacheExpiration { get; set; }

    public TimeSpan LongCacheExpiration { get; set; }

    public string? TestPdfSigningCertificateThumbprint { get; set; }

    public int? NeispuoExtSystemId { get; set; }

    public string? VAPIDPublicKey { get; set; }

    public string? VAPIDPrivateKey { get; set; }

    public TimeSpan? NotificationTime { get; set; }
}

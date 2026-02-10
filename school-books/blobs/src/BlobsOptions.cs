namespace SB.Blobs;

using System;
using Microsoft.AspNetCore.HttpOverrides;

public class BlobsOptions
{
    public TimeSpan BlobUrlExpiration { get; set; }

    public TimeSpan BlobUploadExpiration { get; set; }

    public TimeSpan ClockSkew { get; set; }

    public string HMACKey { get; set; }

    public string PostAuthOIDCAuthority { get; set; }

    public string[] PostAuthOIDCValidAudiences { get; set; }

    public TimeSpan? PostAuthOIDCClockSkew { get; set; }

    public bool? PostAuthOIDCRequireHttpsMetadata { get; set; }

    public bool HasPostAuthentication => !string.IsNullOrEmpty(this.PostAuthOIDCAuthority);

    public string[] AllowedCorsOrigins { get; set; }

    public IPNetwork[] ForwardedHeadersKnownNetworks { get; set; }

    public int? ForwardedHeadersForwardLimit { get; set; }
}

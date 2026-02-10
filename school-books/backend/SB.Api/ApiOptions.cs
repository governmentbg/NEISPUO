namespace SB.Api;

using System;
using Microsoft.AspNetCore.HttpOverrides;

public class ApiOptions
{
    public string[]? AllowedCorsOrigins { get; set; }

    public TimeSpan? OIDCClockSkew { get; set; }

    public string? OIDCAuthority { get; set; }

    public string? OIDCAudience { get; set; }

    public bool? OIDCRequireHttpsMetadata { get; set; }

    public IPNetwork[]? ForwardedHeadersKnownNetworks { get; set; }

    public int? ForwardedHeadersForwardLimit { get; set; }
}

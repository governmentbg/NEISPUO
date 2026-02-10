namespace SB.ExtApi;

using Microsoft.AspNetCore.HttpOverrides;

public class ExtApiOptions
{
    // Required because the ExtApi is hosted as a virtual directory with other APIs on the same domain.
    // NSwag supports X-Forwarded-Prefix (see https://github.com/RicoSuter/NSwag/pull/2196) which should
    // make this setting irrelevent but so far the kubernetes ingress does not support dynamic
    // X-Forwarded-Prefix headers based on the regex match
    // (see https://github.com/kubernetes/ingress-nginx/pull/3786)
    public string PathBase { get; set; } = "";

    public string? NeispuoExtApiRootCaPem { get; set; }

    public IPNetwork[]? ForwardedHeadersKnownNetworks { get; set; }

    public int? ForwardedHeadersForwardLimit { get; set; }

    public int? HisExtSystemId { get; set; }
}

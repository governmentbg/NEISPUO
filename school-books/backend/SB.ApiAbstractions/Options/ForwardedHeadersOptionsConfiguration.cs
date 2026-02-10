namespace SB.ApiAbstractions;

using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

public class ForwardedHeadersOptionsConfiguration : IConfigureOptions<ForwardedHeadersOptions>
{
    private readonly IPNetwork[]? knownNetworks;
    private readonly int? forwardLimit;

    public ForwardedHeadersOptionsConfiguration(IPNetwork[]? knownNetworks, int? forwardLimit)
    {
        this.knownNetworks = knownNetworks;
        this.forwardLimit = forwardLimit;
    }

    public void Configure(ForwardedHeadersOptions options)
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        // there is no need to be able to configure the KnownProxies as aspnet requires
        // that the remote address is from either the KnownNetworks OR the KnownProxies
        // and a network with prefix length of 32 is a single IP address
        foreach (var network in this.knownNetworks ?? Enumerable.Empty<IPNetwork>())
        {
            options.KnownNetworks.Add(network);
        }

        options.ForwardLimit = this.forwardLimit ?? 1;
    }
}

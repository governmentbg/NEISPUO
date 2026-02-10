namespace SB.ExtApi;

using Microsoft.Extensions.Logging;
using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

public class ExtSystemClaimsPrincipalProvider
{
    public const string SubjectClaimType = "Subject";
    public const string ThumbprintClaimType = "Thumbprint";
    public const string ExtSystemIdClaimType = "ExtSystemId";
    public const string ExtSystemTypeClaimType = "ExtSystemType";
    public const string SysUserIdClaimType = "SysUserId";

    private readonly IExtApiAuthCachedQueryStore extApiAuthCachedQueryStore;
    private readonly ILogger<ExtSystemClaimsPrincipalProvider> logger;

    public ExtSystemClaimsPrincipalProvider(
        IExtApiAuthCachedQueryStore extApiAuthCachedQueryStore,
        ILogger<ExtSystemClaimsPrincipalProvider> logger)
    {
        this.extApiAuthCachedQueryStore = extApiAuthCachedQueryStore;
        this.logger = logger;
    }

    public async Task<ClaimsPrincipal?> GetByCertificateThumbprintAsync(
        string certSubject,
        string certThumbprint,
        string authenticationType,
        CancellationToken ct)
    {
        var result = await this.extApiAuthCachedQueryStore.GetExtSystemAsync(certThumbprint, ct);

        if (result == null)
        {
            this.logger.LogWarning("Could not find valid ExtSystem for thumbprint:{thumbprint}.", certThumbprint);

            return null;
        }

        return CreateCertificateClaimsPrincipal(
            certSubject,
            certThumbprint,
            (extSystemId: result.ExtSystemId, extSystemTypes: result.ExtSystemTypes, sysUserId: result.SysUserId),
            authenticationType);
    }

    private static ClaimsPrincipal CreateCertificateClaimsPrincipal(
        string certSubject,
        string certThumbprint,
        (int extSystemId, int[] extSystemTypes, int? sysUserId) attributes,
        string authenticationType)
    {
        List<Claim> claims = new()
        {
            new(SubjectClaimType, certSubject),
            new(ThumbprintClaimType, certThumbprint),
            new(ExtSystemIdClaimType, attributes.extSystemId.ToString()),
        };

        claims.AddRange(attributes.extSystemTypes.Select(extSystemType => new Claim(ExtSystemTypeClaimType, extSystemType.ToString())));

        if (attributes.sysUserId.HasValue)
        {
            claims.Add(new Claim(SysUserIdClaimType, attributes.sysUserId.Value.ToString()));
        }

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                authenticationType));
    }
}

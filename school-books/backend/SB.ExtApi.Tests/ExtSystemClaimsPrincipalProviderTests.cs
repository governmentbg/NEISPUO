namespace SB.ExtApi.Tests;

using Microsoft.Extensions.Logging;
using Moq;
using SB.Domain;
using SB.ExtApi;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static SB.Domain.IExtApiAuthQueryRepository;
using Xunit;

public class ExtSystemClaimsPrincipalProviderTests
{
    private static readonly IEqualityComparer<Claim> claimComparer = KeyEqualityComparer<Claim>.Create(c => (c.Type, c.Value));
    private const int extSystemId = 1001;
    private const int sysUserId = 2001;
    private const string subject = "subject";
    private const string thumbprint = "thumbprint";
    private const string authType = "authType";
    private static (
            IExtApiAuthCachedQueryStore extApiAuthCachedQueryStore,
            ILogger<ExtSystemClaimsPrincipalProvider> logger
        ) GetStandartDependencies()
            => (
                extApiAuthCachedQueryStore: Mock.Of<IExtApiAuthCachedQueryStore>(),
                logger: Mock.Of<ILogger<ExtSystemClaimsPrincipalProvider>>()
            );

    [Fact]
    public async Task Should_create_principal_for_found_ExtSystem()
    {
        // Setup
        var extApiAuthCachedQueryStore = Mock.Of<IExtApiAuthCachedQueryStore>(
            m => m.GetExtSystemAsync(thumbprint, default) == Task.FromResult(
                new GetExtSystemVO(extSystemId, new[] { AuthorizationConstants.ExtSystemTypeSchoolBooks }, sysUserId)));
        var (
            _,
            logger
        ) = GetStandartDependencies();
        var provider = new ExtSystemClaimsPrincipalProvider(extApiAuthCachedQueryStore, logger);

        // Act
        var claimsPrincipal = await provider.GetByCertificateThumbprintAsync(
            subject,
            thumbprint,
            authType,
            default);

        // Verify
        Assert.NotNull(claimsPrincipal);
        AssertUtils.SetsEqualWithoutDuplicates(
            new Claim[]
            {
                new(ExtSystemClaimsPrincipalProvider.SubjectClaimType, subject),
                new(ExtSystemClaimsPrincipalProvider.ThumbprintClaimType, thumbprint),
                new(ExtSystemClaimsPrincipalProvider.ExtSystemIdClaimType, extSystemId.ToString()),
                new(ExtSystemClaimsPrincipalProvider.ExtSystemTypeClaimType, AuthorizationConstants.ExtSystemTypeSchoolBooks.ToString()),
                new(ExtSystemClaimsPrincipalProvider.SysUserIdClaimType, sysUserId.ToString())
            },
            claimsPrincipal!.Claims,
            claimComparer);
    }

    [Fact]
    public async Task Should_return_null_if_ExtSystem_does_not_exist()
    {
        // Setup
        var (
            extApiAuthCachedQueryStore,
            logger
        ) = GetStandartDependencies();
        var provider = new ExtSystemClaimsPrincipalProvider(extApiAuthCachedQueryStore, logger);

        // Act
        var claimsPrincipal = await provider.GetByCertificateThumbprintAsync(
            subject,
            thumbprint,
            authType,
            default);

        // Verify
        Assert.Null(claimsPrincipal);
    }
}

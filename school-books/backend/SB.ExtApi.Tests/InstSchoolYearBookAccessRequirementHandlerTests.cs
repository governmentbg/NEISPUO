namespace SB.ExtApi.Tests;

using HttpContextMoq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using SB.Domain;
using SB.ExtApi;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class InstSchoolYearBookAccessRequirementTests
{
    private const int schoolYear = 2021;
    private const int instId = 300125;
    private const int extSystemId = 1001;
    private const int sysUserId = 2001;

    private static HttpContextMock GetHttpContextMock()
    {
        var mock = new HttpContextMock();
        mock.RequestMock.HttpContextMock.FeaturesMock.Mock
            .Setup(m => m.Get<IRouteValuesFeature>())
            .Returns(new RouteValuesFeature()
            {
                RouteValues = new RouteValueDictionary()
                {
                    {"schoolYear", schoolYear.ToString()},
                    {"institutionId", instId.ToString()}
                }
            });
        mock.UserMock.IdentityMock.Mock.Setup(m => m.IsAuthenticated).Returns(true);
        mock.UserMock.IdentityMock.Mock.Setup(m => m.Claims)
            .Returns(new Claim[]
            {
                new(ExtSystemClaimsPrincipalProvider.ExtSystemIdClaimType, extSystemId.ToString()),
                new(ExtSystemClaimsPrincipalProvider.ExtSystemTypeClaimType, AuthorizationConstants.ExtSystemTypeSchoolBooks.ToString()),
                new(ExtSystemClaimsPrincipalProvider.SysUserIdClaimType, sysUserId.ToString()),
            });
        mock.UserMock.IdentityMock.Mock.Setup(m => m.FindFirst(It.IsAny<string>())).CallBase();
        mock.UserMock.IdentityMock.Mock.Setup(m => m.FindAll(It.IsAny<string>())).CallBase();
        return mock;
    }

    private static (
            IHttpContextAccessor httpContextAccessor,
            ICommonCachedQueryStore commonCachedQueryStore,
            ILogger<InstSchoolYearBookAccessRequirementHandler> logger,
            AuthorizationHandlerContext context
        ) GetStandartDependencies()
            => (
                httpContextAccessor: Mock.Of<IHttpContextAccessor>(m => m.HttpContext == GetHttpContextMock()),
                commonCachedQueryStore: Mock.Of<ICommonCachedQueryStore>(),
                logger: Mock.Of<ILogger<InstSchoolYearBookAccessRequirementHandler>>(),
                context: new AuthorizationHandlerContext(
                    new[] { new InstSchoolYearBookAccessRequirement() },
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                                new(ExtSystemClaimsPrincipalProvider.ExtSystemIdClaimType, extSystemId.ToString()),
                                new(ExtSystemClaimsPrincipalProvider.ExtSystemTypeClaimType, AuthorizationConstants.ExtSystemTypeSchoolBooks.ToString()),
                                new(ExtSystemClaimsPrincipalProvider.SysUserIdClaimType, sysUserId.ToString()),
                            }
                        )
                    ),
                    null)
            );

    [Fact]
    public async Task Should_succeed_on_existing_ExtSystemIsInstCBExtProvider()
    {
        // Setup
        var commonCachedQueryStore = Mock.Of<ICommonCachedQueryStore>(
            m => m.GetExtSystemIsInstCBExtProviderAsync(extSystemId, schoolYear, instId, default)
                == Task.FromResult(true));
        var (
            httpContextAccessor,
            _,
            logger,
            context
        ) = GetStandartDependencies();
        var requirementHandler =
            new InstSchoolYearBookAccessRequirementHandler(
                httpContextAccessor,
                commonCachedQueryStore,
                logger);

        // Act
        await requirementHandler.HandleAsync(context);

        // Verify
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Should_fail_on_missing_ExtSystemIsInstCBExtProvider()
    {
        // Setup
        var (
            httpContextAccessor,
            commonCachedQueryStore,
            logger,
            context
        ) = GetStandartDependencies();
        var requirementHandler =
            new InstSchoolYearBookAccessRequirementHandler(
                httpContextAccessor,
                commonCachedQueryStore,
                logger);

        // Act
        await requirementHandler.HandleAsync(context);

        // Verify
        Assert.False(context.HasSucceeded);
    }
}

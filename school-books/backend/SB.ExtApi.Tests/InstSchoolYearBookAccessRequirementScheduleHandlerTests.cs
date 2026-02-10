namespace SB.ExtApi.Tests;

using HttpContextMoq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using SB.Domain;
using SB.ExtApi;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class InstSchoolYearBookAccessRequirementScheduleHandlerTests
{
    private const int schoolYear = 2021;
    private const int instId = 300125;
    private const int extSystemId = 1001;
    private const int sysUserId = 2001;

    private static HttpContextMock GetHttpContextMock(ControllerActionDescriptor? actionDescriptor)
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
                new(ExtSystemClaimsPrincipalProvider.ExtSystemTypeClaimType, AuthorizationConstants.ExtSystemTypeSchedule.ToString()),
                new(ExtSystemClaimsPrincipalProvider.SysUserIdClaimType, sysUserId.ToString()),
            });
        mock.UserMock.IdentityMock.Mock.Setup(m => m.FindFirst(It.IsAny<string>())).CallBase();
        mock.UserMock.IdentityMock.Mock.Setup(m => m.FindAll(It.IsAny<string>())).CallBase();

        if (actionDescriptor != null)
        {
            mock.RequestMock.HttpContextMock.FeaturesMock.Mock
                .Setup(m => m.Get<IEndpointFeature>())
                .Returns(
                    Mock.Of<IEndpointFeature>(m =>
                        m.Endpoint == new Endpoint(
                            null,
                            new EndpointMetadataCollection(actionDescriptor),
                            null)));
        }

        return mock;
    }

    [Theory,
        InlineData(true, true, true),
        InlineData(true, false, false),
        InlineData(false, true, false),
        InlineData(false, false, false)]
    public async Task Should_succeed_on_existing_ExtSystemIsInstScheduleExtProvider_and_existing_AdditionalAccessAttribute(
        bool extSystemIsInstScheduleExtProvider,
        bool hasAdditionalAccessAttribute,
        bool hasSucceeded)
    {
        // Setup
        var commonCachedQueryStore = Mock.Of<ICommonCachedQueryStore>(
            m => m.GetExtSystemIsInstScheduleExtProviderAsync(extSystemId, schoolYear, instId, default)
                == Task.FromResult(extSystemIsInstScheduleExtProvider));

        var httpContextAccessor = Mock.Of<IHttpContextAccessor>(
            m => m.HttpContext == GetHttpContextMock(
                hasAdditionalAccessAttribute
                    ? new ControllerActionDescriptor()
                    {
                        MethodInfo = Mock.Of<MethodInfo>(
                            mi => mi.GetCustomAttributes(false) == new object[]
                                {
                                    new AdditionalAccessAttribute(AuthorizationConstants.ScheduleProviderAdditionalAccess)
                                })
                    }
                    : null));

        var logger = Mock.Of<ILogger<InstSchoolYearBookAccessRequirementScheduleHandler>>();
        var context = new AuthorizationHandlerContext(
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
            null);

        var requirementHandler =
            new InstSchoolYearBookAccessRequirementScheduleHandler(
                httpContextAccessor,
                commonCachedQueryStore,
                logger);

        // Act
        await requirementHandler.HandleAsync(context);

        // Verify
        Assert.Equal(hasSucceeded, context.HasSucceeded);
    }
}

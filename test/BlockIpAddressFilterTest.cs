using BlockIpAddressFilter;
using BlockIpAddressFilter.Abstractions;
using BlockIpAddressFilter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace TestProject
{
    public class BlockIpAddressFilterTest
    {
        private readonly Mock<IBlockIpService> _mockIpBlockingService = new();
        private readonly BlockIpService _ipBlockingService;
        private const string IpAddress = "127.0.0.1";

        public BlockIpAddressFilterTest()
        {
            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns(IpAddress);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection(It.IsAny<string>())).Returns(mockConfigSection.Object);

            _ipBlockingService = new BlockIpService(mockConfig.Object);
        }

        [Fact]
        public void GivenIpAddressIsBlocked_WhenCheckingIfIpIsBlocked_ThenReturnsTrue()
        {
            // Arrange
            var ipAddress = IPAddress.Parse(IpAddress);

            // Act
            var result = _ipBlockingService.IsBlocked(ipAddress);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GivenIpAddressIsNotBlocked_WhenCheckingIfIpIsBlocked_ThenReturnsFalse()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("127.0.0.2");

            // Act
            var result = _ipBlockingService.IsBlocked(ipAddress);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GivenABlockedIpAddress_WhenRequestIsMadeWithActionFilter_ThenResponseIsForbidden()
        {
            // Arrange
            const int expectedStatusCode = (int)HttpStatusCode.Forbidden;
            var ipAddress = IPAddress.Parse("127.0.0.1");

            _mockIpBlockingService.Setup(x => x.IsBlocked(ipAddress)).Returns(true);

            var httpContext = new DefaultHttpContext
            {
                Connection =
                {
                    RemoteIpAddress = ipAddress
                }
            };

            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(
                    httpContext, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new object());

            var actionFilter = new IpBlockActionFilter(_mockIpBlockingService.Object);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result as StatusCodeResult;

            // Assert
            Assert.Equal(expectedStatusCode, result.StatusCode);
        }

        [Fact]
        public void GivenAnUnblockedIpAddress_WhenRequestIsMadeWithActionFilter_ThenResultIsNull()
        {
            // Arrange
            var ipAddress = IPAddress.Parse("127.0.0.1");

            _mockIpBlockingService.Setup(x => x.IsBlocked(ipAddress)).Returns(false);

            var httpContext = new DefaultHttpContext
            {
                Connection =
                {
                    RemoteIpAddress = ipAddress
                }
            };

            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(
                    httpContext, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new object());

            var actionFilter = new IpBlockActionFilter(_mockIpBlockingService.Object);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.Null(actionExecutingContext.Result);
        }
    }
}

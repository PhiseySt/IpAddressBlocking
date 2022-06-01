using BlockIpAddressFilter.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlockIpAddressFilter
{
    public class IpBlockActionFilter : ActionFilterAttribute
    {
        private readonly IBlockIpService _ipBlockingService;

        public IpBlockActionFilter(IBlockIpService ipBlockingService)
        {
            _ipBlockingService = ipBlockingService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

            var isBlocked = _ipBlockingService.IsBlocked(remoteIp!);

            if (isBlocked)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

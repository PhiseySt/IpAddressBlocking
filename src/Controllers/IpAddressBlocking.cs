using Microsoft.AspNetCore.Mvc;

namespace BlockIpAddressFilter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpBlockController : ControllerBase
    {
        [HttpGet("unblocked")]
        public string Unblocked()
        {
            return "Unblocked access";
        }

        [ServiceFilter(typeof(IpBlockActionFilter))]
        [HttpGet("blocked")]
        public string Blocked()
        {
            return "Blocked access";
        }
    }
}
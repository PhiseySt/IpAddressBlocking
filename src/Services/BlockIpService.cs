using System.Net;
using BlockIpAddressFilter.Abstractions;

namespace BlockIpAddressFilter.Services
{
    public class BlockIpService : IBlockIpService
    {
        private readonly List<string> _blockedIps;

        public BlockIpService(IConfiguration configuration)
        {
            var blockedIps = configuration.GetValue<string>("BlockedIPs");
            _blockedIps = blockedIps.Split(',').ToList();
        }

        public bool IsBlocked(IPAddress ipAddress) => _blockedIps.Contains(ipAddress.ToString());
    }

}

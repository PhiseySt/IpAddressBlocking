using System.Net;

namespace BlockIpAddressFilter.Abstractions
{
    public interface IBlockIpService
    {
        bool IsBlocked(IPAddress ipAddress);
    }
}

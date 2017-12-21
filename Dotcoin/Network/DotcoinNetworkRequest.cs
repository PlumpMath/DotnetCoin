using System.Net;

namespace Dotcoin.Network
{
    public class DotcoinNetworkRequest
    {
        public enum RequestMethods
        {
            GetMasterIp,
            GetNetworkIps,
            GetBlockChain,
            InvalidRequestRecieveds
        }
        public IPAddress SenderIp;
        public RequestMethods RequestMethod;
    }
}
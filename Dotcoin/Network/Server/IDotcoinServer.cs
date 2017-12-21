using System;
using System.Net;

namespace Dotcoin.Network.Server
{
    public interface IDotcoinServer : IDisposable
    {
        void StartServer(IPAddress ipAddress, int port);
    }
}
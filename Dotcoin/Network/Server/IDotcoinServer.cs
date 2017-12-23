using System;
using System.Collections.Generic;
using System.Net;

namespace Dotcoin.Network.Server
{
    public interface IDotcoinServer : IDisposable
    {
        void StartServer(IPAddress ipAddress, int port);
        void SendRequest(List<IPAddress> ipAddresses, DotcoinNetworkRequest networkRequest, Action onComplete = null);
        bool Ping(IPAddress ipAddress);
        DotcoinNetworkRequest[] GetRequests(int num);

    }
}
using System;
using System.Net;

namespace Dotcoin.Network.Server
{
    public class DotcoinTestServer : IDotcoinServer
    {
        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }

        public void StartServer(IPAddress ipAddress, int port)
        {
            Console.WriteLine("Do stuff");
        }
    }
}
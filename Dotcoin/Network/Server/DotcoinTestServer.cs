using System;
using System.Collections.Generic;
using System.Net;

namespace Dotcoin.Network.Server
{
    public class DotcoinTestServer : IDotcoinServer
    {
        private Queue<DotcoinNetworkRequest> _fakeMessageQueue = new Queue<DotcoinNetworkRequest>();
        
        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }

        public void StartServer(IPAddress ipAddress, int port)
        {
            Console.WriteLine("Do stuff");
        }

        public void SendRequest(List<IPAddress> ipAddresses, DotcoinNetworkRequest networkRequest, Action onComplete = null)
        {
            onComplete?.Invoke();
        }

        public bool Ping(IPAddress ipAddress)
        {
            return true;
        }

        public DotcoinNetworkRequest[] GetRequests(int num)
        {
            var topNumRequests = new DotcoinNetworkRequest[num];
            
            for (int i = 0; i < num; ++i)
            {
                if (_fakeMessageQueue.Count == 0)
                {
                    break;
                }
                topNumRequests[i] = _fakeMessageQueue.Dequeue();
            }

            return topNumRequests;
        }

        public void AddMessage(DotcoinNetworkRequest networkRequest)
        {
            _fakeMessageQueue.Enqueue(networkRequest);
        }
    }
}
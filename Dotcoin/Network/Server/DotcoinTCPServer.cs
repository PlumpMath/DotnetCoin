using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using static System.Text.Encoding;
using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin.Network.Server
{
    /// <summary>
    /// This class managers the listening of inbound requests
    /// </summary>
    public class DotcoinTCPServer : IDotcoinServer
    {
        private TcpListener _listener;
        private Thread _listenThread;
        private volatile bool _runListenThread = true;

        private ConcurrentQueue<DotcoinNetworkRequest> _requests = new ConcurrentQueue<DotcoinNetworkRequest>();

        public void StartServer(IPAddress ipAddress, int port)
        {
            _listener = new TcpListener(ipAddress, port);

            _listenThread = new Thread(Listen);

            _listenThread.Start();
        }

        private void Listen()
        {
            
            Console.WriteLine("Starting listen thread");
            
            _listener.Start();

            if (_listener == null || !_runListenThread)
            {
                return;
            }

            while (_runListenThread)
            {
                Console.WriteLine("Waiting for client...");
                
                var clientTask = _listener.AcceptTcpClientAsync();

                if (clientTask.Result != null)
                {
                    Console.WriteLine("Client connected.  Waiting for data");

                    var client = clientTask.Result;

                    string message = "";

                    while (message != null && !message.StartsWith("quit"))
                    {
                        byte[] buffer = new byte[1024];
                        client.GetStream().Read(buffer, 0, buffer.Length);

                        message = ASCII.GetString(buffer);
                        
                        var request = DeserializeObject<DotcoinNetworkRequest>(message);

                        if (request != null)
                        {
                            _requests.Enqueue(request);
                        }
                        
                    }
                    Console.WriteLine("Closing connection.");
                    client.GetStream().Dispose();
                }
            }
            
            _listener.Stop();
        }

        //need to clean up the threads
        public void Dispose()
        {
            Console.WriteLine("Attempting to clean up the server");
            
            _runListenThread = false;

            _listenThread.Join();
            
            Console.WriteLine("Server cleaned up");
        }
    }
}
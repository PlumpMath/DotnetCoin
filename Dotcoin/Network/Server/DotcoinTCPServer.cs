using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


using static System.Text.Encoding;
using static Newtonsoft.Json.JsonConvert;
using static Dotcoin.Network.DotcoinNetworkRequest;

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
        private int _port;
        private IPAddress _ipAddress;
        
        private ConcurrentQueue<DotcoinNetworkRequest> _requests = new ConcurrentQueue<DotcoinNetworkRequest>();
       
        public void StartServer(IPAddress ipAddress, int port)
        {
            if (_listener != null)
            {
                throw new Exception("Server already running");
            }
            
            _listener = new TcpListener(ipAddress, port);

            _listenThread = new Thread(Listen);

            _listenThread.Start();

            _port = port;
            _ipAddress = ipAddress;
        }

        public void SendRequest(List<IPAddress> ipAddresses, DotcoinNetworkRequest networkRequest, Action onComplete = null)
        {
            var data = SerializeObject(networkRequest).ToByteArray();
            
            Parallel.ForEach(ipAddresses, ip =>
            {
                SendRequest(ip, data);
            }); 
        }

        public bool Ping(IPAddress ipAddress)
        {
            var request = new DotcoinNetworkRequest
            {
                RequestMethod = RequestMethods.Ping,
                SenderIp = _ipAddress
            };

            var data = SerializeObject(request).ToByteArray();
            
            SendRequest(ipAddress, data);

            //todo think aobut a good way of doing this
            //maybe do something that will imdediatly reply
            //instead of pusing it on the queue
            return true;
        }
        
        public DotcoinNetworkRequest[] GetRequests(int num)
        {
            var topNumRequests = new DotcoinNetworkRequest[num];
            
            for (int i = 0; i < num; ++i)
            {
                if (!_requests.TryDequeue(out topNumRequests[i]))
                {
                    break;
                }    
            }

            return topNumRequests;
        }
        
        //need to clean up the threads
        public void Dispose()
        {
            Console.WriteLine("Attempting to clean up the server");
            
            _runListenThread = false;

            _listenThread.Join();
            
            Console.WriteLine("Server cleaned up");
        }

        //Handles listening and enquening requests into a concurrent queue
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

        private void SendRequest(IPAddress ip, byte[] data)
        {
            using (var client = new TcpClient(ip.ToString(), _port))
            {
                using (var stream = client.GetStream())
                {
                    try
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(string.Format("Error writing to {0}\nError mesage of {1}\n", ip.ToString(), e.ToString()));
                    }
                }
            }
        }
    }
}
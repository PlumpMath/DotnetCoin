﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin.Network
{
    public class DotcoinNetwork : IDisposable
    {
        //keep track of ip addresses and last sucessful transmission
        private readonly ConcurrentDictionary<IPAddress, DateTime> _addresses = new ConcurrentDictionary<IPAddress, DateTime>();
        private readonly IPAddress _nodeIp;
        private volatile IPAddress _masterIp;
        
        private const int TIMEOUT_SECONDS = 1000;
        private const int PORT = 5000;
        
        public DotcoinNetwork(IPAddress nodeIp, IPAddress masterIp)
        {
            _nodeIp = nodeIp;
            _masterIp = masterIp;
            
            _addresses.TryAdd(nodeIp, DateTime.Now);

            if (!nodeIp.Equals(masterIp)) //helps prevent glitch when working locally
            {
                _addresses.TryAdd(masterIp, DateTime.Now);
            } 
            
            LoadKnownIps();
            
        }

        public async Task<bool> LoadNetwork()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(string.Format("http://{0}/api", _masterIp));

            var result = await client.GetAsync("Network");

            var json = await result.Content.ReadAsStringAsync();

            var listAddreses = DeserializeObject<List<IPAddress>>(json);

            if (listAddreses.Count == 0)
                return false;
            
            foreach (var ips in listAddreses)
            {
                _addresses.TryAdd(ips, DateTime.Now);
            }

            return true;
        }

        public async Task<bool> SendToNetwork(Object data, string route)
        {
            //only get addreses that weve heard from and think
            //are still active
            var ips = _addresses.Where(e => DateTime.Now.Subtract(e.Value).Seconds < TIMEOUT_SECONDS);

            bool @return = true;
            
            Parallel.ForEach(ips, ip =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(string.Format("http://{0}.api", _masterIp));

                var result = client.PostAsJsonAsync(route, data);

                if (result.Result.StatusCode != HttpStatusCode.OK)
                {
                    @return = false;
                }
            });

            return @return;
        }

        public bool PingIp(IPAddress address)
        {
            return true;
        }
 
        public void Dispose()
        {
            var knownIps = _addresses.Keys.ToList();
            
            DotcoinIpManager.SaveIpAddresses(knownIps);
        }
        
        private void LoadKnownIps()
        {
            var knownIps = DotcoinIpManager.LoadIpAddresses();

            if (knownIps.Count == 0)
            {
                return;
            }

            Parallel.ForEach(knownIps, e =>
            {
                if (PingIp(e))
                {
                    _addresses[e] = DateTime.Now;
                }
                else
                {
                    _addresses[e] = DateTime.MinValue;
                }
            });
        }

     
    }
}
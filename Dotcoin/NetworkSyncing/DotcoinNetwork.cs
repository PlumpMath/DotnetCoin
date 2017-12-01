using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin.NetworkSyncing
{
    public class DotcoinNetwork
    {
        //keep track of ip addresses and last sucessful transmission
        private readonly Dictionary<IPAddress, DateTime> _addresses = new Dictionary<IPAddress, DateTime>();
        private readonly IPAddress _nodeIp;
        private volatile IPAddress _masterIp;
        
        private const int TIMEOUT_SECONDS = 1000;
        private const int PORT = 5000;
        
        public DotcoinNetwork(IPAddress nodeIp, IPAddress masterIp)
        {
            _addresses.Add(nodeIp, DateTime.Now);
            _addresses.Add(masterIp, DateTime.Now);
            
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
                _addresses.Add(ips, DateTime.Now);
            }

            return true;
        }

        public async Task<bool> PostToNetwork(Object data, string route)
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
    }
}
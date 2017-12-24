using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using Dotcoin.Network;
using Dotcoin.Network.Server;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DotcoinTests
{
    public class DotcoinNetworkTests
    {
        private readonly DotcoinNetwork _testNetwork;
        const string TEST_NETWORK_IP_FILE = "testNetworkFile.txt";
        private List<string> _knownIps = new List<string>
        {
            "192.168.1.1",
            "192.168.1.4"
        };
        
        public DotcoinNetworkTests()
        {
            createIpFile();
            
            _testNetwork = new DotcoinNetwork(IPAddress.Any, IPAddress.Any, new DotcoinTestServer(), TEST_NETWORK_IP_FILE);
        }
        
        [Fact]
        public void Test_LoadingKnownIps()
        {
            var knownIps = _testNetwork.Network.Select(e => e.ToString()).ToList();

            var good = _knownIps.Count <= knownIps.Count;
            
            foreach (var ip in _knownIps)
            {
                if (!knownIps.Contains(ip))
                {
                    good = false;
                }
            }
            
            Assert.True(good);
        }

        private void createIpFile()
        {
            DotcoinIpManager.SaveIpAddresses(_knownIps, TEST_NETWORK_IP_FILE);
        }
    }
}
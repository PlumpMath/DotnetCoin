using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Dotcoin.Network;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DotcoinTests
{
    public class DotcoinIpManagerTests : IDisposable
    {
        private const string TEST_CREATE = "textCreate.txt";
        private const string DEFAULT_TEST_FILE = "IpManagerTestFile.txt";

        public DotcoinIpManagerTests()
        {
             CreateTestFile();   
        }
        
        [Fact]
        public void Test_CreatingFile()
        {
            var ipAddresses = new List<string>()
            {
                "192.168.1.1"
            };
            
            DotcoinIpManager.SaveIpAddresses(ipAddresses, TEST_CREATE);
            
            
            Assert.True(File.Exists(TEST_CREATE));
        }

        [Fact]
        public void Test_Savingfile()
        {
            var ipAddresses = new List<string>()
            {
                "192.168.1.1",
                "192.168.1.2"
            };
            
            DotcoinIpManager.SaveIpAddresses(ipAddresses, DEFAULT_TEST_FILE);

            var fileAddresses = DotcoinIpManager.LoadIpAddresses(DEFAULT_TEST_FILE);

            var pass = fileAddresses.Count == ipAddresses.Count;
            
            foreach (var address in fileAddresses)
            {
                var sAddress = address.ToString();

                if (!ipAddresses.Contains(sAddress))
                {
                    pass = false;
                }
            }
            
            Assert.True(pass);
        }
        
        public void Dispose()
        {
            CleanupTestCreatingFile();
        }

        private void CreateTestFile()
        {
            if (!File.Exists(DEFAULT_TEST_FILE))
            {
                var ipAddresses = new List<string>()
                {
                    "192.168.1.1"
                };
            
                DotcoinIpManager.SaveIpAddresses(ipAddresses, DEFAULT_TEST_FILE);
            }   
        }
        private void CleanupTestCreatingFile()
        {
            File.Delete(TEST_CREATE);
        }
    }
}
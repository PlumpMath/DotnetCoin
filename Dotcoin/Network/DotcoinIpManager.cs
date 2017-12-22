using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin.Network
{
    public static class DotcoinIpManager
    {
        public const string FILE_LOCATION = "NetworkIPs.txt";
        
        public static List<IPAddress> LoadIpAddresses(string overideLocation = "")
        {
            if (overideLocation == "")
            {
                overideLocation = FILE_LOCATION;
            }
            
            if (File.Exists(overideLocation))
            {
                var fileContents = File.ReadAllText(overideLocation);

                var listStringIps = DeserializeObject<List<string>>(fileContents);

                var returnIps = new List<IPAddress>();

                foreach (var stringIp in listStringIps)
                {
                    returnIps.Add(IPAddress.Parse(stringIp));
                }

                return returnIps;
            }
            
            return new List<IPAddress>();
        }

        public static void SaveIpAddresses(List<string> ipAddresses, string overideLocation = "")
        {
            if (overideLocation == "")
            {
                overideLocation = FILE_LOCATION;
            }
            
            using (var file = File.CreateText(overideLocation))
            {
                var stuffToWrite = SerializeObject(ipAddresses);
                
                file.Write(stuffToWrite);
            }
        }
    }
}
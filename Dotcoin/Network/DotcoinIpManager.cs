using System.Collections.Generic;
using System.IO;
using System.Net;

using static Newtonsoft.Json.JsonConvert;

namespace Dotcoin.Network
{
    public static class DotcoinIpManager
    {
        private const string FILE_LOCATION = "NetworkIPs.txt";
        
        public static List<IPAddress> LoadIpAddresses()
        {
            if (File.Exists(FILE_LOCATION))
            {
                var fileContents = File.ReadAllText(FILE_LOCATION);

                return DeserializeObject<List<IPAddress>>(fileContents);
            }
            
            return new List<IPAddress>();
        }

        public static void SaveIpAddresses(List<IPAddress> ipAddresses)
        {
            using (var file = File.CreateText(FILE_LOCATION))
            {
                var stuffToWrite = SerializeObject(ipAddresses);
                
                file.Write(stuffToWrite);
            }
        }
    }
}
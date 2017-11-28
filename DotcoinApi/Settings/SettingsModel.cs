using System.Collections.Generic;
using System.Net;

namespace DotcoinApi.Settings
{
    public class SettingsModel
    {
        //This will be a list of all the ipaddreses on the network
        public List<IPAddress> IpAddresses;

        //This will be used to togle if your role is to delegate ips or not
        public bool Master = true;

        //This will be used to locate the master ip address if you are queried
        public int MasterIpIndex = 0;
    }
}
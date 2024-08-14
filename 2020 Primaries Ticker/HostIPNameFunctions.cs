using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace _2020_Primaries_Ticker
{
    public class HostIPNameFunctions
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetLocalIPAddress()
        {
            string ipAddress = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip.ToString();
                    }
                }
                if (ipAddress == string.Empty)
                {
                    throw new Exception("Local IP Address Not Found!");
                }
            }
            catch (Exception ex)
            {
                // Log error
                log.Error("HostIPNameFunctions Exception occurred while trying to get IP address: " + ex);
                log.Debug("HostIPNameFunctions Exception occurred while trying to get IP address", ex);

                Election_Ticker_2020.ErrorFlag = true;
            }
            return ipAddress;
        }

        // Get thre host name based on the IP address
        public static string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                // Log error
                log.Error("HostIPNameFunctions Exception occurred while trying to get host name: " + ex);
                log.Debug("HostIPNameFunctions Exception occurred while trying to get host name", ex);

                Election_Ticker_2020.ErrorFlag = true;
            }

            return null;
        }
    }
}
